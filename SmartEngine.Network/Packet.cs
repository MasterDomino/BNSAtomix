using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SmartEngine.Network
{
    /// <summary>
    /// 会自动扩充缓存的封包类
    /// </summary>
    /// <typeparam name="T">Opcode的枚举</typeparam>
    [Serializable]
    public unsafe class Packet<T> : Stream
    {
        #region Members

        /// <summary>
        /// buffer
        /// </summary>
        internal byte[] data = new byte[32];

        internal int length;

        /// <summary>
        /// offset of packet
        /// </summary>
        protected ushort offset;

        private readonly Action<Packet<T>, Session<T>> onProcess;

        #endregion

        #region Instantiation

        /// <summary>
        /// Create new instance of a packet，and give it the initial size(to avoid automatic expansion
        /// of the re-allocation of memory).
        /// </summary>
        /// <param name="capacity">Initial size</param>
        public Packet(int capacity)
        {
            data = new byte[capacity];
            length = capacity;
        }

        /// <summary>
        /// Create a new instance of the packet, And give the initial size (to avoid automatic
        /// expansion of the re-allocation of memory), And specify a default packet handling process
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="onProcess">Packet processing</param>
        public Packet(int capacity, Action<Packet<T>, Session<T>> onProcess) : this(capacity)
        {
            this.onProcess = onProcess;
        }

        /// <summary>
        /// create new instance of the packet, and specify a default packet handling process
        /// </summary>
        /// <param name="onProcess"></param>
        public Packet(Action<Packet<T>, Session<T>> onProcess)
        {
            this.onProcess = onProcess;
        }

        /// <summary>
        /// Create an empty packet instance
        /// </summary>
        public Packet()
        {
            // do nothing
        }

        #endregion

        #region Properties

        /// <summary>
        /// buffer
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                return data;
            }
        }

        /// <summary>
        /// defines ability to read from packet
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// defines ability to seek from packet
        /// </summary>
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <summary>
        /// defines ability to write from packet
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Maximum capacity of the buffer
        /// </summary>
        public int Capacity { get { return data.Length; } }

        /// <summary>
        /// Whether the packet needs to be encrypted
        /// </summary>
        public bool Encrypt { get; set; } = true;

        /// <summary>
        /// Define if the packet has wrapper
        /// </summary>
        public bool HasWrapper { get; set; }

        /// <summary>
        /// Packet Opcode
        /// </summary>
        public virtual T ID
        {
            get
            {
                return (T)(object)(int)GetUShort(0);
            }
            set
            {
                //ushort back = offset;
                PutUShort((ushort)(int)(object)value, 0);

                //offset = back;
            }
        }

        /// <summary>
        /// Packet length
        /// </summary>
        public override long Length { get { return length; } }

        /// <summary>
        /// position within the packet
        /// </summary>
        public override long Position
        {
            get
            {
                return offset;
            }
            set
            {
                offset = (ushort)value;
            }
        }

        /// <summary>
        /// 用于发送此封包的Socket
        /// </summary>
        public Network<T> Sender { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// dumps raw bytes of packet
        /// </summary>
        /// <returns></returns>
        public string DumpData()
        {
            string tmp2 = string.Empty;
            for (int i = 0; i < length; i++)
            {
                tmp2 += ($"{data[i]:X2} ");
                if (((i + 1) % 16 == 0) && (i != 0))
                {
                    tmp2 += "\r\n";
                }
            }
            return tmp2;
        }

        /// <summary>
        /// flushes the handler
        /// </summary>
        public override void Flush()
        {
            // do nothing
        }

        /// <summary>
        /// Get a Byte at the specified offset
        /// </summary>
        /// <param name="index">Offset</param>
        /// <returns>byte result</returns>
        public byte GetByte(ushort index)
        {
            offset = (ushort)(index + 1);
            return data[index];
        }

        /// <summary>
        /// Get a Byte at the location
        /// </summary>
        /// <returns>byte result</returns>
        public byte GetByte()
        {
            return data[offset++];
        }

        /// <summary>
        /// Get a set of bytes from a given location.
        /// </summary>
        /// <param name="count">Number of bytes to get.</param>
        /// <param name="index">Indec from where to get bytes.</param>
        /// <returns>Byte array.</returns>
        public byte[] GetBytes(ushort count, ushort index)
        {
            offset = (ushort)(index + count);
            if (count == 0)
            {
                return new byte[0];
            }

            byte[] buf = new byte[count];
            fixed (byte* ptr = &data[index])
            {
                fixed (byte* ptr2 = buf)
                {
                    int* src = (int*)ptr, dst = (int*)ptr2;
                    int tmp = count / 4;
                    for (int i = 0; i < tmp; i++)
                    {
                        dst[i] = src[i];
                    }
                    switch (count % 4)
                    {
                        case 3:
                            *(short*)&ptr2[count - 3] = *(short*)&ptr[count - 3];
                            ptr2[count - 1] = ptr[count - 1];
                            break;

                        case 2:
                            *(short*)&ptr2[count - 2] = *(short*)&ptr[count - 2];
                            break;

                        case 1:
                            ptr2[count - 1] = ptr[count - 1];
                            break;
                    }
                }
            }
            return buf;
        }

        /// <summary>
        /// Get a certain amount of bytes from the current offset.
        /// </summary>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns>Byte array.</returns>
        public byte[] GetBytes(ushort count)
        {
            return GetBytes(count, offset);
        }

        /// <summary>
        /// gets the float value within the packet
        /// </summary>
        /// <param name="bigEndian"></param>
        /// <returns></returns>
        public float GetFloat(bool bigEndian)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(FloatToIntByBytes(GetFloat()));
            }
            else
            {
                return GetFloat();
            }
        }

        /// <summary>
        /// gets the float value within the packet on given index
        /// </summary>
        /// <param name="bigEndian"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetFloat(bool bigEndian, ushort index)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(FloatToIntByBytes(GetFloat(index)));
            }
            else
            {
                return GetFloat(index);
            }
        }

        /// <summary>
        /// Get the float at the given index.
        /// </summary>
        /// <param name="index">Index of the float.</param>
        /// <returns>The float value at the index.</returns>
        public float GetFloat(ushort index)
        {
            offset = (ushort)(index + 4);
            fixed (byte* ptr = &data[index])
            {
                return *(float*)ptr;
            }
        }

        /// <summary>
        /// Get the float at the current offset.
        /// </summary>
        /// <returns>The float value at the offset.</returns>
        public float GetFloat()
        {
            return GetFloat(offset);
        }

        /// <summary>
        /// Get the int at the given index.
        /// </summary>
        /// <param name="index">Index of the int.</param>
        /// <returns>The int value at the index.</returns>
        public int GetInt(ushort index)
        {
            offset = (ushort)(index + 4);
            fixed (byte* ptr = &data[index])
            {
                return *(int*)ptr;
            }
        }

        /// <summary>
        /// Get the int at the current offset.
        /// </summary>
        /// <returns>The int value at the offset.</returns>
        public int GetInt()
        {
            return GetInt(offset);
        }

        /// <summary>
        /// Get the int at the current offset using bigendian.
        /// </summary>
        /// <param name="bigEndian"></param>
        /// <returns></returns>
        public int GetInt(bool bigEndian)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(GetInt());
            }
            else
            {
                return GetInt();
            }
        }

        /// <summary>
        /// get the int at the given offset using bigendian.
        /// </summary>
        /// <param name="bigEndian"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetInt(bool bigEndian, ushort index)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(GetInt(index));
            }
            else
            {
                return GetInt(index);
            }
        }

        /// <summary>
        /// get long on the current offset.
        /// </summary>
        /// <returns>long value</returns>
        public long GetLong()
        {
            return GetLong(offset);
        }

        /// <summary>
        /// get long on the given offset.
        /// </summary>
        /// <param name="index">offset</param>
        /// <returns>long value</returns>
        public long GetLong(ushort index)
        {
            offset = (ushort)(index + 8);
            fixed (byte* ptr = &data[index])
            {
                return *(long*)ptr;
            }
        }

        /// <summary>
        /// get long on the current offset using bigendian.
        /// </summary>
        /// <param name="bigEndian">boolean used to determine of usage</param>
        /// <returns>long value</returns>
        public long GetLong(bool bigEndian)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(GetLong());
            }
            else
            {
                return GetLong();
            }
        }

        /// <summary>
        /// get long on the given offset using bigendian.
        /// </summary>
        /// <param name="bigEndian">boolean used to determine of usage</param>
        /// <param name="index">offset</param>
        /// <returns>long value</returns>
        public long GetLong(bool bigEndian, ushort index)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(GetLong(index));
            }
            else
            {
                return GetLong(index);
            }
        }

        /// <summary>
        /// Get the short at the given index.
        /// </summary>
        /// <param name="index">Index of the short.</param>
        /// <returns>The short value at the index.</returns>
        public short GetShort(ushort index)
        {
            offset = (ushort)(index + 2);
            fixed (byte* ptr = &data[index])
            {
                return *(short*)ptr;
            }
        }

        /// <summary>
        /// Get the short at the current offset.
        /// </summary>
        /// <returns>The short value at the offset.</returns>
        public short GetShort()
        {
            return GetShort(offset);
        }

        public short GetShort(bool bigEndian)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(GetShort());
            }
            else
            {
                return GetShort();
            }
        }

        public short GetShort(bool bigEndian, ushort index)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(GetShort(index));
            }
            else
            {
                return GetShort(index);
            }
        }

        /// <summary>
        /// 取得UNICODE字符串
        /// </summary>
        /// <param name="index">字符串所在偏移</param>
        /// <returns>字符串</returns>
        public string GetString(ushort index)
        {
            ushort len = GetUShort(index);
            offset = (ushort)(index + 2 + len);

            return Global.Encoding.GetString(data, index + 2, len);
        }

        /// <summary>
        /// 取得在某个指定偏移的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public string GetString()
        {
            return GetString(offset);
        }

        /// <summary>
        /// 取得UNICODE字符串
        /// </summary>
        /// <param name="bigEndian"></param>
        /// <param name="index">字符串所在偏移</param>
        /// <returns>字符串</returns>
        public string GetString(bool bigEndian, ushort index)
        {
            ushort len = GetUShort(bigEndian, index);
            offset = (ushort)(index + 2 + len);

            return Global.Encoding.GetString(data, index + 2, len);
        }

        /// <summary>
        /// 取得在某个指定偏移的字符串
        /// </summary>
        /// <param name="bigEndian"></param>
        /// <returns>字符串</returns>
        public string GetString(bool bigEndian)
        {
            return GetString(bigEndian, offset);
        }

        public K GetStruct<K>()
                    where K : struct
        {
            return GetStruct<K>(offset);
        }

        public K GetStruct<K>(ushort index)
                    where K : struct
        {
            offset = (ushort)(index + Marshal.SizeOf(typeof(K)));
            fixed (byte* ptr = &data[index])
            {
                return (K)Marshal.PtrToStructure(new IntPtr(ptr), typeof(K));
            }
        }

        /// <summary>
        /// 在某位开始读取指定位数的整数
        /// </summary>
        /// <param name="index">字节位移</param>
        /// <param name="offset">偏移位数</param>
        /// <param name="length">长度位数</param>
        /// <returns>结果</returns>
        public long GetSubBits(ushort index, int offset, int length)
        {
            int totalLen = offset + length;
            long res = 0;
            long val = 0;
            if (totalLen <= 8)
            {
                val = GetByte(index);
            }
            else if (totalLen <= 16)
            {
                val = GetShort(index);
            }
            else if (totalLen <= 32)
            {
                val = GetInt(index);
            }
            else if (totalLen <= 64)
            {
                val = GetLong(index);
            }
            else
            {
                throw new OverflowException();
            }

            res = (val >> offset) & ((1 << (length)) - 1);
            return res;
        }

        /// <summary>
        /// Get the uint at the given index.
        /// </summary>
        /// <param name="index">Index of the uint.</param>
        /// <returns>The uint value at the index.</returns>
        public uint GetUInt(ushort index)
        {
            offset = (ushort)(index + 4);
            fixed (byte* ptr = &data[index])
                return *(uint*)ptr;
        }

        /// <summary>
        /// Get the uint at the current offset.
        /// </summary>
        /// <returns>The uint value at the offset.</returns>
        public uint GetUInt()
        {
            return GetUInt(offset);
        }

        public uint GetUInt(bool bigEndian)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(GetUInt());
            }
            else
            {
                return GetUInt();
            }
        }

        public uint GetUInt(bool bigEndian, ushort index)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(GetUInt(index));
            }
            else
            {
                return GetUInt(index);
            }
        }

        public ulong GetULong()
        {
            return GetULong(offset);
        }

        public ulong GetULong(ushort index)
        {
            offset = (ushort)(index + 8);
            fixed (byte* ptr = &data[index])
                return *(ulong*)ptr;
        }

        public ulong GetULong(bool bigEndian)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(GetULong());
            }
            else
            {
                return GetULong();
            }
        }

        public ulong GetULong(bool bigEndian, ushort index)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(GetULong(index));
            }
            else
            {
                return GetULong(index);
            }
        }

        /// <summary>
        /// 在指定位置取得一个ushort
        /// </summary>
        /// <param name="index">偏移</param>
        /// <returns>The ushort value at the index.</returns>
        public ushort GetUShort(ushort index)
        {
            offset = (ushort)(index + 2);
            fixed (byte* ptr = &data[index])
            {
                return *(ushort*)ptr;
            }
        }

        /// <summary>
        /// Get the ushort at the current offset.
        /// </summary>
        /// <returns>The ushort value at the offset.</returns>
        public ushort GetUShort()
        {
            return GetUShort(offset);
        }

        public ushort GetUShort(bool bigEndian)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(GetUShort());
            }
            else
            {
                return GetUShort();
            }
        }

        public ushort GetUShort(bool bigEndian, ushort index)
        {
            if (bigEndian)
            {
                return Global.LittleToBigEndian(GetUShort(index));
            }
            else
            {
                return GetUShort(index);
            }
        }

        /// <summary>
        /// 返回一个封包实例
        /// </summary>
        /// <returns></returns>
        public virtual Packet<T> New()
        {
            return new Packet<T>(Capacity, onProcess);
        }

        /// <summary>
        /// 封包处理过程
        /// </summary>
        /// <param name="client"></param>
        public virtual void OnProcess(Session<T> client)
        {
            onProcess?.Invoke(this, client);
        }

        /// <summary>
        /// 在指定偏移处写入一个字节
        /// </summary>
        /// <param name="b">字节</param>
        /// <param name="index">偏移</param>
        public void PutByte(byte b, ushort index)
        {
            EnsureLength(index + 1);
            data[index] = b;
            offset = (ushort)(index + 1);
        }

        /// <summary>
        /// 在当前位置写入一个字节
        /// </summary>
        /// <param name="b">Byte to insert.</param>
        public void PutByte(byte b)
        {
            EnsureLength(offset + 1);
            data[offset++] = b;
        }

        /// <summary>
        /// Put some given bytes at a given position in the data array.
        /// </summary>
        /// <param name="bdata">bytes to add to the data array</param>
        /// <param name="index">position to add the bytes to</param>
        public void PutBytes(byte[] bdata, ushort index)
        {
            EnsureLength(index + bdata.Length);

            offset = (ushort)(index + bdata.Length);
            int count = bdata.Length;
            if (count == 0)
            {
                return;
            }

            fixed (byte* ptr = &data[index])
            {
                fixed (byte* ptr2 = bdata)
                {
                    int* src = (int*)ptr2, dst = (int*)ptr;
                    int tmp = count / 4;
                    for (int i = 0; i < tmp; i++)
                    {
                        dst[i] = src[i];
                    }
                    switch (count % 4)
                    {
                        case 3:
                            *(short*)&ptr[count - 3] = *(short*)&ptr2[count - 3];
                            ptr[count - 1] = ptr2[count - 1];
                            break;

                        case 2:
                            *(short*)&ptr[count - 2] = *(short*)&ptr2[count - 2];
                            break;

                        case 1:
                            ptr[count - 1] = ptr2[count - 1];
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Put some given bytes at the current offset in the data array.
        /// </summary>
        /// <param name="bdata">bytes to add to the data array</param>
        public void PutBytes(params byte[] bdata)
        {
            PutBytes(bdata, offset);
        }

        public void PutFloat(float s, bool bigEndian)
        {
            if (bigEndian)
            {
                PutInt(Global.LittleToBigEndian(FloatToIntByBytes(s)));
            }
            else
            {
                PutFloat(s);
            }
        }

        public void PutFloat(float s, ushort index, bool bigEndian)
        {
            if (bigEndian)
            {
                PutInt(Global.LittleToBigEndian(FloatToIntByBytes(s)), index);
            }
            else
            {
                PutFloat(s, index);
            }
        }

        /// <summary>
        /// Put the given float at the given index.
        /// </summary>
        /// <param name="s">Float to insert.</param>
        /// <param name="index">Index to insert at.</param>
        public void PutFloat(float s, ushort index)
        {
            EnsureLength(index + 4);
            fixed (byte* ptr = &data[index])
                *(float*)ptr = s;
            offset = (ushort)(index + 4);
        }

        /// <summary>
        /// Put the given float at the current offset in the data.
        /// </summary>
        /// <param name="s">Float to insert.</param>
        public void PutFloat(float s)
        {
            PutFloat(s, offset);
        }

        /// <summary>
        /// Put the given int at the given index.
        /// </summary>
        /// <param name="s">Int to insert.</param>
        /// <param name="index">Index to insert at.</param>
        public void PutInt(int s, ushort index)
        {
            EnsureLength(index + 4);
            fixed (byte* ptr = &data[index])
                *(int*)ptr = s;
            offset = (ushort)(index + 4);
        }

        public void PutInt(int s, bool bigEndian)
        {
            if (bigEndian)
            {
                PutInt(Global.LittleToBigEndian(s));
            }
            else
            {
                PutInt(s);
            }
        }

        public void PutInt(int s, ushort index, bool bigEndian)
        {
            if (bigEndian)
            {
                PutInt(Global.LittleToBigEndian(s), index);
            }
            else
            {
                PutInt(s, index);
            }
        }

        /// <summary>
        /// Put the given int at the current offset in the data.
        /// </summary>
        /// <param name="s">Int to insert.</param>
        public void PutInt(int s)
        {
            PutInt(s, offset);
        }

        public void PutLong(long s)
        {
            PutLong(s, offset);
        }

        public void PutLong(long s, ushort index)
        {
            EnsureLength(index + 8);
            fixed (byte* ptr = &data[index])
                *(long*)ptr = s;
            offset = (ushort)(index + 8);
        }

        public void PutLong(long s, bool bigEndian)
        {
            if (bigEndian)
            {
                PutLong(Global.LittleToBigEndian(s));
            }
            else
            {
                PutLong(s);
            }
        }

        public void PutLong(long s, ushort index, bool bigEndian)
        {
            if (bigEndian)
            {
                PutLong(Global.LittleToBigEndian(s), index);
            }
            else
            {
                PutLong(s, index);
            }
        }

        /// <summary>
        /// Put the given short at the given index.
        /// </summary>
        /// <param name="s">Short to insert.</param>
        /// <param name="index">Index to insert at.</param>
        public void PutShort(short s, ushort index)
        {
            EnsureLength(index + 2);
            fixed (byte* ptr = &data[index])
            {
                *(short*)ptr = s;
            }
            offset = (ushort)(index + 2);
        }

        /// <summary>
        /// Put the given short at the current offset.
        /// </summary>
        /// <param name="s">Short to insert.</param>
        public void PutShort(short s)
        {
            PutShort(s, offset);
        }

        public void PutShort(short s, bool bigEndian)
        {
            if (bigEndian)
            {
                PutShort(Global.LittleToBigEndian(s));
            }
            else
            {
                PutShort(s);
            }
        }

        public void PutShort(short s, ushort index, bool bigEndian)
        {
            if (bigEndian)
            {
                PutShort(Global.LittleToBigEndian(s), index);
            }
            else
            {
                PutShort(s, index);
            }
        }

        /// <summary>
        /// 将Unicode字符串写入指定偏移
        /// </summary>
        /// <param name="s">要写入的字符串.</param>
        /// <param name="index">偏移.</param>
        public void PutString(string s, ushort index)
        {
            byte[] buf = Global.Encoding.GetBytes(s);
            EnsureLength(index + buf.Length + 2);
            PutUShort((ushort)buf.Length, index);
            PutBytes(buf, (ushort)(index + 2));
            offset = (ushort)(index + buf.Length + 2);
        }

        /// <summary>
        /// 在当前偏移处写入字符串
        /// </summary>
        /// <param name="s">String to insert.</param>
        public void PutString(string s)
        {
            PutString(s, offset);
        }

        /// <summary>
        /// 将Unicode字符串写入指定偏移
        /// </summary>
        /// <param name="bigEndian"></param>
        /// <param name="s">要写入的字符串.</param>
        /// <param name="index">偏移.</param>
        public void PutString(bool bigEndian, string s, ushort index)
        {
            byte[] buf = Global.Encoding.GetBytes(s);
            EnsureLength(index + buf.Length + 2);
            PutUShort((ushort)buf.Length, index, bigEndian);
            PutBytes(buf, (ushort)(index + 2));
            offset = (ushort)(index + buf.Length + 2);
        }

        /// <summary>
        /// 在当前偏移处写入字符串
        /// </summary>
        /// <param name="bigEndian"></param>
        /// <param name="s">String to insert.</param>
        public void PutString(bool bigEndian, string s)
        {
            PutString(bigEndian, s, offset);
        }

        public void PutStruct<K>(K obj) where K : struct
        {
            PutStruct(obj, offset);
        }

        public void PutStruct<K>(K obj, ushort index) where K : struct
        {
            offset = (ushort)(index + Marshal.SizeOf(typeof(K)));
            fixed (byte* ptr = &data[index])
            {
                Marshal.StructureToPtr(obj, new IntPtr(ptr), false);
            }
        }

        /// <summary>
        /// 在某字节偏移的位偏移处开始写入某指定位数的整数
        /// </summary>
        /// <param name="value">需要写入的整数</param>
        /// <param name="index">字节偏移</param>
        /// <param name="offset">位偏移</param>
        /// <param name="length">位长度</param>
        public void PutSubBits(long value, ushort index, int offset, int length)
        {
            int totalLen = offset + length;
            long val = 0;
            if (totalLen <= 8)
            {
                val = GetByte(index);
                PutByte((byte)(((value & ((1 << (length)) - 1)) << offset) | val), index);
            }
            else if (totalLen <= 16)
            {
                val = GetShort(index);
                PutShort((short)(((value & ((1 << (length)) - 1)) << offset) | val), index);
            }
            else if (totalLen <= 32)
            {
                val = GetInt(index);
                PutInt((int)(((value & ((1 << (length)) - 1)) << offset) | val), index);
            }
            else if (totalLen <= 64)
            {
                val = GetLong(index);
                PutLong(((value & ((1 << (length)) - 1)) << offset) | val, index);
            }
            else
            {
                throw new OverflowException();
            }
        }

        /// <summary>
        /// Put the given uint at the given index.
        /// </summary>
        /// <param name="s">uint to insert.</param>
        /// <param name="index">Index to insert at.</param>
        public void PutUInt(uint s, ushort index)
        {
            EnsureLength(index + 4);
            fixed (byte* ptr = &data[index])
                *(uint*)ptr = s;
            offset = (ushort)(index + 4);
        }

        /// <summary>
        /// Put the given uint at the current offset.
        /// </summary>
        /// <param name="s">uint to insert</param>
        public void PutUInt(uint s)
        {
            PutUInt(s, offset);
        }

        public void PutUInt(uint s, bool bigEndian)
        {
            if (bigEndian)
            {
                PutUInt(Global.LittleToBigEndian(s));
            }
            else
            {
                PutUInt(s);
            }
        }

        public void PutUInt(uint s, ushort index, bool bigEndian)
        {
            if (bigEndian)
            {
                PutUInt(Global.LittleToBigEndian(s), index);
            }
            else
            {
                PutUInt(s, index);
            }
        }

        public void PutULong(ulong s)
        {
            PutULong(s, offset);
        }

        public void PutULong(ulong s, ushort index)
        {
            EnsureLength(index + 8);
            fixed (byte* ptr = &data[index])
                *(ulong*)ptr = s;
            offset = (ushort)(index + 8);
        }

        public void PutULong(ulong s, bool bigEndian)
        {
            if (bigEndian)
            {
                PutULong(Global.LittleToBigEndian(s));
            }
            else
            {
                PutULong(s);
            }
        }

        public void PutULong(ulong s, ushort index, bool bigEndian)
        {
            if (bigEndian)
            {
                PutULong(Global.LittleToBigEndian(s), index);
            }
            else
            {
                PutULong(s, index);
            }
        }

        /// <summary>
        /// Put the given ushort at the given index.
        /// </summary>
        /// <param name="s">Ushort to insert.</param>
        /// <param name="index">Index to insert at.</param>
        public void PutUShort(ushort s, ushort index)
        {
            EnsureLength(index + 2);
            fixed (byte* ptr = &data[index])
            {
                *(ushort*)ptr = s;
            }
            offset = (ushort)(index + 2);
        }

        /// <summary>
        /// Put the given ushort at the current offset.
        /// </summary>
        /// <param name="s"></param>
        public void PutUShort(ushort s)
        {
            PutUShort(s, offset);
        }

        public void PutUShort(ushort s, bool bigEndian)
        {
            if (bigEndian)
            {
                PutUShort(Global.LittleToBigEndian(s));
            }
            else
            {
                PutUShort(s);
            }
        }

        public void PutUShort(ushort s, ushort index, bool bigEndian)
        {
            if (bigEndian)
            {
                PutUShort(Global.LittleToBigEndian(s), index);
            }
            else
            {
                PutUShort(s, index);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > data.Length - this.offset)
            {
                count = data.Length - this.offset;
            }

            Array.Copy(data, this.offset, buffer, offset, count);
            this.offset += (ushort)count;
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.offset = (ushort)offset;
            return this.offset;
        }

        /// <summary>
        /// 将封包长度写入头4个字节
        /// </summary>
        public void SetLength()
        {
            uint tLen = (uint)(length - 4);
            EnsureLength(4);
            fixed (byte* ptr = data)
            {
                *(uint*)ptr = tLen;
            }
            offset = 4;
        }

        public override void SetLength(long value)
        {
            EnsureLength((int)value);
        }

        /// <summary>
        /// 将缓存区转换成Array
        /// </summary>
        public byte[] ToArray()
        {
            byte[] buf = new byte[length];
            Array.Copy(data, 0, buf, 0, length);
            return buf;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            EnsureLength(this.offset + count);
            Array.Copy(buffer, offset, this.data, this.offset, count);
            this.offset += (ushort)count;
        }

        /// <summary>
        /// 确保有足够的缓存区，不够则自动扩充
        /// </summary>
        /// <param name="len">长度</param>
        protected void EnsureLength(int len)
        {
            int capacity = data.Length;
            bool extend = false;
            while (capacity < len)
            {
                capacity += capacity;
                extend = true;
            }
            if (extend)
            {
                byte[] buf = new byte[capacity];
                data.CopyTo(buf, 0);
                data = buf;
            }
            if (length < len)
            {
                length = len;
            }
        }

        protected int FloatToIntByBytes(float f)
        {
            return *((int*)(byte*)&f);
        }

        #endregion
    }
}