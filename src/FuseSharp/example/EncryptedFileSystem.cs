using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Mono.Unix.Native;

namespace FuseSharp.Examples
{
    public class EncryptedFileSystem : FileSystem
    {
        private string _targetRoot;

        private string GetTargetPath(string sourcePath)
        {
            string newSource = sourcePath.Substring(1);
            return Path.Combine(_targetRoot, newSource);
        }

        public EncryptedFileSystem(string targetRoot)
        {
            this._targetRoot = targetRoot;
        }

        private Errno GetLastError()
        {
            Errno errno = Stdlib.GetLastError();
            Trace.TraceError(errno.ToString());
            return errno;
        }

        public override Errno OnGetPathStatus(string path, out Stat stat)
        {
            Trace.WriteLine($"OnGetPathStatus {path}");

            string targetPath = GetTargetPath(path);
            int r = Syscall.lstat(targetPath, out stat);
            if (r < 0)
                return GetLastError();

            return 0;
        }

        private Errno Access(string path, AccessModes mode)
        {
            Trace.WriteLine($"Access {path} {mode}");

            int r = Syscall.access(path, mode);
            if (r < 0)
                return GetLastError();

            return 0;
        }

        public override Errno OnAccessPath(string path, AccessModes mode)
        {
            return Access(GetTargetPath(path), mode);
        }

        public override Errno OnReadDirectory(
            string directory,
            PathInfo info,
            out IEnumerable<DirectoryEntry> paths)
        {
            Trace.WriteLine($"OnReadDirectory {directory}");

            IntPtr dirPtr = info.Handle;
            if (dirPtr == IntPtr.Zero)
            {
                paths = null;
                return GetLastError();
            }

            List<DirectoryEntry> entries = new List<DirectoryEntry>();

            Dirent dirent;
            while ((dirent = Syscall.readdir(dirPtr)) != null)
            {
                entries.Add(new DirectoryEntry(dirent.d_name));
            }

            paths = entries;

            return 0;
        }



        public override Errno OnOpenHandle(string file, PathInfo info)
        {
            Trace.WriteLine($"OnOpenHandle {file} Flags={info.OpenFlags}");

            int fd = Syscall.open(GetTargetPath(file), info.OpenFlags);
            if (fd < 0)
                return GetLastError();

            info.Handle = new IntPtr(fd);
            return 0;
        }

        public unsafe override Errno OnReadHandle(
            string file,
            PathInfo info,
            byte[] buf,
            long offset,
            out int bytesRead)
        {
            Trace.WriteLine($"OnReadHandle {file} Flags={info.OpenFlags}");

            int fd = info.Handle.ToInt32();

            long r;
            fixed (byte* pBuf = buf)
            {
                r = Syscall.pread(fd, pBuf, (ulong)buf.Length, offset);
                if (r < 0)
                {
                    bytesRead = 0;
                    return GetLastError();
                }
                for (int i = 0; i < buf.Length; ++i)
                {
                    ++buf[i];
                }
            }
            bytesRead = (int)r;

            return 0;
        }

        public unsafe override Errno OnWriteHandle(
            string file,
            PathInfo info,
            byte[] buf,
            long offset,
            out int bytesWritten)
        {
            Trace.WriteLine($"OnWriteHandle {file} Flags={info.OpenFlags}");

            int fd = info.Handle.ToInt32();

            long r;
            fixed (byte* pBuf = buf)
            {
                for (int i = 0; i < buf.Length; ++i)
                {
                    --buf[i];
                }
                r = Syscall.pwrite(fd, pBuf, (ulong)buf.Length, offset);
                if (r < 0)
                {
                    bytesWritten = 0;
                    return GetLastError();
                }
            }
            bytesWritten = (int)r;

            return 0;
        }

        public override Errno OnRenamePath(string oldpath, string newpath)
        {
            Trace.WriteLine($"OnRenamePath {oldpath} to {newpath}");

            int r = Stdlib.rename(oldpath, newpath);
            if (r < 0)
                return Stdlib.GetLastError();

            return 0;
        }

        public override Errno OnGetPathExtendedAttribute(string path, string name, byte[] value, out int bytesWritten)
        {
            Trace.WriteLine($"OnGetPathExtendedAttribute {path} Attribute:{name}");

            if (value == null)
            {
                bytesWritten = 0;
                return 0;
            }

            int r = (int)Syscall.getxattr(GetTargetPath(path), name, value, (ulong)value.LongLength);
            if (r == -1)
            {
                bytesWritten = 0;
                return 0; //GetLastError();
            }

            bytesWritten = r;
            return 0;
        }

        public override Errno OnSetPathExtendedAttribute(
            string path, string name, byte[] value, XattrFlags flags)
        {
            Trace.WriteLine($"OnSetPathExtendedAttribute Path:{path} Attribute:{value}");

            int r = Syscall.setxattr(GetTargetPath(path), name, value, flags);
            if (r < 0)
                return GetLastError();

            return 0;
        }

        public override Errno OnRemovePathExtendedAttribute(string path, string name)
        {
            Trace.WriteLine($"OnRemovePathExtendedAttribute Path:{path} Attribute:{name}");
            return 0;
        }

        public override Errno OnListPathExtendedAttributes(string path, out string[] names)
        {
            Trace.WriteLine($"OnListPathExtendedAttributes {path}");
            names = new string[0];
            return 0;
        }

        public override Errno OnOpenDirectory(string directory, PathInfo info)
        {
            Trace.WriteLine($"OnOpenDirectory {directory} Flags={info.OpenFlags}");

            IntPtr dirPtr = Syscall.opendir(GetTargetPath(directory));
            if (dirPtr == IntPtr.Zero)
                return GetLastError();

            info.Handle = dirPtr;
            return 0;
        }

        public override Errno OnReleaseDirectory(string directory, PathInfo info)
        {
            Trace.WriteLine($"OnReleaseDirectory {directory}");

            IntPtr dirPtr = info.Handle;
            int r = Syscall.closedir(dirPtr);
            if (r < 0)
                return GetLastError();

            return 0;
        }

        public override Errno OnCreateDirectory(string directory, FilePermissions mode)
        {
            Trace.WriteLine($"OnCreateDirectory {directory} Permissions={mode}");

            string directoryToCreate = GetTargetPath(directory);
            string parent = Directory.GetParent(directoryToCreate).FullName;
            Errno errno = Access(parent, AccessModes.W_OK);
            if (errno < 0)
                return errno;

            int fd = Syscall.mkdir(directoryToCreate, mode);
            if (fd < 0)
            {
                Trace.WriteLine(string.Format("\tmkdir returned {0}", GetLastError()));
                return GetLastError();
            }

            return 0;
        }



        public override Errno OnCreateHandle(string file, PathInfo info, FilePermissions mode)
        {
            Trace.WriteLine($"OnCreateHandle {file} Flags={info.OpenFlags} Permissions={mode}");
            int fd = Syscall.creat(GetTargetPath(file), mode);
            if (fd < 0)
                return GetLastError();

            info.Handle = new IntPtr(fd);
            return 0;
        }

        public override Errno OnTruncateHandle(string file, PathInfo info, long length)
        {
            Trace.WriteLine($"OnTruncateHandle {file} Flags={info.OpenFlags}");
            int fd = info.Handle.ToInt32();
            int r = Syscall.ftruncate(fd, length);
            if (r < 0)
                return GetLastError();

            return 0;
        }


        public override Errno OnGetHandleStatus(string file, PathInfo info, out Stat buf)
        {
            Trace.WriteLine($"OnGetHandleStatus {file}");
            string targetPath = GetTargetPath(file);
            int r = Syscall.lstat(targetPath, out buf);
            if (r < 0)
                return GetLastError();

            return 0;
        }

        public override Errno OnGetFileSystemStatus(string path, out Statvfs buf)
        {
            Trace.WriteLine($"OnGetFileSystemStatus {path}");
            int r = Syscall.statvfs(GetTargetPath(path), out buf);
            if (r < 0)
                return GetLastError();

            return 0;
        }

        public override Errno OnLockHandle(string file, PathInfo info, FcntlCommand cmd, ref Flock @lock)
        {
            Trace.WriteLine($"OnLockHandle {file}");
            int fd = info.Handle.ToInt32();
            int r = Syscall.fcntl(fd, cmd, ref @lock);
            if (r < 0)
                return GetLastError();

            return 0;
        }

        public override Errno OnFlushHandle(string file, PathInfo info)
        {
            return 0;
        }

        public override Errno OnReleaseHandle(string file, PathInfo info)
        {
            Trace.WriteLine($"OnReleaseHandle {file}");
            int r = Syscall.close(info.Handle.ToInt32());
            if (r < 0)
                return GetLastError();

            return 0;
        }

        public override Errno OnMapPathLogicalToPhysicalIndex(
            string path, ulong logical, out ulong physical)
        {
            Trace.WriteLine($"OnMapPathLogicalToPhysicalIndex {path}");

            physical = ulong.MaxValue;
            return Errno.ENOSYS;
        }

        private Errno Chmod(string path, FilePermissions mode)
        {
            Trace.WriteLine($"Chmod {path} {mode}");

            int r = Syscall.chmod(path, mode);
            if (r < 0)
                return GetLastError();

            return 0;
        }


        public override Errno OnChangePathPermissions(
            string path, FilePermissions mode)
        {
            Trace.WriteLine($"OnChangePathPermissions {path} {mode}");
            return Chmod(GetTargetPath(path), mode);
        }


        private Errno Chown(string path, long owner, long group)
        {
            Trace.WriteLine($"Chown {path} Owner:{owner} Group:{group}");

            int r = Syscall.chown(path, (uint)owner, (uint)group);
            if (r < 0)
                return GetLastError();

            return 0;
        }

        public override Errno OnChangePathOwner(
            string path, long owner, long group)
        {
            Trace.WriteLine($"OnChangePathOwner {path} Owner:{owner} Group:{group}");
            return Chown(GetTargetPath(path), owner, group);
        }


        private Errno Remove(string path)
        {
            Trace.WriteLine($"Remove {path}");
            int r = Stdlib.remove(path);
            if (r < 0)
                return GetLastError();

            return 0;
        }

        public override Errno OnRemoveFile(string file)
        {
            Trace.WriteLine($"OnRemoveFile {file}");
            return Remove(GetTargetPath(file));
        }

        public override Errno OnRemoveDirectory(string directory)
        {
            Trace.WriteLine($"OnRemoveDirectory {directory}");
            return Remove(GetTargetPath(directory));
        }

        public override Errno OnCreateSpecialFile(
            string file, FilePermissions perms, ulong dev)
        {
            Trace.WriteLine($"OnCreateSpecialFile {file} Permissions:{perms}");

            string target = GetTargetPath(file);

            int r = 0;
            if ((perms & FilePermissions.S_IFMT) == FilePermissions.S_IFREG)
            {
                r = Syscall.open(
                        target,
                        OpenFlags.O_CREAT | OpenFlags.O_EXCL | OpenFlags.O_WRONLY,
                        perms);

                if (r >= 0)
                    r = Syscall.close(r);
            }
            else if ((perms & FilePermissions.S_IFMT) == FilePermissions.S_IFIFO)
            {
                r = Syscall.mkfifo(target, perms);
            }
            else
            {
                r = Syscall.mknod(target, perms, dev);
            }

            if (r < 0)
            {
                return GetLastError();
            }
            return 0;
        }
    }
}
