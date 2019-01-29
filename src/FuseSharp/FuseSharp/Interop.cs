using System;
using System.Runtime.InteropServices;

using Mono.Unix.Native;

namespace FuseSharp
{
    public class Interop
    {
        private const String fuse_libname = "osxfuse.2";
        private const String adaptor_libname = "Adaptor";

        private const CallingConvention callingConvention = CallingConvention.Cdecl;

        [DllImport(adaptor_libname, CallingConvention = callingConvention, SetLastError = true)]
        public static extern int adaptor_fuse_main(int argc, IntPtr argv, IntPtr op);

        [DllImport(fuse_libname, CallingConvention = callingConvention)]
        public static extern IntPtr fuse_get_context();

        [DllImport(fuse_libname, CallingConvention = callingConvention)]
        public static extern void fuse_exit(IntPtr fusep);

        [DllImport(adaptor_libname, CallingConvention = callingConvention)]
        public static extern int adaptor_invoke_filler(IntPtr filler, IntPtr buf, string name, IntPtr stbuf, long offset);

        [DllImport(adaptor_libname, CallingConvention = callingConvention, SetLastError = true)]
        public static extern int adaptor_pathInfoToPtr(PathInfo source, IntPtr dest);

        [DllImport(adaptor_libname, CallingConvention = callingConvention, SetLastError = true)]
        public static extern int adaptor_ptrToPathInfo(IntPtr source, [Out] PathInfo dest);

        public static void CopyStat(IntPtr source, out Stat dest)
        {
            if (!NativeConvert.TryCopy(source, out dest))
            {
                throw new ArgumentOutOfRangeException(
                    "Unable to copy `struct stat' into Mono.Unix.Native.Stat.");
            }
        }

        public static void CopyStat(ref Stat source, IntPtr dest)
        {
            if (!NativeConvert.TryCopy(ref source, dest))
            {
                throw new ArgumentOutOfRangeException(
                    "Unable to copy Mono.Unix.Native.Stat into `struct stat'.");
            }
        }

        public static void CopyUtimbuf(IntPtr source, out Utimbuf dest)
        {
            if (!NativeConvert.TryCopy(source, out dest))
            {
                throw new ArgumentOutOfRangeException(
                    "Unable to copy `struct utimbuf' into Mono.Unix.Native.Utimbuf.");
            }
        }

        public static void CopyUtimbuf(ref Utimbuf source, IntPtr dest)
        {
            if (!NativeConvert.TryCopy(ref source, dest))
            {
                throw new ArgumentOutOfRangeException(
                    "Unable to copy Mono.Unix.Native.Utimbuf into `struct utimbuf'.");
            }
        }

        public static void CopyStatvfs(IntPtr source, out Statvfs dest)
        {
            if (!NativeConvert.TryCopy(source, out dest))
            {
                throw new ArgumentOutOfRangeException(
                    "Unable to copy `struct statvfs' into Mono.Unix.Native.Statvfs.");
            }
        }

        public static void CopyStatvfs(ref Statvfs source, IntPtr dest)
        {
            if (!NativeConvert.TryCopy(ref source, dest))
            {
                throw new ArgumentOutOfRangeException(
                    "Unable to copy Mono.Unix.Native.Statvfs into `struct statvfs'.");
            }
        }

        public static void CopyFlock(IntPtr source, out Flock dest)
        {
            if (!NativeConvert.TryCopy(source, out dest))
            {
                throw new ArgumentOutOfRangeException(
                    "Unable to copy `struct flock' into Mono.Unix.Native.Flock.");
            }
        }

        public static void CopyFlock(ref Flock source, IntPtr dest)
        {
            if (!NativeConvert.TryCopy(ref source, dest))
            {
                throw new ArgumentOutOfRangeException(
                    "Unable to copy Mono.Unix.Native.Flock into `struct flock'.");
            }
        }

        public static int ConvertErrno(Errno e)
        {
            int r;
            if (NativeConvert.TryFromErrno(e, out r))
                return -r;
            return -1;
        }

    }
}
