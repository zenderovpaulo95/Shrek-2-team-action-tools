using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            public int width;
            public int height;
            public byte[] data;
        }

        private xproFormat getData(string fileName)
        {
            xproFormat format = new xproFormat();

            FileStream fs = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            br.BaseStream.Seek(4, SeekOrigin.Begin);
            format.fileSize = br.ReadInt32();
            format.textureOffset = br.ReadUInt32();
            br.BaseStream.Seek(30, SeekOrigin.Begin);
            byte[] tmp = br.ReadBytes(1);
            byte[] tmpVal = new byte[4];
            tmpVal[0] = tmp[0];

            format.height = BitConverter.ToInt32(tmpVal, 0) == 0 ? -1 : (BitConverter.ToInt32(tmpVal, 0) + 1) * 16;

            tmp = br.ReadBytes(1);
            tmpVal = new byte[4];
            tmpVal[0] = tmp[0];

            format.width = BitConverter.ToInt32(tmpVal, 0) == 0 ? -1 : (BitConverter.ToInt32(tmpVal, 0) + 1) * 16;

            br.BaseStream.Seek(format.textureOffset, SeekOrigin.Begin);
            format.data = br.ReadBytes(format.fileSize - (int)format.textureOffset);

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

        private void extractXPR0(string fileName, xproFormat format)
        {
            if (format.width != -1 && format.height != -1)
            {
                Bitmap bmp = new Bitmap(format.width, format.height, PixelFormat.Format32bppArgb);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                IntPtr bmpPtr = bmpData.Scan0;
                Marshal.Copy(format.data, 0, bmpPtr, format.data.Length);
                bmp.UnlockBits(bmpData);

                bmp.Save(MainForm.settings.outputPath + Path.DirectorySeparatorChar + fileName.Remove(fileName.Length - 3, 3) + "png");

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
            FileInfo[] fi = di.GetFiles("*.res", SearchOption.AllDirectories);

            if(fi.Length > 0)
            {
                listBoxRES.Items.Clear();

                for(int i = 0; i < fi.Length; i++)
                {
                    int type = getType(fi[i].FullName);

                    switch(type)
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
            }
        }
    }
}
