using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NetworkTablesCore.Native
{
    [SuppressUnmanagedCodeSecurity]
    internal unsafe class Interop
    {
        internal const string NTSharedFile = "ntcore";

        //Callback Typedefs
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void NT_EntryListenerCallback(
            uint uid, void* data, byte* name, UIntPtr name_len, void* value, int is_new);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void NT_ConnectionListenerCallback(
            uint uid, void* data, int connected, NT_ConnectionInfo* conn);

        //Table Functions
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_SetEntryFlags(byte[] name, UIntPtr name_len, uint flags);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint NT_GetEntryFlags(byte[] name, UIntPtr name_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_DeleteEntry(byte[] name, UIntPtr name_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_DeleteAllEntries();
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern NT_EntryInfo* NT_GetEntryInfo(byte[] prefix, UIntPtr prefix_len, uint types, UIntPtr* count);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_Flush();

        //Callback Functions
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint NT_AddEntryListener(byte[] prefix, UIntPtr prefix_len, void* data,
            NT_EntryListenerCallback callback, int immediate_notify);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_RemoveEntryListener(uint entry_listener_uid);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint NT_AddConnectionListener(void* data, NT_ConnectionListenerCallback callback, int immediate_notify);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_RemoveConnectionListener(uint conn_listener_uid);

        //Ignoring RPC for now


        //Client/Server Functions
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_SetNetworkIdentity(byte[] name, UIntPtr name_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_StartServer(byte[] persist_filename, byte[] listen_address, uint port);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_StopServer();
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_StartClient(byte[] server_name, uint port);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_StopClient();
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_SetUpdateRate(double interval);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern NT_ConnectionInfo* NT_GetConnections(UIntPtr* count);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WarmFunction(UIntPtr line, byte* msg);

        //Persistent Functions
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* NT_SavePersistent(byte[] filename);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* NT_LoadPersistent(byte[] filename, WarmFunction warn);

        //Utility Functions
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_DisposeValue(void* value);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_DisposeString(NT_String_Read* str);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern NT_Type NT_GetType(byte[] name, UIntPtr name_len);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_DisposeConnectionInfoArray(NT_ConnectionInfo* arr, UIntPtr count);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_DisposeEntryInfoArray(NT_EntryInfo* arr, UIntPtr count);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong NT_Now();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void NT_LogFunc(uint level, byte* file, uint line, byte* msg);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_SetLogger(NT_LogFunc funct, uint min_level);

        //Interop Utility Functions
        //[DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        //public static extern NT_String NT_AllocateNTString(UIntPtr size);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* NT_AllocateCharArray(UIntPtr size);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_FreeBooleanArray(int* arr);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_FreeDoubleArray(double* arr);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_FreeCharArray(byte* arr);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_FreeStringArray(NT_String_Read* arr, UIntPtr arr_size);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern NT_Type NT_GetValueType(void* value);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_GetValueBoolean(void* value, ulong* last_change, int* v_boolean);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_GetValueDouble(void* value, ulong* last_change, double* v_double);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* NT_GetValueString(void* value, ulong* last_change, UIntPtr* string_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* NT_GetValueRaw(void* value, ulong* last_change, UIntPtr* raw_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int* NT_GetValueBooleanArray(void* value, ulong* last_change, UIntPtr* size);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern double* NT_GetValueDoubleArray(void* value, ulong* last_change, UIntPtr* size);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern NT_String_Read* NT_GetValueStringArray(void* value, ulong* last_change, UIntPtr* size);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]

        public static extern int NT_GetEntryBoolean(byte[] name, UIntPtr name_len, ulong* last_change, int* v_boolean);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_GetEntryDouble(byte[] name, UIntPtr name_len, ulong* last_change, double* v_double);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* NT_GetEntryString(byte[] name, UIntPtr name_len, ulong* last_change, UIntPtr* string_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte* NT_GetEntryRaw(byte[] name, UIntPtr name_len, ulong* last_change, UIntPtr* raw_len);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int* NT_GetEntryBooleanArray(byte[] name, UIntPtr name_len, ulong* last_change, UIntPtr* size);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern double* NT_GetEntryDoubleArray(byte[] name, UIntPtr name_len, ulong* last_change, UIntPtr* size);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern NT_String_Read* NT_GetEntryStringArray(byte[] name, UIntPtr name_len, ulong* last_change, UIntPtr* size);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryBoolean(byte[] name, UIntPtr name_len, int v_boolean, int force);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryDouble(byte[] name, UIntPtr name_len, double v_double, int force);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryString(byte[] name, UIntPtr name_len, byte[] v_string, UIntPtr string_len, int force);
        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryRaw(byte[] name, UIntPtr name_len, byte[] raw, UIntPtr raw_len, int force);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryBooleanArray(byte[] name, UIntPtr name_len, int[] arr, UIntPtr size,
            int force);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryDoubleArray(byte[] name, UIntPtr name_len, double[] arr, UIntPtr size,
            int force);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_SetEntryStringArray(byte[] name, UIntPtr name_len, NT_String_Write* arr, UIntPtr size,
            int force);



        //RPC

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr NT_RPCCallback(
            IntPtr data, IntPtr name, UIntPtr name_len, IntPtr param, UIntPtr params_len, ref UIntPtr results_len);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_CreateRpc(byte[] name, UIntPtr name_len, byte[] def, UIntPtr def_len, IntPtr data,
            NT_RPCCallback callback);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_CreatePolledRpc(byte[] name, UIntPtr name_len, byte[] def, UIntPtr def_len);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NT_PollRpc(int blocking, ref NT_RpcCallInfo call_info);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NT_PostRpcResponse(uint rpc_id, uint call_uid, byte[] result, UIntPtr result_len);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint NT_CallRpc(byte[] name, UIntPtr name_len, byte[] param, UIntPtr params_len);

        [DllImport(NTSharedFile, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr NT_GetRpcResult(int blocking, uint call_uid, ref UIntPtr result_len);

    }
}
