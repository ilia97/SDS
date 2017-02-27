using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Misc
{
    public static class Converter
    {
        public static string ToBitArray(this string text)
        {
            byte[] bytes = new byte[text.Length * sizeof(char)];
            Buffer.BlockCopy(text.ToCharArray(), 0, bytes, 0, bytes.Length);

            var bits = string.Join("", bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));

            return bits;
        }

        public static string FromBitsToString(this string bits)
        {
            var bytes = Enumerable.Range(0, bits.Length / 8)
                .Select(pos => Convert.ToByte(bits.Substring(pos * 8, 8), 2))
                .ToArray();

            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);

            return new string(chars);
        }
    }
}
