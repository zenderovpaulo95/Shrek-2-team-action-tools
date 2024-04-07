using System;
using System.IO;

namespace Shrek_2_team_action_tools
{
    public class OtherMethods
    {
        public static bool hasSpecificChars(string str)
        {
            for(int i = 0; i < str.Length; i++)
            {
                if (str[i] < ' ') return true;
            }

            return false;
        }


        public static byte[] argb4444ToArgb8888(byte[] bytes, int width, int height)
        {
            int cpel = width * height;
            int newSize = cpel * 4;
            byte[] result = new byte[newSize];

            int offset = 0;

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    for (int i = 0; i < cpel; i++)
                    {
                        ushort pel = br.ReadUInt16();
                        byte r = (byte)(pel & 0xFF);
                        byte g = (byte)((pel >> 4) & 0xFF);
                        byte b = (byte)((pel >> 8) & 0xFF);
                        byte a = (byte)((pel >> 12) & 0xFF);

                        r <<= 4;
                        g <<= 4;
                        b <<= 4;
                        a <<= 4;

                        uint res = (uint)(r | g << 8 | b << 16 | a << 24);
                        byte[] tmp = BitConverter.GetBytes(res);
                        Array.Copy(tmp, 0, result, offset, tmp.Length);
                        offset += 4;
                    }
                }
            }

            return result;
        }

        public static byte[] argb8888ToArgb4444(byte[] bytes, int width, int height)
        {
            //make actual number of pixel elements in the buffer
            int cpel = width * height;
            int newSize = cpel * 2;

            byte[] result = new byte[newSize];

            int offset = 0;

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    for (int i = 0; i < cpel; i++)
                    {
                        uint pel = br.ReadUInt32();
                        //Unpack source data as 8 bit values
                        byte r = (byte)(pel & 0xFF);
                        byte g = (byte)((pel >> 8) & 0xFF);
                        byte b = (byte)((pel >> 16) & 0xFF);
                        byte a = (byte)((pel >> 24) & 0xFF);

                        //Convert to 4 bit values
                        r >>= 4;
                        g >>= 4;
                        b >>= 4;
                        a >>= 4;

                        //and store
                        ushort res = (ushort)(r | g << 4 | b << 8 | a << 12);
                        byte[] tmp = BitConverter.GetBytes(res);
                        Array.Copy(tmp, 0, result, offset, tmp.Length);

                        offset += 2;
                    }
                }
            }

            return result;
        }
    }
}
