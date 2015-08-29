using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NetworkTablesCore.Native.Exceptions;
using NetworkTablesCore.Tables;
using static NetworkTablesCore.Native.Interop;

namespace NetworkTablesCore.Native
{
    public unsafe static class CoreMethods
    {

        #region Setters
        public static bool SetEntryBoolean(string name, bool value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            int retVal = NT_SetEntryBoolean(namePtr, size, value ? 1 : 0, force ? 1 : 0);
            return retVal != 0;
        }

        public static bool SetEntryDouble(string name, double value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            int retVal = NT_SetEntryDouble(namePtr, size, value, force ? 1 : 0);
            return retVal != 0;
        }

        public static bool SetEntryString(string name, string value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize;
            byte[] stringPtr = CreateUTF8String(value, out stringSize);
            int retVal = NT_SetEntryString(namePtr, size, stringPtr, stringSize, force ? 1 : 0);
            //str.Dispose();
            return retVal != 0;
        }

        public static bool SetEntryRaw(string name, string value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize;
            byte[] stringPtr = CreateUTF8String(name, out stringSize);
            int retVal = NT_SetEntryRaw(namePtr, size, stringPtr, stringSize, force ? 1 : 0);
            return retVal != 0;
        }

        public static bool SetEntryBooleanArray(string name, bool[] value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);

            int[] valueIntArr = new int[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                valueIntArr[i] = value[i] ? 1 : 0;
            }

            int retVal = NT_SetEntryBooleanArray(namePtr, size, valueIntArr, (UIntPtr)valueIntArr.Length, force ? 1 : 0);

            return retVal != 0;
        }

        public static bool SetEntryDoubleArray(string name, double[] value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);

            int retVal = NT_SetEntryDoubleArray(namePtr, size, value, (UIntPtr)value.Length, force ? 1 : 0);

            return retVal != 0;
        }

        public static bool SetEntryStringArray(string name, string[] value, bool force = false)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);

            NT_String_Write[] ntStrings = new NT_String_Write[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                ntStrings[i] = new NT_String_Write(value[i]);
            }
            int retVal;
            fixed (NT_String_Write* ptr = ntStrings)
            {
                retVal = NT_SetEntryStringArray(namePtr, size, ptr, (UIntPtr)ntStrings.Length, force ? 1 : 0);
            }
            foreach (var ntString in ntStrings)
            {
                ntString.Dispose();
            }

            return retVal != 0;
        }

        #endregion

        #region DefaultGetters
        public static bool GetEntryBoolean(string name, bool defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            int boolean = 0;
            ulong lc = 0;
            int status = NT_GetEntryBoolean(namePtr, size, &lc, &boolean);
            if (status == 0)
            {
                return defaultValue;
            }
            return boolean != 0;
        }

        public static double GetEntryDouble(string name, double defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            double retVal = 0;
            ulong last_change = 0;
            int status = NT_GetEntryDouble(namePtr, size, &last_change, &retVal);
            if (status == 0)
            {
                return defaultValue;
            }
            return retVal;
        }

        public static string GetEntryString(string name, string defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize = UIntPtr.Zero;
            ulong last_change = 0;
            byte* ret = NT_GetEntryString(namePtr, size, &last_change, &stringSize);
            if (ret == null)
            {
                return defaultValue;
            }
            else
            {
                string str = ReadUTF8String(ret, stringSize);
                NT_FreeCharArray(ret);
                return str;
            }
        }

        public static string GetEntryRaw(string name, string defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize = UIntPtr.Zero;
            ulong last_change = 0;
            byte* ret = NT_GetEntryRaw(namePtr, size, &last_change, &stringSize);
            if (ret == null)
            {
                return defaultValue;
            }
            else
            {
                string str = ReadUTF8String(ret, stringSize);
                NT_FreeCharArray(ret);
                return str;
            }
        }

        public static double[] GetEntryDoubleArray(string name, double[] defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong last_change = 0;
            double* arrPtr = NT_GetEntryDoubleArray(namePtr, size, &last_change, &arrSize);
            if (arrPtr == null)
            {
                return defaultValue;
            }
            double[] arr = GetDoubleArrayFromPtr(arrPtr, arrSize);
            NT_FreeDoubleArray(arrPtr);
            return arr;
        }

        public static bool[] GetEntryBooleanArray(string name, bool[] defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong last_change = 0;
            int* arrPtr = NT_GetEntryBooleanArray(namePtr, size, &last_change, &arrSize);
            if (arrPtr == null)
            {
                return defaultValue;
            }
            bool[] arr = GetBooleanArrayFromPtr(arrPtr, arrSize);
            NT_FreeBooleanArray(arrPtr);
            return arr;
        }

        public static string[] GetEntryStringArray(string name, string[] defaultValue)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            IntPtr strLen = IntPtr.Zero;
            ulong last_change = 0;
            NT_String_Read* arrPtr = NT_GetEntryStringArray(namePtr, size, &last_change, &arrSize);
            if (arrPtr == null)
            {
                return defaultValue;
            }
            string[] arr = GetStringArrayFromPtr(arrPtr, arrSize);
            NT_FreeStringArray(arrPtr, arrSize);
            return arr;
        }
        #endregion

        #region Getters

        public static bool GetEntryBoolean(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            int boolean = 0;
            ulong lc = 0;
            int status = NT_GetEntryBoolean(namePtr, size, &lc, &boolean);
            if (status == 0)
            {
                throw new TableKeyNotDefinedException(name);//Change this to GetTableKeyNotDefined
            }
            return boolean != 0;
        }

        public static double GetEntryDouble(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            double retVal = 0;
            ulong last_change = 0;
            int status = NT_GetEntryDouble(namePtr, size, &last_change, &retVal);
            if (status == 0)
            {
                throw new TableKeyNotDefinedException(name);//Change this to GetTableKeyNotDefined
            }
            return retVal;
        }

        public static string GetEntryString(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize = UIntPtr.Zero;
            ulong last_change = 0;
            byte* ret = NT_GetEntryString(namePtr, size, &last_change, &stringSize);
            if (ret == null)
            {
                throw new TableKeyNotDefinedException(name);
            }
            else
            {
                string str = ReadUTF8String(ret, stringSize);
                NT_FreeCharArray(ret);
                return str;
            }
        }

        public static string GetEntryRaw(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr stringSize = UIntPtr.Zero;
            ulong last_change = 0;
            byte* ret = NT_GetEntryRaw(namePtr, size, &last_change, &stringSize);
            if (ret == null)
            {
                throw new TableKeyNotDefinedException(name);
            }
            else
            {
                string str = ReadUTF8String(ret, stringSize);
                NT_FreeCharArray(ret);
                return str;
            }
        }

        public static double[] GetEntryDoubleArray(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong last_change = 0;
            double* arrPtr = NT_GetEntryDoubleArray(namePtr, size, &last_change, &arrSize);
            if (arrPtr == null)
            {
                throw new TableKeyNotDefinedException(name);//TODO: Change this to not defined exception
            }
            double[] arr = GetDoubleArrayFromPtr(arrPtr, arrSize);
            NT_FreeDoubleArray(arrPtr);
            return arr;
        }

        public static bool[] GetEntryBooleanArray(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong last_change = 0;
            int* arrPtr = NT_GetEntryBooleanArray(namePtr, size, &last_change, &arrSize);
            if (arrPtr == null)
            {
                throw new TableKeyNotDefinedException(name);//TODO: Change this to not defined exception
            }
            bool[] arr = GetBooleanArrayFromPtr(arrPtr, arrSize);
            NT_FreeBooleanArray(arrPtr);
            return arr;
        }

        public static string[] GetEntryStringArray(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            ulong last_change = 0;
            NT_String_Read* arrPtr = NT_GetEntryStringArray(namePtr, size, &last_change, &arrSize);
            if (arrPtr == null)
            {
                throw new TableKeyNotDefinedException(name);//TODO: Change this to not defined exception
            }
            string[] arr = GetStringArrayFromPtr(arrPtr, arrSize);
            NT_FreeStringArray(arrPtr, arrSize);
            return arr;
        }
        #endregion

        #region EntryInfo

        public static EntryInfo[] GetEntryInfo(string prefix, int types)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(prefix, out size);
            UIntPtr arrSize = UIntPtr.Zero;
            NT_EntryInfo* arr = NT_GetEntryInfo(str, size, (uint)types, &arrSize);

            int entryInfoSize = Marshal.SizeOf(typeof(NT_EntryInfo));
            int arraySize = (int)arrSize.ToUInt64();
            EntryInfo[] entryArray = new EntryInfo[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                entryArray[i] = new EntryInfo(arr[i].name.ToString(), arr[i].type, (int)arr[i].flags, (long)arr[i].last_change);
                /*
                IntPtr data = new IntPtr(arr.ToInt64() + entryInfoSize * i);
                NT_EntryInfo info = (NT_EntryInfo)Marshal.PtrToStructure(data, typeof(NT_EntryInfo));
                entryArray[i] = new EntryInfo(info.name.ToString(), info.type, (int)info.flags, (long)info.last_change);
                */
            }
            NT_DisposeEntryInfoArray(arr, arrSize);
            return entryArray;
        }
        #endregion

        #region ConnectionInfo

        public static ConnectionInfo[] GetConnections()
        {
            UIntPtr count = UIntPtr.Zero;
            NT_ConnectionInfo* connections = NT_GetConnections(&count);

            //int connectionInfoSize = Marshal.SizeOf(typeof(NT_ConnectionInfo));
            int arraySize = (int)count.ToUInt64();

            ConnectionInfo[] connectionsArray = new ConnectionInfo[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                //IntPtr data = new IntPtr(connections.ToInt64() + connectionInfoSize * i);
                //var con = (NT_ConnectionInfo)Marshal.PtrToStructure(data, typeof(NT_ConnectionInfo));
                connectionsArray[i] = new ConnectionInfo(connections[i].remote_id.ToString(),
                    ReadUTF8String(connections[i].remote_name), (int)connections[i].remote_port,
                    (long)connections[i].last_update, (int)connections[i].protocol_version);
            }
            NT_DisposeConnectionInfoArray(connections, count);
            return connectionsArray;
        }
        #endregion

        #region EntryListeners
        private static readonly Dictionary<int, NT_EntryListenerCallback> s_entryCallbacks =
            new Dictionary<int, NT_EntryListenerCallback>();

        public static int AddEntryListener(string prefix, Delegates.EntryListenerFunction listener, bool immediateNotify)
        {
            NT_EntryListenerCallback modCallback = (uid, data, name, len, value, isNew) =>
            {
                NT_Type type = NT_GetValueType(value);
                object obj;
                ulong lastChange = 0;
                UIntPtr size = UIntPtr.Zero;
                void* ptr;
                switch (type)
                {
                    case NT_Type.NT_UNASSIGNED:
                        obj = null;
                        break;
                    case NT_Type.NT_BOOLEAN:
                        int boolean = 0;
                        NT_GetValueBoolean(value, &lastChange, &boolean);
                        obj = boolean != 0;
                        break;
                    case NT_Type.NT_DOUBLE:
                        double val = 0;
                        NT_GetValueDouble(value, &lastChange, &val);
                        obj = val;
                        break;
                    case NT_Type.NT_STRING:
                        ptr = NT_GetValueString(value, &lastChange, &size);
                        obj = ReadUTF8String((byte*)ptr, size);
                        break;
                    case NT_Type.NT_RAW:
                        ptr = NT_GetValueRaw(value, &lastChange, &size);
                        obj = ReadUTF8String((byte*)ptr, size);
                        break;
                    case NT_Type.NT_BOOLEAN_ARRAY:
                        ptr = NT_GetValueBooleanArray(value, &lastChange, &size);
                        obj = GetBooleanArrayFromPtr((int*)ptr, size);
                        break;
                    case NT_Type.NT_DOUBLE_ARRAY:
                        ptr = NT_GetValueDoubleArray(value, &lastChange, &size);
                        obj = GetDoubleArrayFromPtr((double*)ptr, size);
                        break;
                    case NT_Type.NT_STRING_ARRAY:
                        ptr = NT_GetValueStringArray(value, &lastChange, &size);
                        obj = GetStringArrayFromPtr((NT_String_Read*)ptr, size);
                        break;
                    case NT_Type.NT_RPC:
                        obj = null;
                        break;
                    default:
                        obj = null;
                        break;
                }
                string key = ReadUTF8String(name, len);
                listener((int)uid, key, obj, isNew != 0);
            };
            UIntPtr prefixSize;
            byte[] prefixStr = CreateUTF8String(prefix, out prefixSize);
            int retVal = (int)NT_AddEntryListener(prefixStr, prefixSize, null, modCallback, immediateNotify ? 1 : 0);
            s_entryCallbacks.Add(retVal, modCallback);
            return retVal;
        }

        public static void RemoveEntryListener(int uid)
        {
            NT_RemoveEntryListener((uint)uid);
            if (s_entryCallbacks.ContainsKey(uid))
            {
                s_entryCallbacks.Remove(uid);
            }
        }
        #endregion

        #region Connection Listeners
        private static readonly Dictionary<int, NT_ConnectionListenerCallback> s_connectionCallbacks =
            new Dictionary<int, NT_ConnectionListenerCallback>();

        public static int AddConnectionListener(Delegates.ConnectionListenerFunction callback, bool immediateNotify)
        {
            NT_ConnectionListenerCallback modCallback =
                (uint uid, void* data, int connected, NT_ConnectionInfo* conn) =>
                {
                    string remoteName = ReadUTF8String(conn->remote_name);
                    ConnectionInfo info = new ConnectionInfo(conn->remote_id.ToString(), remoteName, (int)conn->remote_port, (long)conn->last_update, (int)conn->protocol_version);
                    callback((int)uid, connected != 0, info);
                };

            int retVal = (int)NT_AddConnectionListener(null, modCallback, immediateNotify ? 1 : 0);
            s_connectionCallbacks.Add(retVal, modCallback);
            return retVal;
        }

        public static void RemoveConnectionListener(int uid)
        {
            NT_RemoveConnectionListener((uint)uid);
            if (s_connectionCallbacks.ContainsKey(uid))
            {
                s_connectionCallbacks.Remove(uid);
            }
        }
        #endregion

        #region Server and Client Methods

        public static void SetNetworkIdentity(string name)
        {
            UIntPtr size;
            byte[] namePtr = CreateUTF8String(name, out size);
            NT_SetNetworkIdentity(namePtr, size);
        }
        public static void StartClient(string serverName, uint port)
        {
            if (serverName == null)
            {
                throw new ArgumentNullException(nameof(serverName), "Server cannot be null");
            }
            UIntPtr size;
            byte[] serverNamePtr = CreateUTF8String(serverName, out size);
            NT_StartClient(serverNamePtr, port);
        }

        public static void StartServer(string fileName, string listenAddress, uint port)
        {
            UIntPtr size = UIntPtr.Zero;
            byte[] fileNamePtr = CreateUTF8String(fileName, out size);
            byte[] listenAddressPtr = CreateUTF8String(listenAddress, out size);
            NT_StartServer(fileNamePtr, listenAddressPtr, port);
        }

        public static void StopClient()
        {
            NT_StopClient();
        }

        public static void StopServer()
        {
            NT_StopServer();
        }

        public static void SetUpdateRate(double interval)
        {
            NT_SetUpdateRate(interval);
        }
        #endregion

        #region Persistent

        public static void SavePersistent(string filename)
        {
            UIntPtr size;
            byte[] name = CreateUTF8String(filename, out size);
            byte* err = NT_SavePersistent(name);
            if (err != null) throw new Exception();//TODO: Figure out this exception
        }

        public static string[] LoadPersistent(string filename)
        {
            UIntPtr size;
            byte[] name = CreateUTF8String(filename, out size);
            List<string> warns = new List<string>();
            byte* err = NT_LoadPersistent(name, (line, msg) =>
            {
                warns.Add($"{line.ToString()}: {ReadUTF8String(msg)}");
            });
            if (err != null) throw new Exception();//TODO: Figure out this exception
            return warns.ToArray();
        }
        #endregion

        #region Flags
        public static void SetEntryFlags(string name, int flags)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(name, out size);
            NT_SetEntryFlags(str, size, (uint)flags);
        }

        public static int GetEntryFlags(string name)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(name, out size);
            uint flags = NT_GetEntryFlags(str, size);
            return (int)flags;
        }
        #endregion

        #region Utility

        public static void DeleteEntry(string name)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(name, out size);
            NT_DeleteEntry(str, size);
        }

        public static void DeleteAllEntries()
        {
            NT_DeleteAllEntries();
        }

        public static void Flush()
        {
            NT_Flush();
        }

        public static NT_Type GetType(string name)
        {
            UIntPtr size;
            byte[] str = CreateUTF8String(name, out size);
            NT_Type retVal = NT_GetType(str, size);
            return retVal;
        }

        public static bool ContainsKey(string key)
        {
            return GetType(key) != NT_Type.NT_UNASSIGNED;
        }

        public static long Now()
        {
            return (long)NT_Now();
        }
        #endregion

        #region Logger

        private static NT_LogFunc nativeLog;

        public static void SetLogger(Delegates.LoggerFunction func, int minLevel)
        {
            nativeLog = (level, file, line, msg) =>
            {
                string message = ReadUTF8String(msg);
                string fileName = ReadUTF8String(file);

                func((int)level, fileName, (int)line, message);
            };

            NT_SetLogger(nativeLog, (uint)minLevel);
        }

        #endregion

        #region IntPtr to Array Conversions
        private static double[] GetDoubleArrayFromPtr(double* ptr, UIntPtr size)
        {
            int iSize = (int)size.ToUInt64();
            double[] arr = new double[iSize];
            fixed (double* b = arr)
            {
                double* pb = b;
                double* pbb = ptr;
                for (int i = 0; i < iSize; i++)
                {
                    *pb = *pbb;
                    pb++;
                    pbb++;
                }
            }
            return arr;
        }

        private static bool[] GetBooleanArrayFromPtr(int* ptr, UIntPtr size)
        {
            int iSize = (int)size.ToUInt64();
            bool[] arr = new bool[iSize];
            fixed (bool* b = arr)
            {
                bool* pb = b;
                int* pbb = ptr;
                for (int i = 0; i < iSize; i++)
                {
                    *pb = (*pbb) != 0;
                    pb++;
                    pbb++;
                }
            }

            /*

            Marshal.Copy(ptr, arr, 0, arr.Length);
            bool[] bArr = new bool[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                bArr[i] = arr[i] != 0;
            }
            */
            return arr;
        }

        private static string[] GetStringArrayFromPtr(NT_String_Read* ptr, UIntPtr size)
        {
            //int ntStringSize = Marshal.SizeOf(typeof(NT_String_Read));
            int arraySize = (int)size.ToUInt64();
            string[] strArray = new string[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                strArray[i] = ptr[i].ToString();


                //IntPtr data = new IntPtr(ptr.ToInt64() + ntStringSize * i);
                //strArray[i] = Marshal.PtrToStructure(data, typeof(NT_String_Read)).ToString();
            }
            return strArray;
        }
        #endregion

        #region IntPtrs To String Conversions
        //Must be null terminated
        public static byte[] ReadUTF8StringToByteArray(IntPtr str, UIntPtr size)
        {
            int iSize = (int)size.ToUInt64();
            byte[] data = new byte[iSize];
            Marshal.Copy(str, data, 0, iSize);
            return data;
        }

        public static byte[] CreateUTF8String(string str, out UIntPtr size)
        {
            var bytes = Encoding.UTF8.GetByteCount(str);

            var buffer = new byte[bytes + 1];
            size = (UIntPtr)bytes;
            Encoding.UTF8.GetBytes(str, 0, str.Length, buffer, 0);
            buffer[bytes] = 0;
            return buffer;
        }

        //Must be null terminated
        public static string ReadUTF8String(byte* str, UIntPtr size)
        {
            int iSize = (int)size.ToUInt64();
            byte[] data = new byte[iSize];
            fixed (byte* b = data)
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
            //Marshal.Copy(str, data, 0, iSize);
            return Encoding.UTF8.GetString(data);
        }

        public static string ReadUTF8String(byte* ptr)
        {
            var data = new List<byte>();
            var off = 0;
            while (true)
            {
                var ch = *ptr;//Marshal.ReadByte(ptr, off++);
                ptr++;
                if (ch == 0)
                {
                    break;
                }
                data.Add(ch);
            }
            return Encoding.UTF8.GetString(data.ToArray());
        }
        #endregion

    }
}
