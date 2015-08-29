using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static NetworkTablesCore.Native.Interop;

namespace NetworkTablesCore.Native
{
    public enum NT_Type
    {
        NT_UNASSIGNED = 0,
        NT_BOOLEAN = 0x01,
        NT_DOUBLE = 0x02,
        NT_STRING = 0x04,
        NT_RAW = 0x08,
        NT_BOOLEAN_ARRAY = 0x10,
        NT_DOUBLE_ARRAY = 0x20,
        NT_STRING_ARRAY = 0x40,
        NT_RPC = 0x80
    }

    public enum NT_EntryFlags
    {
        NT_PERSISTENT = 0x01
    }

    public enum NT_LogLevel
    {
        NT_LOG_CRITICAL = 50,
        NT_LOG_ERROR = 40,
        NT_LOG_WARNING = 30,
        NT_LOG_INFO = 20,
        NT_LOG_DEBUG = 10,
        NT_LOG_DEBUG1 = 9,
        NT_LOG_DEBUG2 = 8,
        NT_LOG_DEBUG3 = 7,
        NT_LOG_DEBUG4 = 6
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct NT_String_Read : IDisposable
    {
        private readonly byte* str;
        private readonly UIntPtr len;

        public override string ToString()
        {
            int iSize = (int)len.ToUInt64();
            byte[] arr = new byte[iSize];
            fixed (byte* b = arr)
            {
                byte* pb = b;
                byte* pbb = str;
                for (int i = 0; i < iSize; i++)
                {
                    *pb = *pbb;
                    pb++;
                    pbb++;
                }
            }
            //Marshal.Copy(str, arr, 0, (int)len.ToUInt64());
            return Encoding.UTF8.GetString(arr);
        }

        public void Dispose()
        {
            unsafe
            {
                fixed (NT_String_Read* ptr = &this)
                NT_DisposeString(ptr);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct NT_String_Write : IDisposable
    {
        private readonly byte* str;
        private readonly UIntPtr len;

        public NT_String_Write(string v_str)
        {
            int bytes = Encoding.UTF8.GetByteCount(v_str);
            str = (byte*)(Marshal.AllocHGlobal(bytes * sizeof (byte)).ToPointer());
            byte[] buffer = new byte[bytes];
            Encoding.UTF8.GetBytes(v_str, 0, v_str.Length, buffer, 0);
            fixed (byte* b = buffer)
            {
                byte* pb = b;
                byte* pbb = str;
                for (int i = 0; i < bytes; i++)
                {
                    *pbb = *pb;
                    pb++;
                    pbb++;
                }
            }
            len = (UIntPtr)bytes;
        }

        public override string ToString()
        {
            int iSize = (int)len.ToUInt64();
            byte[] arr = new byte[iSize];
            fixed (byte* b = arr)
            {
                byte* pb = b;
                byte* pbb = str;
                for (int i = 0; i < iSize; i++)
                {
                    *pb = *pbb;
                    pb++;
                    pbb++;
                }
            }
            return Encoding.UTF8.GetString(arr);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)str);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NT_EntryInfo
    {
        public NT_String_Read name;
        public NT_Type type;
        public uint flags;
        public ulong last_change;
    }


    //Looks like this will always be created for us by the library, so we do not have to write it.
    internal unsafe struct NT_ConnectionInfo
    {
        public NT_String_Read remote_id;
        public byte* remote_name;
        public uint remote_port;
        public ulong last_update;
        public uint protocol_version;

    }




}
