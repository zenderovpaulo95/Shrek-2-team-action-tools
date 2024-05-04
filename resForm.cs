using ImageMagick;
using Shrek_2_team_action_tools.DDS;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Shrek_2_team_action_tools
{
    public partial class resForm : Form
    {
        public resForm()
        {
            InitializeComponent();
        }

        void AddNewReport(string report)
        {
            if (listBoxRES.InvokeRequired)
            {
                listBoxRES.Invoke(new ReportHandler(AddNewReport), report);
            }
            else
            {
                listBoxRES.Items.Add(report);
                listBoxRES.SelectedIndex = listBoxRES.Items.Count - 1;
                listBoxRES.SelectedIndex = -1;
            }
        }

        struct xproFormat
        {
            public int fileSize;
            public uint textureOffset;
            public int textureSize;
            public int kratnostWidth;
            public int kratnostHeight;
            public int width;
            public int height;
            public int textureFormat;
            public byte[] data;
        }

        struct texturePackage
        {
            public int index;
            public int offset;
        }

        private xproFormat getData(Stream stream, int offset)
        {
            xproFormat format = new xproFormat();

            BinaryReader br = new BinaryReader(stream);
            br.BaseStream.Seek(offset + 4, SeekOrigin.Begin);
            format.fileSize = br.ReadInt32();
            format.textureOffset = br.ReadUInt32();

            br.BaseStream.Seek(offset + 25, SeekOrigin.Begin);
            format.textureFormat = 0;
            byte tf = br.ReadByte();
            byte wk = br.ReadByte();
            byte hk = br.ReadByte();
            format.textureFormat = (int)tf;
            format.kratnostWidth = (int)((wk & 0xF0) >> 4);
            format.kratnostHeight = (int)hk;

            br.BaseStream.Seek(offset + 30, SeekOrigin.Begin);
            byte[] tmp = br.ReadBytes(1);
            byte[] tmpVal = new byte[4];
            tmpVal[0] = tmp[0];

            format.height = BitConverter.ToInt32(tmpVal, 0) == 0 && format.textureFormat < 0x12 ? -1 : (BitConverter.ToInt32(tmpVal, 0) + 1) * 16;

            tmp = br.ReadBytes(1);
            tmpVal = new byte[4];
            tmpVal[0] = tmp[0];

            format.width = BitConverter.ToInt32(tmpVal, 0) == 0 && format.textureFormat < 0x12 ? -1 : (BitConverter.ToInt32(tmpVal, 0) + 1) * 16;

            format.textureSize = format.fileSize - (int)format.textureOffset < 0 ? 0 : format.width * format.height * 4; //for argb888

            if (format.kratnostWidth > 0 && format.kratnostHeight > 0 && (format.width == -1 || format.height == -1))
            {
                format.width = (int)Math.Pow(2, format.kratnostWidth);
                format.height = (int)Math.Pow(2, format.kratnostHeight);
                format.textureFormat = (int)tf;
                int kratnost = 0;
                OtherMethods.getSizeAndKratnost(format.width, format.height, format.textureFormat, ref format.textureSize, ref kratnost);
            }

            br.BaseStream.Seek(offset + format.textureOffset, SeekOrigin.Begin);
            format.data = br.ReadBytes(format.textureSize);

            if (format.textureFormat < 0x12)
            {
                byte[] header = TextureWorker.GenHeader((uint)format.textureFormat, format.width, format.height, (uint)format.textureSize, 1);
                tmp = new byte[header.Length + format.textureSize];
                Array.Copy(header, 0, tmp, 0, header.Length);
                Array.Copy(format.data, 0, tmp, header.Length, format.data.Length);
                format.data = new byte[tmp.Length];
                Array.Copy(tmp, 0, format.data, 0, tmp.Length);

                MagickImage img = new MagickImage(format.data);
                img.Settings.Compression = CompressionMethod.NoCompression;
                MemoryStream ms = new MemoryStream();
                img.Flip();
                img.Write(ms, MagickFormat.Bmp);

                Image dd = Image.FromStream(ms);
                Bitmap bmp = new Bitmap(format.width, format.height, PixelFormat.Format32bppArgb);
                using(Graphics graph = Graphics.FromImage(bmp))
                {
                    graph.DrawImage(dd, new Rectangle(0, 0, format.width, format.height));
                }
                ms.Close();

                ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Bmp);
                tmp = ms.ToArray();
                ms.Close();

                int tmpOff = 0;
                byte[] t = new byte[4];
                Array.Copy(tmp, 10, t, 0, 4);
                tmpOff = BitConverter.ToInt32(t, 0);
                format.data = new byte[tmp.Length - tmpOff];
                Array.Copy(tmp, tmpOff, format.data, 0, format.data.Length);
            }

            br.Close();

            return format;
        }

        private xproFormat getData(string fileName)
        {
            xproFormat format = new xproFormat();

            FileStream fs = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            br.BaseStream.Seek(4, SeekOrigin.Begin);
            format.fileSize = br.ReadInt32();
            format.textureOffset = br.ReadUInt32();

            br.BaseStream.Seek(25, SeekOrigin.Begin);
            format.textureFormat = 0;
            byte tf = br.ReadByte();
            byte wk = br.ReadByte();
            byte hk = br.ReadByte();
            format.textureFormat = (int)tf;
            format.kratnostWidth = (int)((wk & 0xF0) >> 4);
            format.kratnostHeight = (int)hk;

            br.BaseStream.Seek(30, SeekOrigin.Begin);
            byte[] tmp = br.ReadBytes(1);
            byte[] tmpVal = new byte[4];
            tmpVal[0] = tmp[0];

            format.height = BitConverter.ToInt32(tmpVal, 0) == 0 && format.textureFormat < 0x12 ? -1 : (BitConverter.ToInt32(tmpVal, 0) + 1) * 16;

            tmp = br.ReadBytes(1);
            tmpVal = new byte[4];
            tmpVal[0] = tmp[0];

            format.width = BitConverter.ToInt32(tmpVal, 0) == 0 && format.textureFormat < 0x12 ? -1 : (BitConverter.ToInt32(tmpVal, 0) + 1) * 16;

            format.textureSize = format.fileSize - (int)format.textureOffset < 0 ? 0 : format.fileSize - (int)format.textureOffset;

            if (format.kratnostWidth > 0 && format.kratnostHeight > 0 && (format.width == -1 || format.height == -1))
            {
                format.width = (int)Math.Pow(2, format.kratnostWidth);
                format.height = (int)Math.Pow(2, format.kratnostHeight);
                format.textureFormat = (int)tf;
                int kratnost = 0;
                OtherMethods.getSizeAndKratnost(format.width, format.height, format.textureFormat, ref format.textureSize, ref kratnost);
            }

            br.BaseStream.Seek(format.textureOffset, SeekOrigin.Begin);
            format.data = br.ReadBytes(format.textureSize);

            if(format.textureFormat < 0x12)
            {
                byte[] header = TextureWorker.GenHeader((uint)format.textureFormat, format.width, format.height, (uint)format.textureSize, 1);
                tmp = new byte[header.Length + format.textureSize];
                Array.Copy(header, 0, tmp, 0, header.Length);
                Array.Copy(format.data, 0, tmp, header.Length, format.data.Length);
                format.data = new byte[tmp.Length];
                Array.Copy(tmp, 0, format.data, 0, tmp.Length);

                MagickImage img = new MagickImage(format.data);
                img.Settings.Compression = CompressionMethod.NoCompression;
                MemoryStream ms = new MemoryStream();
                img.Flip();
                img.Flop();
                img.Write(ms, MagickFormat.Bmp);
                tmp = ms.ToArray();
                ms.Close();
                int tmpOff = 0;
                byte[] t = new byte[4];
                Array.Copy(tmp, 10, t, 0, 4);
                tmpOff = BitConverter.ToInt32(t, 0);
                format.data = new byte[tmp.Length - tmpOff];
                Array.Copy(tmp, tmpOff, format.data, 0, format.data.Length);
            }

            br.Close();
            fs.Close();

            return format;
        }

        private int getType(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            byte[] tmp = new byte[4];
            fs.Read(tmp, 0, 4);
            fs.Close();

            if (Encoding.ASCII.GetString(tmp) == "XPR0") return 0;
            if (Encoding.ASCII.GetString(tmp) == "DDS ") return 1;

            return -1;
        }

        private void extractDDS(FileInfo fi)
        {
            byte[] ddsContent = File.ReadAllBytes(fi.FullName);

            if (File.Exists(MainForm.settings.outputPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 3, 3) + "dds")) File.Delete(MainForm.settings.outputPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 3, 3) + "dds");

            File.WriteAllBytes(MainForm.settings.outputPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 3, 3) + "dds", ddsContent);

            AddNewReport("File " + fi.Name + " successfully extracted as DDS file.");
        }

        private void extractXPR0(string fileName, xproFormat format, int index = -1)
        {
            if (format.width != -1 && format.height != -1)
            {
                Bitmap bmp = new Bitmap(format.width, format.height, PixelFormat.Format32bppArgb);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                IntPtr bmpPtr = bmpData.Scan0;
                Marshal.Copy(format.data, 0, bmpPtr, format.data.Length);
                bmp.UnlockBits(bmpData);
                string outputName = MainForm.settings.outputPath + Path.DirectorySeparatorChar + fileName.Remove(fileName.Length - 4, 4);
                outputName += index != -1 ? "_" + Convert.ToString(index) + ".png" : ".png";
                bmp.Save(outputName);

                AddNewReport("File " + fileName + " successfully extracted as PNG.");
            }
            else
            {
                AddNewReport("File " + fileName + " probably doesn't have an image.");
            }
        }

        private void extractBtn_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(MainForm.settings.inputPath);
            string[] extensions = new[] { "*.res", "*.tpk" };
            FileInfo[] fi = extensions.SelectMany(x => di.GetFiles(x, SearchOption.AllDirectories)).ToArray();

            if(fi.Length > 0)
            {
                listBoxRES.Items.Clear();

                for(int i = 0; i < fi.Length; i++)
                {
                    if (fi[i].Extension.ToLower() == ".res")
                    {
                        int type = getType(fi[i].FullName);

                        switch (type)
                        {
                            case 0:
                                xproFormat format = getData(fi[i].FullName);
                                extractXPR0(fi[i].Name, format);
                                break;

                            case 1:
                                extractDDS(fi[i]);
                                break;

                            default:
                                AddNewReport("File " + fi[i].Name + ": uknown type");
                                break;
                        }
                    }
                    else
                    {
                        //Check if tpk file is bmp or dds
                        FileStream fs = new FileStream(fi[i].FullName, FileMode.Open);
                        BinaryReader br = new BinaryReader(fs);
                        byte[] tmp = br.ReadBytes(4);
                        br.Close();
                        fs.Close();

                        if (Encoding.ASCII.GetString(tmp) == "DDS ")
                        {
                            extractDDS(fi[i]);
                        }
                        else
                        {

                            fs = new FileStream(fi[i].FullName, FileMode.Open);
                            br = new BinaryReader(fs);
                            tmp = br.ReadBytes(2);
                            int checkSize = br.ReadInt32();
                            int fileSize = (int)fs.Length;
                            br.Close();
                            fs.Close();

                            if (Encoding.ASCII.GetString(tmp) == "BM" && checkSize == fileSize)
                            {
                                tmp = File.ReadAllBytes(fi[i].FullName);
                                File.WriteAllBytes(MainForm.settings.outputPath + Path.DirectorySeparatorChar + fi[i].Name.Remove(fi[i].Name.Length - 3, 3) + "bmp", tmp);
                                AddNewReport("File " + fi[i].Name + " successfully extracted as BMP.");
                            }
                            else
                            {

                                fs = new FileStream(fi[i].FullName, FileMode.Open);
                                br = new BinaryReader(fs);
                                int count = br.ReadInt32();
                                texturePackage[] tp = new texturePackage[count];

                                for (int c = 0; c < count; c++)
                                {
                                    tp[c].index = br.ReadInt32();
                                    tp[c].offset = br.ReadInt32();
                                }

                                br.Close();
                                fs.Close();

                                for (int c = 0; c < count; c++)
                                {
                                    fs = new FileStream(fi[i].FullName, FileMode.Open);
                                    br = new BinaryReader(fs);

                                    xproFormat format = getData(fs, tp[c].offset);
                                    extractXPR0(fi[i].Name, format, c);

                                    br.Close();
                                    fs.Close();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void importXPR0(xproFormat format, FileInfo resFI, FileInfo pngFI)
        {
            byte[] file = File.ReadAllBytes(resFI.FullName);
            byte[] rawData = null;
            int size = 0;

            Image img = Bitmap.FromFile(pngFI.FullName);

            if(img.PixelFormat != PixelFormat.Format32bppArgb)
            {
                AddNewReport("File " + resFI.Name + ": " + pngFI.Name + ": bitmap must be ARGB8888!");
                return;
            }

            if(img.Width != format.width || img.Height != format.height)
            {
                AddNewReport("File " + resFI.Name + ": " + pngFI.Name + " has incorrect width and height!");
                return;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                img.Save(ms, ImageFormat.Bmp);
                rawData = ms.ToArray();

                byte[] tmp = new byte[4];
                Array.Copy(rawData, 10, tmp, 0, tmp.Length);
                int offset = BitConverter.ToInt32(tmp, 0);
                size = rawData.Length - offset;
                format.data = new byte[size];
                Array.Copy(rawData, offset, format.data, 0, size);
            }

            if (format.textureFormat != 0)
            {
                MagickImage mgImg = new MagickImage(rawData);

                switch(format.textureFormat)
                {
                    case 12:
                        mgImg.Settings.Compression = CompressionMethod.DXT1;
                        break;

                    case 15:
                        mgImg.Settings.Compression = CompressionMethod.DXT5;
                        break;
                }

                using(MemoryStream ms = new MemoryStream())
                {
                    mgImg.Write(ms, MagickFormat.Dds);
                    rawData = ms.ToArray();
                    int texSize = 0;
                    int kratnost = 0;
                    OtherMethods.getSizeAndKratnost(format.width, format.height, format.textureFormat, ref texSize, ref kratnost);
                    byte[] tmpImg = new byte[texSize];
                    Array.Copy(rawData, 128, tmpImg, 0, tmpImg.Length);
                    format.data = new byte[tmpImg.Length];
                    Array.Copy(tmpImg, 0, format.data, 0, format.data.Length);
                    size = format.data.Length;
                }

                mgImg.Dispose();
                img.Dispose();
            }

            if(size != format.fileSize - format.textureOffset)
            {
                AddNewReport("File " + resFI.Name + " has incorrect size compare with " + pngFI.Name);
                return;
            }

            img.Dispose();

            if (File.Exists(MainForm.settings.outputPath + Path.DirectorySeparatorChar + resFI.Name)) File.Delete(MainForm.settings.outputPath + Path.DirectorySeparatorChar + resFI.Name);
            Array.Copy(format.data, 0, file, format.textureOffset, format.data.Length);
            File.WriteAllBytes(MainForm.settings.outputPath + Path.DirectorySeparatorChar + resFI.Name, file);

            AddNewReport("File " + resFI + " successfully imported.");
        }

        private void importDDS(FileInfo ddsFI)
        {
            File.Copy(ddsFI.FullName, MainForm.settings.outputPath + Path.DirectorySeparatorChar + ddsFI.Name.Remove(ddsFI.Name.Length - 3, 3) + "res");
            AddNewReport("File " + ddsFI.Name.Remove(ddsFI.Name.Length - 3, 3) + ".res successfully imported");
        }

        private void importBtn_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(MainForm.settings.inputPath);
            var extensions = new[] { "*.png", "*.dds" };
            var file_extensions = new[] { "*.res", "*.tpk" };
            FileInfo[] fi = file_extensions.SelectMany(im_ext => di.GetFiles(im_ext, SearchOption.AllDirectories)).ToArray();
            FileInfo[] fi2 = extensions.SelectMany(ext => di.GetFiles(ext, SearchOption.AllDirectories)).ToArray();

            if (fi.Length > 0 && fi2.Length > 0)
            {
                listBoxRES.Items.Clear();

                for (int i = 0; i < fi.Length; i++)
                {
                    if (fi[i].Extension.ToLower() == ".res")
                    {
                        for (int j = 0; j < fi2.Length; j++)
                        {
                            if (fi2[j].Name.Remove(fi2[j].Name.Length - 4, 4) == fi[i].Name.Remove(fi[i].Name.Length - 4, 4))
                            {
                                int type = getType(fi[i].FullName);

                                switch (type)
                                {
                                    case 0:
                                        xproFormat format = getData(fi[i].FullName);
                                        if (fi2[j].Extension.ToLower() == ".png")
                                        {
                                            importXPR0(format, fi[i], fi2[j]);
                                        }
                                        else
                                        {
                                            AddNewReport("File " + fi[i].Name + " needs a png file to import.");
                                        }
                                        break;

                                    case 1:
                                        if (fi2[j].Extension.ToLower() == ".dds")
                                        {
                                            importDDS(fi2[j]);
                                        }
                                        else
                                        {
                                            AddNewReport("File " + fi[i].Name + " needs a dds file to import.");
                                        }
                                        break;

                                    default:
                                        AddNewReport("File " + fi[i].Name + ": uknown type");
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        //нужно будет поправить
                        for (int j = 0; j < fi2.Length; j++)
                        {
                            if (fi2[j].Name.Remove(fi2[j].Name.Length - 4, 4) == fi[i].Name.Remove(fi[i].Name.Length - 4, 4))
                            {
                                //Check if tpk file is bmp or dds
                                FileStream fs = new FileStream(fi[i].FullName, FileMode.Open);
                                BinaryReader br = new BinaryReader(fs);
                                byte[] tmp = br.ReadBytes(4);
                                br.Close();
                                fs.Close();

                                if (Encoding.ASCII.GetString(tmp) == "DDS ")
                                {
                                    if (fi2[j].Extension.ToLower() == ".dds")
                                    {
                                        importDDS(fi2[j]);
                                    }
                                    else
                                    {
                                        AddNewReport("File " + fi[i].Name + " needs a dds file to import.");
                                    }
                                }
                                else
                                {

                                    fs = new FileStream(fi[i].FullName, FileMode.Open);
                                    br = new BinaryReader(fs);
                                    tmp = br.ReadBytes(2);
                                    int checkSize = br.ReadInt32();
                                    int fileSize = (int)fs.Length;
                                    br.Close();
                                    fs.Close();

                                    if (Encoding.ASCII.GetString(tmp) == "BM" && checkSize == fileSize)
                                    {
                                        if (fi2[j].Extension.ToLower() == ".bmp")
                                        {
                                            tmp = File.ReadAllBytes(fi2[j].FullName);
                                            File.WriteAllBytes(MainForm.settings.outputPath + Path.DirectorySeparatorChar + fi[i].Name, tmp);
                                            AddNewReport("File " + fi[i].Name + " successfully imported.");
                                        }
                                        else
                                        {
                                            AddNewReport("File " + fi[i].Name + " needs a bmp file to import.");
                                        }
                                    }
                                    else
                                    {

                                        fs = new FileStream(fi[i].FullName, FileMode.Open);
                                        br = new BinaryReader(fs);
                                        int count = br.ReadInt32();
                                        texturePackage[] tp = new texturePackage[count];

                                        for (int c = 0; c < count; c++)
                                        {
                                            tp[c].index = br.ReadInt32();
                                            tp[c].offset = br.ReadInt32();
                                        }

                                        br.Close();
                                        fs.Close();

                                        FileStream fsw = new FileStream(MainForm.settings.outputPath + Path.DirectorySeparatorChar + fi[i].Name, FileMode.OpenOrCreate);
                                        BinaryWriter bw = new BinaryWriter(fsw);

                                        for (int c = 0; c < count; c++)
                                        {
                                            fs = new FileStream(fi[i].FullName, FileMode.Open);
                                            br = new BinaryReader(fs);

                                            xproFormat format = getData(fs, tp[c].offset);
                                            extractXPR0(fi[i].Name, format, c);

                                            br.Close();
                                            fs.Close();
                                        }

                                        bw.Close();
                                        fsw.Close();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
