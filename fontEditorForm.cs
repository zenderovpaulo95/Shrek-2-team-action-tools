using ImageMagick;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Shrek_2_team_action_tools
{
    public partial class fontEditorForm : Form
    {
        public fontEditorForm()
        {
            InitializeComponent();
        }

        FontClass font;
        string fontName;

        struct texture
        {
            public int width;
            public int height;
            public int textureSize; //After copying 1 byte and recalculations
            public byte[] data;
        }

        byte[] header = { 0x44, 0x44, 0x53, 0x20, 0x7C, 0x00, 0x00, 0x00, 0x0F, 0x10, 0x00, 0x00, 0xE0, 0x01, 0x00, 0x00, 0x80, 0x02, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x47, 0x49, 0x4D, 0x50, 0x2D, 0x44, 0x44, 0x53, 0x5C, 0x09, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0xF0, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        struct fntCoordinates
        {
            public short x; //2 bytes
            public short y; //2 bytes
            //next values make from 1 byte
            public short width; 
            public short height; //Add from byte height
            public short xOffset;
            public short yOffset;
            public short xAdvance;
        }

        class FontClass
        {
            public short unknown1;
            public short unknown2;
            public uint textureOffset;
            public byte[] someData; //8 bytes of some data
            public ushort unknown3;
            public int coordsCount; //After copying 1 byte and recalculations
            public byte unknown4;
            public short firstCharNum; //After coping 1 byte
            public byte lineHeight;
            public byte height;

            public fntCoordinates[] coordinates;

            public texture texData;

            public FontClass() { }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Font file (*.fnt) | *.fnt";

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                fontName = ofd.FileName;
                FileStream fs = new FileStream(fontName, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                font = new FontClass();

                font.unknown1 = br.ReadInt16();
                font.unknown2 = br.ReadInt16();
                font.textureOffset = br.ReadUInt32();
                font.someData = br.ReadBytes(8);
                font.unknown3 = br.ReadUInt16();
                
                byte[] tmp = br.ReadBytes(1);
                byte[] tmpVal = new byte[4];
                tmpVal[0] = tmp[0];
                font.texData.textureSize = (BitConverter.ToInt32(tmpVal, 0) + 1) * 0x1000;

                font.texData.width = 128; //default size;
                font.texData.height = font.texData.textureSize / 2 / font.texData.width;
                
                font.unknown4 = br.ReadByte();
                
                tmp = br.ReadBytes(1);
                tmpVal = new byte[4];
                tmpVal[0] = tmp[0];
                font.coordsCount = BitConverter.ToInt32(tmpVal, 0);

                tmp = br.ReadBytes(1);
                tmpVal = new byte[2];
                tmpVal[0] = tmp[0];
                font.firstCharNum = BitConverter.ToInt16(tmpVal, 0);

                font.lineHeight = br.ReadByte();
                font.height = br.ReadByte();

                font.coordinates = new fntCoordinates[font.coordsCount];

                for(int i = 0; i < font.coordsCount; i++)
                {
                    font.coordinates[i].x = br.ReadInt16();
                    font.coordinates[i].y = br.ReadInt16();

                    sbyte b = br.ReadSByte();
                    font.coordinates[i].width = Convert.ToInt16(b);
                    b = br.ReadSByte();
                    font.coordinates[i].xAdvance = Convert.ToInt16(b);
                    b = br.ReadSByte();
                    font.coordinates[i].xOffset = Convert.ToInt16(b);
                    b = br.ReadSByte();
                    font.coordinates[i].yOffset = Convert.ToInt16(b);
                    font.coordinates[i].height = Convert.ToInt16(font.height);
                }

                br.BaseStream.Seek(font.textureOffset, SeekOrigin.Begin);

                byte[] tmpData = br.ReadBytes(font.texData.textureSize);

                tmp = BitConverter.GetBytes(font.texData.width);
                Array.Copy(tmp, 0, header, 16, tmp.Length);

                tmp = BitConverter.GetBytes(font.texData.height);
                Array.Copy(tmp, 0, header, 12, tmp.Length);

                tmp = BitConverter.GetBytes(font.texData.width * 2);
                Array.Copy(tmp, 0, header, 20, tmp.Length);

                font.texData.data = new byte[128 + tmpData.Length];
                Array.Copy(header, 0, font.texData.data, 0, header.Length);
                Array.Copy(tmpData, 0, font.texData.data, 128, tmpData.Length);
                
                tmpData = null;

                br.Close();
                fs.Close();

                textureGridView.ColumnCount = 3;
                textureGridView.RowCount = 1;
                textureGridView.Columns[0].HeaderText = "Width";
                textureGridView.Columns[1].HeaderText = "Height";
                textureGridView.Columns[2].HeaderText = "Texture size";
                textureGridView[0, 0].Value = font.texData.width;
                textureGridView[1, 0].Value = font.texData.height;
                textureGridView[2, 0].Value = font.texData.textureSize;


                textureGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                textureGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                textureGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                coordinatesGridView.ColumnCount = 8;
                coordinatesGridView.RowCount = font.coordsCount;
                coordinatesGridView.Columns[0].HeaderText = "Char";
                coordinatesGridView.Columns[1].HeaderText = "x";
                coordinatesGridView.Columns[2].HeaderText = "y";
                coordinatesGridView.Columns[3].HeaderText = "Width";
                coordinatesGridView.Columns[4].HeaderText = "Height";
                coordinatesGridView.Columns[5].HeaderText = "x offset";
                coordinatesGridView.Columns[6].HeaderText = "x advance";
                coordinatesGridView.Columns[7].HeaderText = "y offset";

                for(int i = 0; i < 8; i++)
                {
                    coordinatesGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                for (int i = 0; i < font.coordsCount; i++)
                {
                    int code = font.firstCharNum + i;
                    coordinatesGridView.Rows[i].HeaderCell.Value = Convert.ToString(code);
                    coordinatesGridView[0, i].Value = Encoding.GetEncoding(MainForm.settings.ASCII).GetString(BitConverter.GetBytes(code)).Replace("\0", "");
                    coordinatesGridView[1, i].Value = Convert.ToString(font.coordinates[i].x);
                    coordinatesGridView[2, i].Value = Convert.ToString(font.coordinates[i].y);
                    coordinatesGridView[3, i].Value = Convert.ToString(font.coordinates[i].width);
                    coordinatesGridView[4, i].Value = Convert.ToString(font.coordinates[i].height);
                    coordinatesGridView[5, i].Value = Convert.ToString(font.coordinates[i].xOffset);
                    coordinatesGridView[6, i].Value = Convert.ToString(font.coordinates[i].xAdvance);
                    coordinatesGridView[7, i].Value = Convert.ToString(font.coordinates[i].yOffset);
                }

                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                exportBtn.Enabled = true;
                importBtn.Enabled = true;
            }
        }

        private void fontEditorForm_Load(object sender, EventArgs e)
        {
            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            exportBtn.Enabled = false;
            importBtn.Enabled = false;
        }

        private void exportBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if(fbd.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(fontName);

                if (File.Exists(fbd.SelectedPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 4, 4) + ".png")) File.Delete(fbd.SelectedPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 4, 4) + ".png");
                if (File.Exists(fbd.SelectedPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 4, 4) + ".fnt")) File.Delete(fbd.SelectedPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 4, 4) + ".fnt");

                using (MemoryStream ms = new MemoryStream(font.texData.data)) 
                {
                    MagickImage image = new MagickImage(font.texData.data);
                    image.Write(fbd.SelectedPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 4, 4) + ".png", MagickFormat.Png);
                    image.Dispose();
                }

                /*Bitmap bmp = new Bitmap(font.texData.width, font.texData.height);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, font.texData.width, font.texData.height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                IntPtr bmpPtr = bmpData.Scan0;
                Marshal.Copy(font.texData.data, 0, bmpPtr, font.texData.data.Length);
                bmp.UnlockBits(bmpData);

                bmp.Save(fbd.SelectedPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 4, 4) + ".png");*/

                FileStream fs = new FileStream(fbd.SelectedPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 4, 4) + ".fnt", FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine("info face=\"" + fi.Name.Remove(fi.Name.Length - 4, 4) + "\" size=" + Convert.ToInt32(font.lineHeight) + " bold=0 italic=0 charset=\"\" unicode=0 stretchH=100 smooth=1 aa=1 padding=0,0,0,0 spacing=1,1 outline=0");
                sw.WriteLine("common lineHeight=" + Convert.ToInt32(font.lineHeight) + " base=" + Convert.ToInt32(font.lineHeight) + " scaleW=" + font.texData.width + " scaleH=" + font.texData.height + " pages=1 packed=0 alphaChnl=0 redChnl=4 greenChnl=4 blueChnl=4");
                sw.WriteLine("page id=0 file=\"" + fi.Name.Remove(fi.Name.Length - 4, 4) + ".png\"");
                sw.WriteLine("chars count=" + font.coordsCount);
                for(int i = 0; i < font.coordsCount; i++)
                {
                    sw.Write("char id=" + Convert.ToString(i + font.firstCharNum) + " x=" + font.coordinates[i].x + " y=" + font.coordinates[i].y + " width=" + font.coordinates[i].width + " height=" + font.coordinates[i].height + " xoffset=" + font.coordinates[i].xOffset + " yoffset=" + font.coordinates[i].yOffset + " xadvance=" + font.coordinates[i].xAdvance + " page=0  chnl=15");
                    if (i + 1 < font.coordsCount) sw.Write("\r\n");
                }
                sw.Close();
                fs.Close();
            }
        }

        private void importBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Font file (*.fnt) | *.fnt";

            if (ofd.ShowDialog() == DialogResult.OK) 
            {
                string test = ofd.FileName.Remove(ofd.FileName.Length - 3, 3) + "png";
                MagickImage image = new MagickImage(test, MagickFormat.Png);
                image.ColorType = ColorType.PaletteAlpha;
                image.SetCompression(CompressionMethod.NoCompression);
                image.Write(ofd.FileName.Remove(ofd.FileName.Length - 3, 3) + "dds", MagickFormat.Dds);
                /*byte[] res = image.ToByteArray(MagickFormat.Dds);
                File.WriteAllBytes(ofd.FileName.Remove(ofd.FileName.Length - 3, 3) + "dds", res);*/
                image.Dispose();
            }
        }
    }
}
