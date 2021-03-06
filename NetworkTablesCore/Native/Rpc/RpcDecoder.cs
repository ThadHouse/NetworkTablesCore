﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NetworkTables.Native.Rpc
{
    internal class RpcDecoder
    {
        public static double ReadDouble(byte[] buf, int start)
        {
            return BitConverter.Int64BitsToDouble(IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buf, start)));
        }

        private readonly byte[] m_buffer;
        private int m_count;

        public RpcDecoder(byte[] buffer)
        {
            m_buffer = buffer;
        }

        public RpcValue ReadValue(NtType type)
        {
            byte size = 0;
            byte[] buf;
            switch (type)
            {
                case NtType.Boolean:
                    byte vB = 0;
                    return !Read8(ref vB) ? null : RpcValue.MakeBoolean(vB != 0);
                case NtType.Double:
                    double vD = 0;
                    return !ReadDouble(ref vD) ? null : RpcValue.MakeDouble(vD);
                case NtType.Raw:
                    byte[] vRa = null;
                    return !ReadRaw(ref vRa) ? null : RpcValue.MakeRaw(vRa);
                case NtType.Rpc:
                case NtType.String:
                    string vS = "";
                    return !ReadString(ref vS) ? null : RpcValue.MakeString(vS);
                case NtType.BooleanArray:
                    if (!Read8(ref size)) return null;
                    buf = ReadArray(size);
                    if (buf == null) return null;
                    bool[] bBuf = new bool[buf.Length];
                    for (int i = 0; i < buf.Length; i++)
                    {
                        bBuf[i] = buf[i] != 0;
                    }
                    return RpcValue.MakeBooleanArray(bBuf);
                case NtType.DoubleArray:
                    if (!Read8(ref size)) return null;
                    buf = ReadArray(size * 8);
                    if (buf == null) return null;
                    double[] dBuf = new double[size];
                    for (int i = 0; i < size; i++)
                    {
                        dBuf[i] = ReadDouble(buf, m_count);
                    }
                    return RpcValue.MakeDoubleArray(dBuf);
                case NtType.StringArray:
                    if (!Read8(ref size)) return null;
                    buf = ReadArray(size * 8);
                    if (buf == null) return null;
                    string[] sBuf = new string[size];
                    for (int i = 0; i < size; i++)
                    {
                        if (!ReadString(ref sBuf[i])) return null;
                    }
                    return RpcValue.MakeStringArray(sBuf);
                default:
                    Console.WriteLine("invalid type when trying to read value");
                    return null;
            }
        }

        public byte[] ReadArray(int len)
        {
            List<byte> buf = new List<byte>(len);
            if (m_buffer.Length < m_count + len) return null;
            for (int i = m_count; i < m_count + len; i++)
            {
                buf.Add(m_buffer[i]);
            }
            return buf.ToArray();
        }

        public bool Read8(ref byte val)
        {
            if (m_buffer.Length < m_count + 1) return false;
            val = (byte)(m_buffer[m_count] & 0xff);
            m_count++;
            return true;
        }

        public bool Read16(ref short val)
        {
            if (m_buffer.Length < m_count + 2) return false;
            val = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(m_buffer, m_count));
            return true;
        }

        public bool Read32(ref int val)
        {
            if (m_buffer.Length < m_count + 4) return false;
            val = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(m_buffer, m_count));
            return true;
        }

        public bool ReadDouble(ref double val)
        {
            if (m_buffer.Length < m_count + 8) return false;
            val = ReadDouble(m_buffer, m_count);
            return true;
        }

        public bool ReadString(ref string val)
        {
            ulong v;
            if (Leb128.ReadUleb128(m_buffer, ref m_count, out v) == 0) return false;
            var len = (int)v;

            if (m_buffer.Length < m_count + len) return false;
            val = Encoding.UTF8.GetString(m_buffer, m_count, len);
            return true;
        }

        public bool ReadRaw(ref byte[] val)
        {
            ulong v;
            if (Leb128.ReadUleb128(m_buffer, ref m_count, out v) == 0) return false;
            var len = (int)v;

            if (m_buffer.Length < m_count + len) return false;
            val = new byte[len];
            Array.Copy(m_buffer, m_count, val, 0, len);
            return true;
        }
    }
}
