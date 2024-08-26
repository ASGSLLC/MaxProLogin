using System;
using System.Text;
using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    [Preserve]
    public static class ConversionUtility
    {
        public static string ToAscii(string hex)
        {
            StringBuilder ascii = new StringBuilder(hex.Length / 2);

            for (int x = 0; x < hex.Length; x += 2)
            {
                string s = hex.Substring(x, 2);
                char c = (char)Convert.ToInt32(s, 16);
                ascii.Append(c);
            }

            return ascii.ToString();
        }

        public static string ToAscii(byte[] bytes, int offset, int count)
        {
            string hex = ToHex(bytes, offset, count);

            return ToAscii(hex);
        }

        public static byte[] ToBytes(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length; i += 2)
            {
                string s = hex.Substring(i, 2);
                bytes[i / 2] = Convert.ToByte(s, 16);
            }

            return bytes;
        }

        public static string ToHex(string ascii)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(ascii);

            return ToHex(bytes, 0, bytes.Length);
        }

        public static string ToHex(byte[] bytes, int offset, int count)
        {
            StringBuilder hex = new StringBuilder(count * 2);

            for (int i = 0; i < count; i++)
            {
                hex.AppendFormat("{0:x2}", bytes[offset + i]);
            }

            return hex.ToString();
        }

        public static ushort ToUshort(byte[] bytes, int offset)
        {
            return (ushort)((bytes[offset] << 8) | bytes[offset + 1]);
        }
    }
}
