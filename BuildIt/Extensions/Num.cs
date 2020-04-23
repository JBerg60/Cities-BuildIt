using System;

namespace BuildIt.Extensions
{
    public static class Num
    {
        public static ushort SwapBytes(ushort b)
        {
            byte[] bytes = BitConverter.GetBytes(b);
            Array.Reverse(bytes, 0, bytes.Length);

            return BitConverter.ToUInt16(bytes, 0);
        }

    }
}
