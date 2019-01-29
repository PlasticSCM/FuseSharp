using System;
using System.Runtime.InteropServices;

using Mono.Unix;

namespace FuseSharp
{
    public class FileNameMarshaler : ICustomMarshaler
    {
        static ICustomMarshaler GetInstance(String pstrCookie)
        {
            FileNameMarshaler marshaller = new FileNameMarshaler();
            return marshaller;
        }

        void ICustomMarshaler.CleanUpManagedData(object ManagedObj)
        {
        }

        void ICustomMarshaler.CleanUpNativeData(IntPtr pNativeData)
        {
            UnixMarshal.FreeHeap(pNativeData);
        }

        int ICustomMarshaler.GetNativeDataSize()
        {
            throw new NotImplementedException();
        }

        IntPtr ICustomMarshaler.MarshalManagedToNative(object ManagedObj)
        {
            return (!(ManagedObj is string))
                ? IntPtr.Zero
                : UnixMarshal.StringToHeap((string)ManagedObj, UnixEncoding.Instance);
        }

        object ICustomMarshaler.MarshalNativeToManaged(IntPtr pNativeData)
        {
            return UnixMarshal.PtrToString(pNativeData, UnixEncoding.Instance);
        }
    }
}
