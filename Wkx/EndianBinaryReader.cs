using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Wkx
{
    public class EndianBinaryReader : BinaryReader
    {
        public bool IsBigEndian { get; set; }

        public EndianBinaryReader(Stream input)
            : this(input, false)
        {
        }

        public EndianBinaryReader(Stream input, bool isBigEndian)
            : this(input, Encoding.UTF8, isBigEndian)
        {
        }

        public EndianBinaryReader(Stream input, Encoding encoding)
            : this(input, encoding, false, false)
        {
        }

        public EndianBinaryReader(Stream input, Encoding encoding, bool isBigEndian)
            : this(input, encoding, false, isBigEndian)
        {
        }

        public EndianBinaryReader(Stream input, Encoding encoding, bool leaveOpen, bool isBigEndian)
            : base(input, encoding, leaveOpen)
        {
            IsBigEndian = isBigEndian;
        }

        public override double ReadDouble()
        {
            if (IsBigEndian)
                return BitConverter.ToDouble(ReadReversedBytes(8), 0);

            return base.ReadDouble();
        }

        public override short ReadInt16()
        {
            if (IsBigEndian)
                return BitConverter.ToInt16(ReadReversedBytes(2), 0);

            return base.ReadInt16();
        }

        public override int ReadInt32()
        {
            if (IsBigEndian)
                return BitConverter.ToInt32(ReadReversedBytes(4), 0);

            return base.ReadInt32();
        }

        public override long ReadInt64()
        {
            if (IsBigEndian)
                return BitConverter.ToInt64(ReadReversedBytes(8), 0);

            return base.ReadInt64();
        }

        public override ushort ReadUInt16()
        {
            if (IsBigEndian)
                return BitConverter.ToUInt16(ReadReversedBytes(2), 0);

            return base.ReadUInt16();
        }

        public override uint ReadUInt32()
        {
            if (IsBigEndian)
                return BitConverter.ToUInt32(ReadReversedBytes(4), 0);

            return base.ReadUInt32();
        }

        public override ulong ReadUInt64()
        {
            if (IsBigEndian)
                return BitConverter.ToUInt64(ReadReversedBytes(8), 0);

            return base.ReadUInt64();
        }

        private byte[] ReadReversedBytes(int count)
        {
            return ReadBytes(count).Reverse().ToArray();
        }
    }
}