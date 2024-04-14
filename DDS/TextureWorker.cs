using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shrek_2_team_action_tools.DDS
{
    public class TextureWorker
    {
        public static int ReadDDSHeader(Stream stream, ref int width, ref int height, ref int mip, ref uint textureFormat)
        {
            BinaryReader br = new BinaryReader(stream);
            try
            {
                dds.header head;
                byte[] tmp = br.ReadBytes(4);
                head.head = Encoding.ASCII.GetString(tmp);
                head.Size = br.ReadUInt32();
                head.Flags = br.ReadUInt32();
                head.Height = br.ReadUInt32();
                head.Width = br.ReadUInt32();
                head.PitchOrLinearSize = br.ReadUInt32();
                head.Depth = br.ReadUInt32();
                head.MipMapCount = br.ReadUInt32();
                head.Reserved1 = new uint[11];

                for (int i = 0; i < 11; i++)
                {
                    head.Reserved1[i] = br.ReadUInt32();
                }

                head.PF.Size = br.ReadUInt32();
                head.PF.Flags = br.ReadUInt32();
                tmp = br.ReadBytes(4);
                head.PF.FourCC = Encoding.ASCII.GetString(tmp);
                head.PF.RgbBitCount = br.ReadUInt32();
                head.PF.RBitMask = br.ReadUInt32();
                head.PF.GBitMask = br.ReadUInt32();
                head.PF.BBitMask = br.ReadUInt32();
                head.PF.ABitMask = br.ReadUInt32();

                head.Caps = br.ReadUInt32();
                head.Caps2 = br.ReadUInt32();
                head.Caps3 = br.ReadUInt32();
                head.Caps4 = br.ReadUInt32();
                head.Reserved2 = br.ReadUInt32();

                width = (int)head.Width;
                height = (int)head.Height;
                mip = head.MipMapCount < 1 ? 1 : (int)head.MipMapCount;

                Flags flags = new Flags();

                if ((uint)(head.PF.Flags & flags.DDPF_RGB) == flags.DDPF_RGB)
                {
                    /*switch (head.PF.RgbBitCount)
                    {
                        case 32:
                            textureFormat = (uint)ClassesStructs.TextureClass.OldTextureFormat.DX_ARGB8888;
                            break;
                    }*/
                }
                else if (head.PF.Flags == 2 && head.PF.RgbBitCount == 8)
                {
                    //textureFormat = (uint)ClassesStructs.TextureClass.OldTextureFormat.DX_L8;
                }
                else if (((uint)(head.PF.Flags & flags.DDPF_LUMINANCE) == flags.DDPF_LUMINANCE) || ((uint)(head.PF.Flags & flags.DDPF_ALPHA) == flags.DDPF_ALPHA))
                {
                    //textureFormat = (uint)ClassesStructs.TextureClass.OldTextureFormat.DX_L8;
                }
                else if ((uint)(head.PF.Flags & flags.DDPF_FOURCC) == flags.DDPF_FOURCC)
                {
                    switch (head.PF.FourCC)
                    {
                        case "DXT1":
                            textureFormat = 12;
                            break;

                        case "DXT3":
                            //textureFormat = (uint)ClassesStructs.TextureClass.OldTextureFormat.DX_DXT3;
                            break;

                        case "DXT5":
                            textureFormat = 15;
                            break;

                        default:
                            throw new Exception("Unknown format");
                    }
                }

                return 0;
            }
            catch
            {
                if (br != null) br.Close();
                return -1;
            }
        }

        public static byte[] GenHeader(uint Format, int Width, int Height, uint Size, int MipCount)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            try
            {
                dds.header head;
                head.head = "DDS ";
                byte[] tmp = Encoding.ASCII.GetBytes(head.head);
                bw.Write(tmp);

                head.Size = 124;
                bw.Write(head.Size);

                head.Flags = 0;
                Flags flags = new Flags();
                head.Flags = flags.DDSD_WIDTH | flags.DDSD_HEIGHT | flags.DDSD_PIXELFORMAT | flags.DDSD_CAPS;
                //head.Flags |= Format < 0x40 ? flags.DDSD_PITCH : flags.DDSD_LINEARSIZE;
                head.Flags |= flags.DDSD_LINEARSIZE; //for compressed textures

                if (MipCount > 1) head.Flags |= flags.DDSD_MIPMAPCOUNT;

                bw.Write(head.Flags);

                head.Height = (uint)Height;
                bw.Write(head.Height);

                head.Width = (uint)Width;
                bw.Write(head.Width);

                head.PitchOrLinearSize = Size;
                /*if (Format < 0x40)
                {
                    int Pitch = 0;
                    int sz = 0;
                    Methods.getSizeAndKratnost((int)head.Width, (int)head.Height, (int)Format, ref sz, ref Pitch);
                    head.PitchOrLinearSize = (uint)Pitch;
                }*/
                bw.Write(head.PitchOrLinearSize);

                head.Depth = 0;
                bw.Write(head.Depth);

                head.MipMapCount = (uint)MipCount;
                bw.Write(head.MipMapCount);

                head.Reserved1 = new uint[11];

                for (int i = 0; i < 11; i++)
                {
                    bw.Write(head.Reserved1[i]);
                }

                head.PF.Size = 32;
                bw.Write(head.PF.Size);

                //head.PF.Flags = (Format >= 0x40 || Format == 0x25) ? flags.DDPF_FOURCC : flags.DDPF_RGB;
                head.PF.Flags = flags.DDPF_FOURCC;

                /*switch (Format)
                {
                    case 0x00:
                        head.PF.Flags = flags.DDPF_RGB | flags.DDPF_ALPHAPIXELS;
                        break;

                    case 0x10:
                        head.PF.Flags = flags.DDPF_ALPHA;
                        break;

                    case 0x11:
                        head.PF.Flags = flags.DDPF_LUMINANCE;
                        break;
                }*/

                bw.Write(head.PF.Flags);

                head.PF.FourCC = "\0\0\0\0";

                switch(Format)
                {
                    case 12:
                        head.PF.FourCC = "DXT1";
                        break;

                    case 15:
                        head.PF.FourCC = "DXT5";
                        break;
                }

                /*if (Faces <= 1 && ArrayMember <= 1)
                {
                    for (int i = 0; i < TexCodes.Length; i++)
                    {
                        if (TexCodes[i] == Format)
                        {
                            head.PF.FourCC = FourCC[i];
                            StrFormat = Formats[i];
                            break;
                        }
                    }
                }
                else
                {
                    head.PF.FourCC = "DX10";
                }*/

                tmp = Encoding.ASCII.GetBytes(head.PF.FourCC);
                bw.Write(tmp);

                head.PF.RgbBitCount = 0;
                /*if (Format < 0x40)
                {
                    switch (Format)
                    {
                        case 0:
                            head.PF.RgbBitCount = 32;
                            break;
                        case 4:
                            head.PF.RgbBitCount = 16;
                            break;

                        case 0x10:
                        case 0x11:
                            head.PF.RgbBitCount = 8;
                            break;

                        default:
                            head.PF.RgbBitCount = 0;
                            break;
                    }
                }*/

                bw.Write(head.PF.RgbBitCount);
                head.PF.RBitMask = 0;
                head.PF.GBitMask = 0;
                head.PF.BBitMask = 0;
                head.PF.ABitMask = 0;

                /*if (Format < 0x40)
                {
                    switch (Format)
                    {
                        case (int)ClassesStructs.TextureClass.NewTextureFormat.ARGB8:
                            head.PF.RBitMask = 0xFF0000;
                            head.PF.GBitMask = 0xFF00;
                            head.PF.BBitMask = 0xFF;
                            head.PF.ABitMask = 0xFF000000;
                            break;

                        case (int)ClassesStructs.TextureClass.NewTextureFormat.ARGB4:
                            head.PF.RBitMask = 0xF00;
                            head.PF.GBitMask = 0xF0;
                            head.PF.BBitMask = 0xF;
                            head.PF.ABitMask = 0xF000;
                            break;

                        case (int)ClassesStructs.TextureClass.NewTextureFormat.IL8:
                            head.PF.RBitMask = 0xFF;
                            head.PF.GBitMask = 0xFF;
                            head.PF.BBitMask = 0xFF;
                            head.PF.ABitMask = 0;
                            break;

                        case (int)ClassesStructs.TextureClass.NewTextureFormat.A8:
                            head.PF.RBitMask = 0;
                            head.PF.GBitMask = 0;
                            head.PF.BBitMask = 0;
                            head.PF.ABitMask = 0xFF;
                            break;
                    }
                }*/

                bw.Write(head.PF.RBitMask);
                bw.Write(head.PF.GBitMask);
                bw.Write(head.PF.BBitMask);
                bw.Write(head.PF.ABitMask);

                Caps caps = new Caps();

                head.Caps = caps.DDSCAPS_TEXTURE;
                head.Caps2 = 0;
                head.Caps3 = 0;
                head.Caps4 = 0;
                head.Reserved2 = 0;

                if (MipCount > 1) head.Caps |= caps.DDSCAPS_COMPLEX | caps.DDSCAPS_MIPMAP;

                //Need find out how caps2 value works
                head.Caps2 = caps.DDSCAPS2_CUBEMAP_POSITIVEY;

                bw.Write(head.Caps);
                bw.Write(head.Caps2);
                bw.Write(head.Caps3);
                bw.Write(head.Caps4);
                bw.Write(head.Reserved2);

                /*if (head.PF.FourCC == "DX10")
                {
                    head.headDX11.DF = dds.DxgiFormat.DXGI_FORMAT_UNKNOWN;

                    switch (Format)
                    {
                        case (uint)ClassesStructs.TextureClass.NewTextureFormat.ARGB8:
                            head.headDX11.DF = dds.DxgiFormat.DXGI_FORMAT_R8G8B8A8_UNORM;
                            break;

                        case (uint)ClassesStructs.TextureClass.NewTextureFormat.ARGB4:
                            head.headDX11.DF = dds.DxgiFormat.DXGI_FORMAT_B4G4R4A4_UNORM;
                            break;

                        case (uint)ClassesStructs.TextureClass.NewTextureFormat.A8:
                            head.headDX11.DF = dds.DxgiFormat.DXGI_FORMAT_A8_UNORM;
                            break;

                        case (uint)ClassesStructs.TextureClass.NewTextureFormat.BC1:
                            head.headDX11.DF = dds.DxgiFormat.DXGI_FORMAT_BC1_UNORM;
                            break;

                        case (uint)ClassesStructs.TextureClass.NewTextureFormat.BC2:
                            head.headDX11.DF = dds.DxgiFormat.DXGI_FORMAT_BC2_UNORM;
                            break;

                        case (uint)ClassesStructs.TextureClass.NewTextureFormat.BC3:
                            head.headDX11.DF = dds.DxgiFormat.DXGI_FORMAT_BC3_UNORM;
                            break;

                        case (uint)ClassesStructs.TextureClass.NewTextureFormat.BC4:
                            head.headDX11.DF = dds.DxgiFormat.DXGI_FORMAT_BC4_UNORM;
                            break;

                        case (uint)ClassesStructs.TextureClass.NewTextureFormat.BC5:
                            head.headDX11.DF = dds.DxgiFormat.DXGI_FORMAT_BC5_UNORM;
                            break;

                        case (uint)ClassesStructs.TextureClass.NewTextureFormat.BC6:
                            head.headDX11.DF = dds.DxgiFormat.DXGI_FORMAT_BC6H_TYPELESS;
                            break;

                        case (uint)ClassesStructs.TextureClass.NewTextureFormat.BC7:
                            head.headDX11.DF = dds.DxgiFormat.DXGI_FORMAT_BC7_UNORM;
                            break;
                    }

                    head.headDX11.ResourceDimension = dds.D3D10ResourceDimension.D3D10_RESOURCE_DIMENSION_TEXTURE2D;
                    head.headDX11.MiscFlag = 0;
                    head.headDX11.MiscFlag2 = 0;
                    head.headDX11.ArraySize = (uint)ArrayMember;

                    bw.Write((uint)head.headDX11.DF);
                    bw.Write((uint)head.headDX11.ResourceDimension);
                    bw.Write(head.headDX11.MiscFlag);
                    bw.Write(head.headDX11.ArraySize);
                    bw.Write(head.headDX11.MiscFlag2);
                }*/
                //NEED THINK ABOUT DX10 FORMAT!

                byte[] header = ms.ToArray();
                bw.Close();
                ms.Close();
                return header;
            }
            catch
            {
                if (bw != null) bw.Close();
                if (ms != null) ms.Close();
                return null;
            }
        }
    }
}
