namespace Wkx
{
    internal static class ZigZag
    {
        internal static int Encode(int value)
        {
            return (value << 1) ^ (value >> 31);
        }

        internal static int Decode(int value)
        {
            return (value >> 1) ^ (-(value & 1));
        }
    }
}
