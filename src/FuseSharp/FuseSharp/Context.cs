using System;
using System.Runtime.InteropServices;

namespace FuseSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Context
    {
        internal IntPtr fuse;
        private long userId;
        private long groupId;
        private int processId;

        internal Context() { }

        public long UserId
        {
            get { return userId; }
        }

        public long GroupId
        {
            get { return groupId; }
        }

        public int ProcessId
        {
            get { return processId; }
        }

        public override string ToString()
        {
            return $"Context UserId:{userId} GroupId:{groupId} ProcessId:{processId}";
        }
    }
}
