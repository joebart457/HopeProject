using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope.Compiler.Extensions
{
    internal static class ByteExtensions
    {

        public static byte[] ToBytes(this long val)
        {
            byte[] intBytes = BitConverter.GetBytes(val);
            return intBytes;
        }

        public static byte[] ToBytes(this int val)
        {
            byte[] intBytes = BitConverter.GetBytes(val);
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(intBytes);
            return intBytes;
        }

        public static byte[] ToBytes(this uint val)
        {
            byte[] intBytes = BitConverter.GetBytes(val);
            return intBytes;
        }

        public static byte[] ToBytes(this short val)
        {
            byte[] shortBytes = BitConverter.GetBytes(val);
            return shortBytes;
        }
        public static byte[] ToBytes(this double val)
        {
            byte[] shortBytes = BitConverter.GetBytes(val);
            return shortBytes;
        }

        public static byte[] ToBytes(this string val)
        {
            return Encoding.ASCII.GetBytes(val);
        }
    }
}
