using System;
using System.Diagnostics;
using System.IO;

namespace FuseSharp.Examples
{
    class Program
    {
        unsafe static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine(
                    "Usage: FuseSharp.Example.exe <mirror|encrypted> <target root> <mount point>{0}" +
                    "       mirror : Mirror FileSystem. Mirrors 'target root' into 'mount point'.{0}" +
                    "    encrypted : Encrypted FileSystem. Mirrors 'target root' into 'mount point',{0}" +
                    "                encrypting and decrypting the content on the fly.{0}",
                    Environment.NewLine);

                return -1;
            }

            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            string targetRoot, mountPoint;
            if (!Directory.Exists(targetRoot = Path.GetFullPath(args[1])))
                Directory.CreateDirectory(targetRoot);

            if (!Directory.Exists(mountPoint = Path.GetFullPath(args[2])))
                Directory.CreateDirectory(mountPoint);

            Console.WriteLine(
                "Fs: {1}{0}Target root: {2}{0}Mount point:{3}{0}",
                Environment.NewLine, args[0], targetRoot, mountPoint);

            string[] actualArgs = { "-s", "-f", mountPoint };

            // string[] actualArgs =
            // {
            //     "/Users/gtr22/fusetemp/MyFilesystem",
            //     "-f",
            //     "-obig_writes",
            //     "-omax_write=8192",
            //     "-oiosize=4096",
            //     path
            // };

            // string[] actualArgs =
            // {
            //     "/Users/gtr22/fusetemp/MyFilesystem",
            //     "-obig_writes",
            //     "-s",
            //     "-f",
            //     "-d",
            //     path
            // };

            // string[] actualArgs =
            // {
            //     "-d",
            //     path
            // };

            int status = -1;

            using (FileSystem fs = GetFileSystem(args[0], targetRoot))
            using (FileSystemHandler fsh = new FileSystemHandler(fs, actualArgs))
            {
                status = fsh.Start();
            }

            Console.WriteLine(status);
            return status;
        }

        static FileSystem GetFileSystem(string id, string mountPoint)
        {
            switch (id.ToLowerInvariant())
            {
                case "mirror":
                    return new MirrorFileSystem(mountPoint);

                case "encrypted":
                    return new CodiceFileSystem(mountPoint);
            }

            throw new ArgumentException("Invalid FileSystem id", "id");
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine((e.ExceptionObject as Exception).Message);
            Debug.WriteLine((e.ExceptionObject as Exception).Message);
        }
    }
}
