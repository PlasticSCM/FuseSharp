using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FuseSharp
{
    public class Process
    {
        public static int Start(IntPtr argsPtr, int argsCount, IntPtr opPtr, int opSize)
        {
            try
            {
                return Interop.adaptor_fuse_main(argsCount, argsPtr, opPtr);
            }
            catch (NullReferenceException e)
            {
                Trace.WriteLine($"Process.Start {e.ToString()}");
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Process.Start {e.ToString()}");
            }
            return -1;
        }

        public static void Stop()
        {
            Interop.fuse_exit(GetContext().fuse);
        }

        public static Context GetContext()
        {
            IntPtr contextp = Interop.fuse_get_context();

            Context context = new Context();
            Marshal.PtrToStructure(contextp, context);

            return context;
        }

    }
}


