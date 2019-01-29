using System;
using System.Collections.Generic;

using Mono.Unix.Native;

namespace FuseSharp
{
    public class FileSystem : IDisposable
    {
        public FileSystem() { }

        public void Dispose() { }

        public virtual Errno OnGetPathStatus(string path, out Stat stat)
        {
            stat = new Stat();
            return Errno.ENOSYS;
        }

        public virtual Errno OnReadSymbolicLink(string link, out string target)
        {
            target = null;
            return Errno.ENOSYS;
        }

        public virtual Errno OnCreateSpecialFile(
            string file, FilePermissions perms, ulong dev)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnCreateDirectory(
            string directory, FilePermissions mode)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnRemoveFile(string file)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnRemoveDirectory(string directory)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnCreateSymbolicLink(string target, string link)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnRenamePath(string oldpath, string newpath)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnCreateHardLink(string oldpath, string link)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnChangePathPermissions(
            string path, FilePermissions mode)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnChangePathOwner(
            string path, long owner, long group)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnTruncateFile(string file, long length)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnChangePathTimes(string path, ref Utimbuf buf)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnOpenHandle(string file, PathInfo info)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnReadHandle(
            string file,
            PathInfo info,
            byte[] buf,
            long offset,
            out int bytesRead)
        {
            bytesRead = 0;
            return Errno.ENOSYS;
        }

        public virtual Errno OnWriteHandle(
            string file,
            PathInfo info,
            byte[] buf,
            long offset,
            out int bytesWritten)
        {
            bytesWritten = 0;
            return Errno.ENOSYS;
        }

        public virtual Errno OnGetFileSystemStatus(
            string path, out Statvfs buf)
        {
            buf = new Statvfs();
            return Errno.ENOSYS;
        }

        public virtual Errno OnFlushHandle(string file, PathInfo info)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnReleaseHandle(string file, PathInfo info)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnSynchronizeHandle(
            string file, PathInfo info, bool onlyUserData)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnSetPathExtendedAttribute(
            string path, string name, byte[] value, XattrFlags flags)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnGetPathExtendedAttribute(
            string path, string name, byte[] value, out int bytesWritten)
        {
            bytesWritten = 0;
            return Errno.ENOSYS;
        }

        public virtual Errno OnListPathExtendedAttributes(
            string path, out string[] names)
        {
            names = null;
            return Errno.ENOSYS;
        }

        public virtual Errno OnRemovePathExtendedAttribute(
            string path, string name)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnOpenDirectory(string directory, PathInfo info)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnReadDirectory(
            string directory,
            PathInfo info,
            out IEnumerable<DirectoryEntry> paths)
        {
            paths = null;
            return Errno.ENOSYS;
        }

        public virtual Errno OnReleaseDirectory(
            string directory, PathInfo info)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnSynchronizeDirectory(
            string directory, PathInfo info, bool onlyUserData)
        {
            return Errno.ENOSYS;
        }

        public virtual void OnInit(ConnectionInfo connection)
        {
        }

        public virtual Errno OnAccessPath(string path, AccessModes mode)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnCreateHandle(
            string file, PathInfo info, FilePermissions mode)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnTruncateHandle(
            string file, PathInfo info, long length)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnGetHandleStatus(
            string file, PathInfo info, out Stat buf)
        {
            buf = new Stat();
            return Errno.ENOSYS;
        }

        public virtual Errno OnLockHandle(
            string file, PathInfo info, FcntlCommand cmd, ref Flock @lock)
        {
            return Errno.ENOSYS;
        }

        public virtual Errno OnMapPathLogicalToPhysicalIndex(
            string path, ulong logical, out ulong physical)
        {
            physical = ulong.MaxValue;
            return Errno.ENOSYS;
        }

    }
}
