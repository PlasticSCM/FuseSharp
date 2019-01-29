using System;
using System.Runtime.InteropServices;

using Mono.Unix.Native;

namespace FuseSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class PathInfo
    {
        internal OpenFlags flags;
        private int write_page;
        private bool direct_io;
        private bool keep_cache;
        private ulong file_handle;

        internal PathInfo()
        {
        }

        public OpenFlags OpenFlags
        {
            get { return flags; }
            set { flags = value; }
        }

        public int WritePage
        {
            get { return write_page; }
            set { write_page = value; }
        }

        public bool DirectIO
        {
            get { return direct_io; }
            set { direct_io = value; }
        }

        public bool KeepCache
        {
            get { return keep_cache; }
            set { keep_cache = value; }
        }

        public IntPtr Handle
        {
            get { return (IntPtr)(long)file_handle; }
            set { file_handle = (ulong)(long)value; }
        }

        // TODO check this
        public static void CopyFromPtr(IntPtr source, PathInfo dest)
        {
            Interop.adaptor_ptrToPathInfo(source, dest);
            dest.flags = NativeConvert.ToOpenFlags((int)dest.flags);
        }

        public static void CopyToPtr(PathInfo source, IntPtr dest)
        {
            source.flags = (OpenFlags)NativeConvert.FromOpenFlags(source.flags);
            Interop.adaptor_pathInfoToPtr(source, dest);
        }


    }
}
