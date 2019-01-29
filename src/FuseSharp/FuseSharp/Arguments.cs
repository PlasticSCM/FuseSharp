using System;
using System.Runtime.InteropServices;

using Mono.Unix;

namespace FuseSharp
{
    public class Arguments
    {
        [StructLayout(LayoutKind.Sequential)]
        class Args
        {
            public int argc;
            public IntPtr argv;
            public int allocated;
        }

        public static IntPtr Allocate(string[] args)
        {
            IntPtr argv =
                UnixMarshal.AllocHeap((args.Length + 1) * IntPtr.Size);

            for (int i = 0; i < args.Length; ++i)
            {
                Marshal.WriteIntPtr(
                    argv,
                    i * IntPtr.Size,
                    UnixMarshal.StringToHeap(args[i]));
            }

            Marshal.WriteIntPtr(argv, args.Length * IntPtr.Size, IntPtr.Zero);
            return argv;
        }

        public static void Free(int argc, IntPtr argPtr)
        {
            if (argPtr == IntPtr.Zero)
                return;

            for (int i = 0; i < argc; ++i)
            {
                IntPtr p = Marshal.ReadIntPtr(argPtr, i * IntPtr.Size);
                UnixMarshal.FreeHeap(p);
            }

            UnixMarshal.FreeHeap(argPtr);
        }
    }
}
