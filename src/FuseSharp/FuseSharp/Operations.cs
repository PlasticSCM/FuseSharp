using System;
using System.Runtime.InteropServices;

using Mono.Unix;

namespace FuseSharp
{
    public delegate int GetPathStatusCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, IntPtr stat);
    public delegate int ReadSymbolicLinkCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, IntPtr buf, ulong bufsize);
    public delegate int CreateSpecialFileCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, uint perms, ulong dev);
    public delegate int CreateDirectoryCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, uint mode);
    public delegate int RemoveFileCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path);
    public delegate int RemoveDirectoryCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path);
    public delegate int CreateSymbolicLinkCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string oldpath,
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string newpath);
    public delegate int RenamePathCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string oldpath,
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string newpath);
    public delegate int CreateHardLinkCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string oldpath,
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string newpath);
    public delegate int ChangePathPermissionsCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, uint mode);
    public delegate int ChangePathOwnerCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, long owner, long group);
    public delegate int TruncateFileb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, long length);
    public delegate int ChangePathTimesCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, IntPtr buf);
    public delegate int OpenHandleCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, IntPtr info);
    public delegate int ReadHandleCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path,
            [Out, MarshalAs (UnmanagedType.LPArray, ArraySubType=UnmanagedType.U1, SizeParamIndex=2)]
            byte[] buf, ulong size, long offset, IntPtr info, out int bytesRead);

    public delegate int WriteHandleCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path,
            [In, MarshalAs (UnmanagedType.LPArray, ArraySubType=UnmanagedType.U1, SizeParamIndex=2)]
            byte[] buf, ulong size, long offset, IntPtr info, out int bytesWritten);

    public delegate int GetFileSystemStatusCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, IntPtr buf);
    public delegate int FlushHandleCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, IntPtr info);
    public delegate int ReleaseHandleCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, IntPtr info);
    public delegate int SynchronizeHandleCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, bool onlyUserData, IntPtr info);
    public delegate int SetPathExtendedAttributeCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path,
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string name,
            [In, MarshalAs (UnmanagedType.LPArray, ArraySubType=UnmanagedType.U1, SizeParamIndex=3)]
            byte[] value, ulong size, int flags);
    public delegate int GetPathExtendedAttributeCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path,
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string name,
            [Out, MarshalAs (UnmanagedType.LPArray, ArraySubType=UnmanagedType.U1, SizeParamIndex=3)]
            byte[] value, ulong size, out int bytesWritten);
    public delegate int ListPathExtendedAttributesCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path,
            [Out, MarshalAs (UnmanagedType.LPArray, ArraySubType=UnmanagedType.U1, SizeParamIndex=2)]
            byte[] list, ulong size, out int bytesWritten);
    public delegate int RemovePathExtendedAttributeCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path,
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string name);
    public delegate int OpenDirectoryCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, IntPtr info);
    public delegate int ReadDirectoryCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, IntPtr buf, IntPtr filler,
            long offset, IntPtr info, IntPtr stbuf);
    public delegate int ReleaseDirectoryCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, IntPtr info);
    public delegate int SynchronizeDirectoryCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, bool onlyUserData, IntPtr info);
    public delegate IntPtr InitCb(IntPtr conn);
    public delegate void DestroyCb(IntPtr conn);
    public delegate int AccessPathCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, int mode);
    public delegate int CreateHandleCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, uint mode, IntPtr info);
    public delegate int TruncateHandleCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, long length, IntPtr info);
    public delegate int GetHandleStatusCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, IntPtr buf, IntPtr info);
    public delegate int LockHandleCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, IntPtr info, int cmd, IntPtr flockp);
    // TODO: utimens
    public delegate int MapPathLogicalToPhysicalIndexCb(
            [MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(FileNameMarshaler))]
            string path, ulong logical, out ulong physical);

    [StructLayout(LayoutKind.Sequential)]
    public class Operations
    {
        public GetPathStatusCb getattr;
        public ReadSymbolicLinkCb readlink;
        public CreateSpecialFileCb mknod;
        public CreateDirectoryCb mkdir;
        public RemoveFileCb unlink;
        public RemoveDirectoryCb rmdir;
        public CreateSymbolicLinkCb symlink;
        public RenamePathCb rename;
        public CreateHardLinkCb link;
        public ChangePathPermissionsCb chmod;
        public ChangePathOwnerCb chown;
        public TruncateFileb truncate;
        public ChangePathTimesCb utime;
        public OpenHandleCb open;
        public ReadHandleCb read;
        public WriteHandleCb write;
        public GetFileSystemStatusCb statfs;
        public FlushHandleCb flush;
        public ReleaseHandleCb release;
        public SynchronizeHandleCb fsync;
        public SetPathExtendedAttributeCb setxattr;
        public GetPathExtendedAttributeCb getxattr;
        public ListPathExtendedAttributesCb listxattr;
        public RemovePathExtendedAttributeCb removexattr;
        public OpenDirectoryCb opendir;
        public ReadDirectoryCb readdir;
        public ReleaseDirectoryCb releasedir;
        public SynchronizeDirectoryCb fsyncdir;
        public InitCb init;
        public DestroyCb destroy;
        public AccessPathCb access;
        public CreateHandleCb create;
        public TruncateHandleCb ftruncate;
        public GetHandleStatusCb fgetattr;
        public LockHandleCb @lock;
        public MapPathLogicalToPhysicalIndexCb bmap;

        public static int SizeOf(Operations operations)
        {
            return Marshal.SizeOf(operations);
        }

        public static IntPtr Allocate(Operations operations)
        {
            int operationsSize = SizeOf(operations);
            IntPtr opPtr = UnixMarshal.AllocHeap(operationsSize);
            Marshal.StructureToPtr(operations, opPtr, false);
            return opPtr;
        }

        public static void Free(IntPtr opPtr)
        {
            Marshal.DestroyStructure(opPtr, typeof(Operations));
            UnixMarshal.FreeHeap(opPtr);
            opPtr = IntPtr.Zero;
        }
    }
}
