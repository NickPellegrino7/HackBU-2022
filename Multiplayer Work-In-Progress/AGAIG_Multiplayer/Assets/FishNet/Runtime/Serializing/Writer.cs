using FishNet.Connection;
using FishNet.Documenting;
using FishNet.Managing;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Serializing.Helping;
using FishNet.Transporting;
using FishNet.Utility.Constant;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

[assembly: InternalsVisibleTo(UtilityConstants.GENERATED_ASSEMBLY_NAME)]
namespace FishNet.Serializing
{
    /// <summary>
    /// Used for write references to generic types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [APIExclude]
    public static class GenericWriter<T>
    {
        public static Action<Writer, T> Write { get; set; }
        public static Action<Writer, T, AutoPackType> WriteAutoPack { get; set; }
    }

    /// <summary>
    /// Writes data to a buffer.
    /// </summary>
    public class Writer
    {
        #region Public.
        /// <summary>
        /// Current write position.
        /// </summary>
        public int Position = 0;
        /// <summary>
        /// Number of bytes writen to the buffer.
        /// </summary>
        public int Length { get; private set; }
        #endregion

        #region Private.
        /// <summary>
        /// Buffer to prevent new allocations. This will grow as needed.
        /// </summary>
        private byte[] _buffer = new byte[64];
        /// <summary>
        /// Encoder for strings.
        /// </summary>
        private readonly UTF8Encoding _encoding = new UTF8Encoding(false, true);
        /// <summary>
        /// StringBuffer to use with encoding.
        /// </summary>
        private byte[] _stringBuffer = new byte[64];
        #endregion


        /// <summary>
        /// Resets the writer as though it was unused. Does not reset buffers.
        /// </summary>
        public void Reset()
        {
            Length = 0;
            Position = 0;
        }

        /// <summary>
        /// Writes a dictionary.
        /// </summary>
        public void WriteDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            if (dict == null)
            {
                WriteBoolean(true);
                return;
            }
            else
            { 
                WriteBoolean(false);
            }

            WriteInt32(dict.Count);
            foreach (KeyValuePair<TKey, TValue> item in dict)
            {
                Write(item.Key);
                Write(item.Value);
            }
        }

        /// <summary>
        /// Resizes the buffer to double it's size as well additionalValue.
        /// </summary>
        private void DoubleBuffer(int additionalValue)
        {
            int nextSize = (_buffer.Length * 2) + additionalValue;
            Array.Resize(ref _buffer, nextSize);
        }

        /// <summary>
        /// Returns the buffer. The returned value will be the full buffer, even if not all of it is used.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBuffer()
        {
            return _buffer;
        }

        /// <summary>
        /// Returns the used portion of the buffer as an ArraySegment.
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> GetArraySegment()
        {
            return new ArraySegment<byte>(_buffer, 0, Length);
        }

        /// <summary>
        /// Reserves a number of bytes from current position.
        /// </summary>
        /// <param name="count"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reserve(int count)
        {
            if (Position + count > _buffer.Length)
                DoubleBuffer(count);

            Position += count;
            Length = Math.Max(Length, Position);
        }

        /// <summary>
        /// Sends a packetId.
        /// </summary>
        /// <param name="pid"></param>
        [CodegenExclude]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WritePacketId(PacketId pid)
        {
            WriteUInt16((ushort)pid);
        }

        /// <summary>
        /// Inserts value at index within the buffer.
        /// This method does not perform sanity checks.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        [CodegenExclude]
        public void FastInsertByte(byte value, int index)
        {
            _buffer[index] = value;
        }

        /// <summary>
        /// Writes a byte.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte value)
        {
            if (Position + 1 > _buffer.Length)
                DoubleBuffer(1);

            _buffer[Position++] = value;

            Length = Math.Max(Length, Position);
        }

        /// <summary>
        /// Writes bytes.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        [CodegenExclude]
        public void WriteBytes(byte[] buffer, int offset, int count)
        {
            if (Position + count > _buffer.Length)
                DoubleBuffer(count);

            Buffer.BlockCopy(buffer, offset, _buffer, Position, count);
            Position += count;
            Length = Math.Max(Length, Position);
        }

        /// <summary>
        /// Writes bytes and length of bytes.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodegenExclude]
        public void WriteBytesAndSize(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                WriteInt32(-1);
            }
            else
            {
                WriteInt32(count);
                WriteBytes(buffer, offset, count);
            }
        }

        /// <summary>
        /// Writes all bytes in value and length of bytes.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBytesAndSize(byte[] value)
        {
            int size = (value == null) ? 0 : value.Length;
            // buffer might be null, so we can't use .Length in that case
            WriteBytesAndSize(value, 0, size);
        }


        /// <summary>
        /// Writes a sbyte.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSByte(sbyte value)
        {
            if (Position + 1 > _buffer.Length)
                DoubleBuffer(1);

            _buffer[Position++] = (byte)value;
            Length = Math.Max(Length, Position);
        }

        /// <summary>
        /// Writes a char.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteChar(char value)
        {
            if (Position + 2 > _buffer.Length)
                DoubleBuffer(2);

            _buffer[Position++] = (byte)value;
            _buffer[Position++] = (byte)(value >> 8);
            Length = Math.Max(Length, Position);
        }

        /// <summary>
        /// Writes a boolean.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBoolean(bool value)
        {
            if (Position + 1 > _buffer.Length)
                DoubleBuffer(1);

            _buffer[Position++] = (value) ? (byte)1 : (byte)0;
            Length = Math.Max(Length, Position);
        }

        /// <summary>
        /// Writes a uint16.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16(ushort value)
        {
            if (Position + 2 > _buffer.Length)
                DoubleBuffer(2);

            _buffer[Position++] = (byte)value;
            _buffer[Position++] = (byte)(value >> 8);
            Length = Math.Max(Length, Position);
        }

        /// <summary>
        /// Writes a int16.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt16(short value)
        {
            if (Position + 2 > _buffer.Length)
                DoubleBuffer(2);

            _buffer[Position++] = (byte)value;
            _buffer[Position++] = (byte)(value >> 8);
            Length = Math.Max(Length, Position);
        }

        /// <summary>
        /// Writes a int32.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32(int value, AutoPackType packType = AutoPackType.Packed) => WriteUInt32((uint)value, packType);
        /// <summary>
        /// Writes a uint32.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt32(uint value, AutoPackType packType = AutoPackType.Packed)
        {
            if (packType == AutoPackType.Unpacked)
            {
                if (Position + 4 > _buffer.Length)
                    DoubleBuffer(4);

                WriterExtensions.WriteUInt32(_buffer, value, ref Position);
                Length = Math.Max(Length, Position);
            }
            else
            {
                WritePackedWhole(value);
            }
        }

        /// <summary>
        /// Writes an int64.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64(long value, AutoPackType packType = AutoPackType.Packed) => WriteUInt64((ulong)value, packType);
        /// <summary>
        /// Writes a uint64.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt64(ulong value, AutoPackType packType = AutoPackType.Packed)
        {
            if (packType == AutoPackType.Unpacked)
            {
                if (Position + 8 > _buffer.Length)
                    DoubleBuffer(8);

                _buffer[Position++] = (byte)value;
                _buffer[Position++] = (byte)(value >> 8);
                _buffer[Position++] = (byte)(value >> 16);
                _buffer[Position++] = (byte)(value >> 24);
                _buffer[Position++] = (byte)(value >> 32);
                _buffer[Position++] = (byte)(value >> 40);
                _buffer[Position++] = (byte)(value >> 48);
                _buffer[Position++] = (byte)(value >> 56);

                Length = Math.Max(Position, Length);
            }
            else
            {
                WritePackedWhole(value);
            }
        }

        /// <summary>
        /// Writes a single (float).
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSingle(float value, AutoPackType packType = AutoPackType.Unpacked)
        {
            if (packType == AutoPackType.Unpacked)
            {
                UIntFloat converter = new UIntFloat { FloatValue = value };
                WriteUInt32(converter.UIntValue, AutoPackType.Unpacked);
            }
            else
            {
                long converter = (long)(value * 100f);
                WritePackedWhole((ulong)converter);
            }
        }

        /// <summary>
        /// Writes a double.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDouble(double value)
        {
            UIntDouble converter = new UIntDouble { DoubleValue = value };
            WriteUInt64(converter.LongValue, AutoPackType.Unpacked);
        }

        /// <summary>
        /// Writes a decimal.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDecimal(decimal value)
        {
            UIntDecimal converter = new UIntDecimal { DecimalValue = value };
            WriteUInt64(converter.LongValue1, AutoPackType.Unpacked);
            WriteUInt64(converter.LongValue2, AutoPackType.Unpacked);
        }

        /// <summary>
        /// Writes a string.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(string value)
        {
            if (value == null)
            {
                WriteInt32(-1);
                return;
            }
            else if (value.Length == 0)
            {
                WriteInt32(0);
                return;
            }

            /* Resize string buffer as needed. There's no harm in
             * increasing buffer on writer side because sender will
             * never intentionally inflict allocations on itself. 
             * Reader ensures string count cannot exceed received
             * packet size. */
            if (value.Length >= _stringBuffer.Length)
            {
                int nextSize = (_stringBuffer.Length * 2) + value.Length;
                Array.Resize(ref _stringBuffer, nextSize);
            }

            int size = _encoding.GetBytes(value, 0, value.Length, _stringBuffer, 0);
            WriteInt32(size);
            WriteBytes(_stringBuffer, 0, size);
        }

        /// <summary>
        /// Writes a byte ArraySegment and it's size.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArraySegmentAndSize(ArraySegment<byte> value)
        {
            WriteBytesAndSize(value.Array, value.Offset, value.Count);
        }

        /// <summary>
        /// Writes an ArraySegment without size.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodegenExclude]
        public void WriteArraySegment(ArraySegment<byte> value)
        {
            WriteBytes(value.Array, value.Offset, value.Count);
        }

        /// <summary>
        /// Writes a Vector2.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVector2(Vector2 value)
        {
            UIntFloat converter;
            converter = new UIntFloat { FloatValue = value.x };
            WriteUInt32(converter.UIntValue, AutoPackType.Unpacked);
            converter = new UIntFloat { FloatValue = value.y };
            WriteUInt32(converter.UIntValue, AutoPackType.Unpacked);
        }

        /// <summary>
        /// Writes a Vector3
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVector3(Vector3 value)
        {
            UIntFloat converter;
            converter = new UIntFloat { FloatValue = value.x };
            WriteUInt32(converter.UIntValue, AutoPackType.Unpacked);
            converter = new UIntFloat { FloatValue = value.y };
            WriteUInt32(converter.UIntValue, AutoPackType.Unpacked);
            converter = new UIntFloat { FloatValue = value.z };
            WriteUInt32(converter.UIntValue, AutoPackType.Unpacked);
        }

        /// <summary>
        /// Writes a Vector4.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVector4(Vector4 value)
        {
            UIntFloat converter;
            converter = new UIntFloat { FloatValue = value.x };
            WriteUInt32(converter.UIntValue, AutoPackType.Unpacked);
            converter = new UIntFloat { FloatValue = value.y };
            WriteUInt32(converter.UIntValue, AutoPackType.Unpacked);
            converter = new UIntFloat { FloatValue = value.z };
            WriteUInt32(converter.UIntValue, AutoPackType.Unpacked);
            converter = new UIntFloat { FloatValue = value.w };
            WriteUInt32(converter.UIntValue, AutoPackType.Unpacked);
        }

        /// <summary>
        /// Writes a Vector2Int.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVector2Int(Vector2Int value)
        {
            WriteInt32(value.x);
            WriteInt32(value.y);
        }

        /// <summary>
        /// Writes a Vector3Int.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVector3Int(Vector3Int value)
        {
            WriteInt32(value.x);
            WriteInt32(value.y);
            WriteInt32(value.z);
        }

        /// <summary>
        /// Writes a Color.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteColor(Color value, AutoPackType packType = AutoPackType.Packed)
        {
            if (packType == AutoPackType.Unpacked)
            {
                WriteSingle(value.r);
                WriteSingle(value.g);
                WriteSingle(value.b);
                WriteSingle(value.a);
            }
            else
            {
                if (Position + 4 > _buffer.Length)
                    DoubleBuffer(4);

                _buffer[Position++] = (byte)(value.r * 100f);
                _buffer[Position++] = (byte)(value.g * 100f);
                _buffer[Position++] = (byte)(value.b * 100f);
                _buffer[Position++] = (byte)(value.a * 100f);

                Length = Math.Max(Length, Position);
            }
        }

        /// <summary>
        /// Writes a Color32.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteColor32(Color32 value)
        {
            if (Position + 4 > _buffer.Length)
                DoubleBuffer(4);

            _buffer[Position++] = value.r;
            _buffer[Position++] = value.g;
            _buffer[Position++] = value.b;
            _buffer[Position++] = value.a;

            Length = Math.Max(Length, Position);
        }

        /// <summary>
        /// Writes a Quaternion.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteQuaternion(Quaternion value)
        {
            if (Position + 4 > _buffer.Length)
                DoubleBuffer(4);

            uint result = Quaternions.Compress(value);
            _buffer[Position++] = (byte)result;
            _buffer[Position++] = (byte)(result >> 8);
            _buffer[Position++] = (byte)(result >> 16);
            _buffer[Position++] = (byte)(result >> 24);

            Length = Math.Max(Length, Position);
        }

        /// <summary>
        /// Writes a rect.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRect(Rect value)
        {
            WriteSingle(value.xMin);
            WriteSingle(value.yMin);
            WriteSingle(value.width);
            WriteSingle(value.height);
        }

        /// <summary>
        /// Writes a plane.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WritePlane(Plane value)
        {
            WriteVector3(value.normal);
            WriteSingle(value.distance);
        }

        /// <summary>
        /// Writes a Ray.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRay(Ray value)
        {
            WriteVector3(value.origin);
            WriteVector3(value.direction);
        }

        /// <summary>
        /// Writes a Ray2D.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRay2D(Ray2D value)
        {
            WriteVector2(value.origin);
            WriteVector2(value.direction);
        }


        /// <summary>
        /// Writes a Matrix4x4.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteMatrix4x4(Matrix4x4 value)
        {
            WriteSingle(value.m00);
            WriteSingle(value.m01);
            WriteSingle(value.m02);
            WriteSingle(value.m03);
            WriteSingle(value.m10);
            WriteSingle(value.m11);
            WriteSingle(value.m12);
            WriteSingle(value.m13);
            WriteSingle(value.m20);
            WriteSingle(value.m21);
            WriteSingle(value.m22);
            WriteSingle(value.m23);
            WriteSingle(value.m30);
            WriteSingle(value.m31);
            WriteSingle(value.m32);
            WriteSingle(value.m33);
        }

        /// <summary>
        /// Writes a Guid.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteGuidAllocated(System.Guid value)
        {
            byte[] data = value.ToByteArray();
            WriteBytes(data, 0, data.Length);
        }

        /// <summary>
        /// Writes a GameObject. GameObject must be spawned over the network already.
        /// </summary>
        /// <param name="go"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteGameObject(GameObject go)
        {
            if (go == null)
            {
                //Write -1, indicating null.
                WriteInt32(-1);
            }
            else
            {
                NetworkObject nob = go.GetComponent<NetworkObject>();
                WriteNetworkObject(nob);
            }
        }


        /// <summary>
        /// Writes a NetworkObject.
        /// </summary>
        /// <param name="nob"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteNetworkObject(NetworkObject nob)
        {
            if (nob == null)
            {
                WriteInt16(-1);
            }
            else if (!nob.IsSpawned)
            {
                WriteInt16(-1);
                if (NetworkManager.StaticCanLog(LoggingType.Warning))
                    Debug.LogWarning($"NetworkObject on GameObject {nob.name} is not spawned. Only objects spawned by the server can be written over the network.");
            }
            else
            {
                WriteInt16((short)nob.ObjectId);
            }
        }

        /// <summary>
        /// Writes a NetworkBehaviour.
        /// </summary>
        /// <param name="nb"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteNetworkBehaviour(NetworkBehaviour nb)
        {
            if (nb == null)
            {
                WriteNetworkObject(null);
                WriteByte(0);
            }
            else if (!nb.IsSpawned)
            {
                if (NetworkManager.StaticCanLog(LoggingType.Warning))
                    Debug.LogWarning($"NetworkObject on GameObject {nb.name} is not spawned.");
                WriteNetworkObject(null);
                WriteByte(0);
            }
            else
            {
                WriteNetworkObject(nb.NetworkObject);
                WriteByte(nb.ComponentIndex);
            }
        }

        /// <summary>
        /// Writes a transport channel.
        /// </summary>
        /// <param name="channel"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteChannel(Channel channel)
        {
            WriteByte((byte)channel);
        }

        /// <summary>
        /// Writes a NetworkConnection.
        /// </summary>
        /// <param name="connection"></param>
        public void WriteNetworkConnection(NetworkConnection connection)
        {
            int value = (connection == null) ? -1 : connection.ClientId;
            WriteInt16((short)value);
        }


        /// <summary>
        /// Writes a short for a connectionId.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodegenExclude]
        public void WriteNetworkConnectionId(short id)
        {
            WriteInt16(id);
        }

        #region Packed writers.
        /// <summary>
        /// Returns PackRate to use for value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [CodegenExclude]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PackRate GetPackRate(ulong value)
        {
            if (value <= byte.MaxValue)
                return PackRate.OneByte;
            else if (value <= ushort.MaxValue)
                return PackRate.TwoBytes;
            else if (value <= uint.MaxValue)
                return PackRate.FourBytes;
            else
                return PackRate.EightBytes;
        }
        /// <summary>
        /// Writes a packed whole number.
        /// </summary>
        /// <param name="value"></param>
        [CodegenExclude]
        public void WritePackedWhole(ulong value)
        {
            PackRate pr = GetPackRate(value);
            /* PackRate returns how many bytes data can fit into.
             * So need to add one extra for the packRate byte, which specifies
             * how many bytes data will use. */
            int neededLength = ((byte)pr + 1);
            if (Position + neededLength > _buffer.Length)
                DoubleBuffer(neededLength);
            //Add packrate.
            _buffer[Position++] = (byte)pr;

            if (pr == PackRate.OneByte)
            {
                _buffer[Position++] = (byte)value;
            }
            else if (value <= ushort.MaxValue)
            {
                _buffer[Position++] = (byte)value;
                _buffer[Position++] = (byte)(value >> 8);
            }
            else if (value <= uint.MaxValue)
            {
                _buffer[Position++] = (byte)value;
                _buffer[Position++] = (byte)(value >> 8);
                _buffer[Position++] = (byte)(value >> 16);
                _buffer[Position++] = (byte)(value >> 24);
            }
            else
            {
                _buffer[Position++] = (byte)value;
                _buffer[Position++] = (byte)(value >> 8);
                _buffer[Position++] = (byte)(value >> 16);
                _buffer[Position++] = (byte)(value >> 24);
                _buffer[Position++] = (byte)(value >> 32);
                _buffer[Position++] = (byte)(value >> 40);
                _buffer[Position++] = (byte)(value >> 48);
                _buffer[Position++] = (byte)(value >> 56);
            }

            Length = Math.Max(Length, Position);
        }
        #endregion

        #region Nullables.
        ///// <summary>
        ///// Writes a nullable int.
        ///// </summary>
        ///// <param name="value"></param>
        //public void WriteNullableInt(int? value)
        //{
        //    if (value == null)
        //        WriteByte(0);
        //    else
        //        WriteByte(1);

        //    WriteInt32(value.Value);
        //}
        #endregion

        #region Generators.
        /// <summary>
        /// Writes to the end of value starting at offset.
        /// </summary>
        [CodegenExclude]
        public void WriteToEnd<T>(List<T> value, int offset)
        {
            int count = value.Count;
            if (value == null || offset >= count)
            {
                WriteInt32(-1);
            }
            else
            {

                WriteInt32(count - offset);
                for (int i = offset; i < count; i++)
                    Write<T>(value[i]);
            }
        }
        /// <summary>
        /// Writes to the end of value starting at offset.
        /// </summary>
        [CodegenExclude]
        public void WriteToEnd<T>(T[] value, int offset)
        {
            int length = value.Length;
            if (value == null || offset >= length)
            {
                WriteInt32(-1);
            }
            else
            {

                WriteInt32(length - offset);
                for (int i = offset; i < length; i++)
                    Write<T>(value[i]);
            }
        }

        /// <summary>
        /// Writers any supported type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void Write<T>(T value)
        {
            if (IsDefaultAutoPack<T>(out AutoPackType packType))
            {
                Action<Writer, T, AutoPackType> del = GenericWriter<T>.WriteAutoPack;
                if (del == null)
                {
                    if (NetworkManager.StaticCanLog(LoggingType.Error))
                        Debug.LogError($"Write method not found for {typeof(T).Name}. Use a supported type or create a custom serializer.");
                }
                else
                {
                    del.Invoke(this, value, packType);
                }
            }
            else
            {
                Action<Writer, T> del = GenericWriter<T>.Write;
                if (del == null)
                {
                    if (NetworkManager.StaticCanLog(LoggingType.Error))
                        Debug.LogError($"Write method not found for {typeof(T).Name}. Use a supported type or create a custom serializer.");
                }
                else
                {
                    del.Invoke(this, value);
                }
            }
        }

        /// <summary>
        /// Returns if T takes AutoPackType argument.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="packType"></param>
        /// <returns></returns>
        internal static bool IsDefaultAutoPack<T>(out AutoPackType packType)
        {
            //performance bench this against using a hash lookup.
            System.Type type = typeof(T);
            if ((type == typeof(int) || type == typeof(uint) ||
                type == typeof(long) || type == typeof(ulong)) ||
                type == typeof(Color))
            {
                packType = AutoPackType.Packed;
                return true;
            }
            else if (type == typeof(float))
            {
                packType = AutoPackType.Unpacked;
                return true;
            }
            else
            {
                packType = AutoPackType.Unpacked;
                return false;
            }
        }
        #endregion

    }
}
