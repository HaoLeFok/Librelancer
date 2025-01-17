using System;
using System.Runtime.InteropServices;
using System.Numerics;
using LiteNetLib.Utils;

namespace LibreLancer
{ 
    public class BitWriter
    {
        private const int GROWTH_AMOUNT = 4;
        
        private byte[] buffer;
        private int bitOffset;
        public BitWriter(int initialCapacity = 64)
        {
            buffer = new byte [(initialCapacity + 7) >> 3];
            bitOffset = 0;
        }

        public void PutInt(int i) => PutUInt((uint) i, 32);
        public void PutByte(byte b) => PutUInt( b, 8);
        [StructLayout(LayoutKind.Explicit)]
        struct F2I
        {
            [FieldOffset(0)] public float f;
            [FieldOffset(0)] public uint i;
        }
        public void PutFloat(float f)
        {
            var c = new F2I() {f = f};
            PutUInt(c.i, 32);
        }
        public void PutVector3(Vector3 vec)
        {
            PutFloat(vec.X); PutFloat(vec.Y); PutFloat(vec.Z);
        }

        static float WrapMinMax(float x, float min, float max)
        {
            var m = max - min;
            var y = (x - min);
            return min + (m + y % m) % m;
        }
        public void PutRadiansQuantized(float angle)
        {
            var wrapped = WrapMinMax(angle, NetPacking.ANGLE_MIN, NetPacking.ANGLE_MAX);
            PutRangedFloat(angle, NetPacking.ANGLE_MIN, NetPacking.ANGLE_MAX, 16);
        }

        public void PutNormal(Vector3 v)
        {
            v.Normalize();
            var maxIndex = 0;
            var maxValue = Math.Abs(v.X);
            bool sign = v.X < 0;
            if (Math.Abs(v.Y) > maxValue) {
                maxIndex = 1;
                maxValue = Math.Abs(v.Y);
                sign = v.Y < 0;
            }
            if (Math.Abs(v.Z) > maxValue)
            {
                maxIndex = 2;
                sign = v.Z < 0;
            }
            PutUInt((uint)maxIndex, 2);
            PutBool(sign);
            if (maxIndex == 0)
            {
                PutRangedFloat( v.Y, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, 14);
                PutRangedFloat(v.Z, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, 15);
            }
            if (maxIndex == 1)
            {
                PutRangedFloat(v.X, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, 14);
                PutRangedFloat(v.Z, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, 15);
            }
            if (maxIndex == 2)
            {
                PutRangedFloat(v.X, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, 14);
                PutRangedFloat(v.Y, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, 15);
            }
        }

        public void PutQuaternion(Quaternion q)
        {
            var maxIndex = 0;
            var maxValue = float.MinValue;
            var sign = 1f;
            maxValue = Math.Abs(q.X);
            sign = q.X < 0 ? -1 : 1;
            if (Math.Abs(q.Y) > maxValue)
            {
                maxValue = Math.Abs(q.Y);
                maxIndex = 1;
                sign = q.Y < 0 ? -1 : 1;
            }
            if (Math.Abs(q.Z) > maxValue)
            {
                maxValue = Math.Abs(q.Z);
                maxIndex = 2;
                sign = q.Z < 0 ? -1 : 1;
            }
            if (Math.Abs(q.W) > maxValue)
            {
                maxIndex = 3;
                sign = q.W < 0 ? -1 : 1;
            }
            PutUInt((uint)maxIndex, 2);
            if (maxIndex == 0)
            {
                PutRangedFloat(q.Y * sign, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, NetPacking.BITS_COMPONENT);
                PutRangedFloat(q.Z * sign, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, NetPacking.BITS_COMPONENT);
                PutRangedFloat(q.W * sign, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, NetPacking.BITS_COMPONENT);
            }
            else if (maxIndex == 1)
            {
                PutRangedFloat(q.X * sign, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, NetPacking.BITS_COMPONENT);
                PutRangedFloat(q.Z * sign, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, NetPacking.BITS_COMPONENT);
                PutRangedFloat(q.W * sign, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, NetPacking.BITS_COMPONENT);
            }
            else if (maxIndex == 2)
            {
                PutRangedFloat(q.X * sign, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, NetPacking.BITS_COMPONENT);
                PutRangedFloat(q.Y * sign, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, NetPacking.BITS_COMPONENT);
                PutRangedFloat(q.W * sign, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, NetPacking.BITS_COMPONENT);
            }
            else
            {
                PutRangedFloat(q.X * sign, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, NetPacking.BITS_COMPONENT);
                PutRangedFloat(q.Y * sign, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, NetPacking.BITS_COMPONENT);
                PutRangedFloat(q.Z * sign, NetPacking.UNIT_MIN, NetPacking.UNIT_MAX, NetPacking.BITS_COMPONENT);
            }
        }
        public void PutBool(bool b)
        {
            CheckSize(bitOffset + 1);
            PackBits(b ? (byte) 1 : (byte) 0, 1, buffer, bitOffset++);
        }
        public void PutUInt(uint u, int bits)
        {
            CheckSize(bitOffset + bits);
            PackUInt(u, bits, buffer, bitOffset);
            bitOffset += bits;
        }
        public void PutRangedFloat(float f, float min, float max, int bits)
        {
            var intMax = (1 << bits) - 1;
            float unit = ((f - min) / (max - min));
            PutUInt((uint)(intMax * unit), bits);
        }
        static void PackUInt(uint src, int nBits, Span<byte> dest, int destOffset)
        {
            if (nBits <= 8)
            {
                PackBits((byte) src, nBits, dest, destOffset);
                return;
            }

            PackBits((byte) src, 8, dest, destOffset);
            destOffset += 8;
            nBits -= 8;
            if (nBits <= 8)
            {
                PackBits((byte) (src >> 8), nBits, dest, destOffset);
                return;
            }
            PackBits((byte) (src >> 8), 8, dest, destOffset);
            destOffset += 8;
            nBits -= 8;
            if (nBits <= 8)
            {
                PackBits((byte) (src >> 16), nBits, dest, destOffset);
                return;
            }

            PackBits((byte) (src >> 16), 8, dest, destOffset);
            destOffset += 8;
            nBits -= 8;
            PackBits((byte) (src >> 24), nBits, dest, destOffset);
        }
        static void PackBits(byte src, int nBits, Span<byte> dest, int destOffset)
        {
            src = (byte)(src & (0xFF >> (8 - nBits)));
            int p = destOffset >> 3;
            int bitsUsed = destOffset & 0x7;
            int bitsFree = 8 - bitsUsed;
            int bitsLeft = bitsFree - nBits;
            if (bitsLeft >= 0)
            {
                int mask = (0xFF >> bitsFree) | (0xFF << (8 - bitsLeft));
                dest[p] = (byte)(
                    (dest[p] & mask) |
                    (src << bitsUsed));
                return;
            }

            dest[p] = (byte) (
                (dest[p] & (0xFF >> bitsFree)) |
                (src << bitsUsed)
            );
            p++;
            dest[p] = (byte)(
                (dest[p] & (0xFF << (nBits - bitsFree))) |
                (src >> bitsFree)
            );
        }
        
        void CheckSize(int nBits)
        {
            int byteLen = (nBits + 7) >> 3;
            if (buffer.Length < byteLen)
                Array.Resize(ref buffer, byteLen + GROWTH_AMOUNT);
        }
        
        
        public int ByteLength => (bitOffset + 7) >> 3;
        public void WriteToPacket(NetDataWriter dw)
        {
            dw.Put(buffer, 0, (bitOffset + 7) >> 3);
        }
    }
}