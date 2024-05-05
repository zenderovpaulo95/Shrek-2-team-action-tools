using System;
using System.IO;
using System.Text;

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

        public static byte[] fillPadded(string str, int paddedSize)
        {
            byte[] tmp = new byte[paddedSize];
            byte[] binStr = Encoding.GetEncoding(MainForm.settings.ASCII).GetBytes(str);
            Array.Copy(binStr, 0, tmp, 0, binStr.Length);

            for (int i = binStr.Length; i < paddedSize; i++)
            {
                tmp[i] = (byte)'U';
            }

            return tmp;
        }

        public static byte[] fillPadded(byte[] block, int paddedSize, int offset)
        {
            byte[] tmp = new byte[paddedSize];
            Array.Copy(block, 0, tmp, 0, block.Length);

            for(int i = offset; i < paddedSize; i++)
            {
                tmp[i] = (byte)'U';
            }

            return tmp;
        }

        public static void getSizeAndKratnost(int width, int height, int code, ref int ddsContentLength, ref int kratnost)
        {
            uint w, h = 0;

            ddsContentLength = 0;

            w = (uint)width;
            h = (uint)height;
            w = Math.Max(1, w);
            h = Math.Max(1, h);
            w <<= 1;
            h <<= 1;

            if (w > 1) w >>= 1;
            if (h > 1) h >>= 1;

            switch (code)
            {
                case 12: //DXT1
                    ddsContentLength = (int)((((w + 3) >> 2) * ((h + 3) >> 2)) * 8);
                    kratnost = (int)((w + 3) >> 2) * 8;
                    break;

                case 15: //DXT5
                    ddsContentLength = (int)((((w + 3) >> 2) * ((h + 3) >> 2)) * 16);
                    kratnost = (int)((w + 3) >> 2) * 16;
                    break;

                default: //ARGB8888
                    ddsContentLength = (int)((w * h) * 4);
                    kratnost = (int)w * 4;
                    break;
            }
        }

        public static int padSize(int size, int pad)
        {
            while (size % pad != 0) size++;
            return size;
        }
    }
}
