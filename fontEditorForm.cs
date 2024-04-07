using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

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
        bool edited;

        struct texture
        {
            public int width;
            public int height;
            public int textureSize; //After copying 1 byte and recalculations
            public byte[] data;
        }

        struct fntCoordinates
        {
            public short charNum;
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

        private void textureTable(bool edited)
        {
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

            for (int j = 0; j < 3; j++)
            {
                textureGridView[j, 0].Style.BackColor = edited ? Color.Bisque : Color.White;
            }
        }

        private void coordinatesTable(bool edited)
        {
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

            for (int i = 0; i < 8; i++)
            {
                coordinatesGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            for (int i = 0; i < font.coordsCount; i++)
            {
                coordinatesGridView.Rows[i].HeaderCell.Value = Convert.ToString(font.coordinates[i].charNum);
                coordinatesGridView[0, i].Value = Encoding.GetEncoding(MainForm.settings.ASCII).GetString(BitConverter.GetBytes(font.coordinates[i].charNum)).Replace("\0", "");
                coordinatesGridView[1, i].Value = Convert.ToString(font.coordinates[i].x);
                coordinatesGridView[2, i].Value = Convert.ToString(font.coordinates[i].y);
                coordinatesGridView[3, i].Value = Convert.ToString(font.coordinates[i].width);
                coordinatesGridView[4, i].Value = Convert.ToString(font.coordinates[i].height);
                coordinatesGridView[5, i].Value = Convert.ToString(font.coordinates[i].xOffset);
                coordinatesGridView[6, i].Value = Convert.ToString(font.coordinates[i].xAdvance);
                coordinatesGridView[7, i].Value = Convert.ToString(font.coordinates[i].yOffset);

                for(int j = 0; j < 8; j++)
                {
                    coordinatesGridView[j, i].Style.BackColor = edited ? Color.Bisque : Color.White;
                }
            }
        }

        private void SaveFont(FontClass font, string fontName)
        {
            if(File.Exists(fontName)) File.Delete(fontName);

            FileStream fs = new FileStream(fontName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(font.unknown1);
            bw.Write(font.unknown2);
            bw.Write(font.textureOffset);
            bw.Write(font.someData);
            bw.Write(font.unknown3);
            byte[] tmp = BitConverter.GetBytes((font.texData.textureSize / 0x1000) - 1);
            bw.Write(tmp[0]);
            bw.Write(font.unknown4);
            tmp = BitConverter.GetBytes((font.coordsCount));
            bw.Write(tmp[0]);
            tmp = BitConverter.GetBytes(font.firstCharNum);
            bw.Write(tmp[0]);
            bw.Write(font.lineHeight);
            bw.Write(font.height);

            for(int i = 0; i < font.coordsCount; i++)
            {
                bw.Write(font.coordinates[i].x);
                bw.Write(font.coordinates[i].y);
                tmp = BitConverter.GetBytes(font.coordinates[i].width);
                bw.Write(tmp[0]);
                tmp = BitConverter.GetBytes(font.coordinates[i].xAdvance);
                bw.Write(tmp[0]);
                tmp = BitConverter.GetBytes(font.coordinates[i].xOffset);
                bw.Write(tmp[0]);
                tmp = BitConverter.GetBytes(font.coordinates[i].yOffset);
                bw.Write(tmp[0]);
            }

            tmp = new byte[font.textureOffset - (24 + (font.coordsCount * 8))];
            tmp = OtherMethods.fillPadded(tmp, tmp.Length, 0);
            bw.Write(tmp);
            bw.Write(font.texData.data);

            bw.Close();
            fs.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Font file (*.fnt) | *.fnt";

            if (ofd.ShowDialog() == DialogResult.OK)
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

                for (int i = 0; i < font.coordsCount; i++)
                {
                    font.coordinates[i].charNum = (short)(i + font.firstCharNum);
                    font.coordinates[i].x = br.ReadInt16();
                    font.coordinates[i].y = br.ReadInt16();

                    byte b = br.ReadByte();
                    byte[] res = new byte[2];
                    res[0] = b;
                    if (b > 127) res[1] = 0xff;
                    font.coordinates[i].width = BitConverter.ToInt16(res, 0);

                    b = br.ReadByte();
                    res = new byte[2];
                    res[0] = b;
                    if (b > 127) res[1] = 0xff;
                    font.coordinates[i].xAdvance = BitConverter.ToInt16(res, 0);

                    b = br.ReadByte();
                    res = new byte[2];
                    res[0] = b;
                    if (b > 127) res[1] = 0xff;
                    font.coordinates[i].xOffset = BitConverter.ToInt16(res, 0);

                    b = br.ReadByte();
                    res = new byte[2];
                    res[0] = b;
                    if (b > 127) res[1] = 0xff;
                    font.coordinates[i].yOffset = BitConverter.ToInt16(res, 0);
                    font.coordinates[i].height = Convert.ToInt16(font.height);
                }

                br.BaseStream.Seek(font.textureOffset, SeekOrigin.Begin);

                font.texData.data = br.ReadBytes(font.texData.textureSize);

                br.Close();
                fs.Close();

                edited = false;

                textureTable(edited);
                coordinatesTable(edited);

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

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(fontName);

                if (File.Exists(fbd.SelectedPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 4, 4) + ".png")) File.Delete(fbd.SelectedPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 4, 4) + ".png");
                if (File.Exists(fbd.SelectedPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 4, 4) + ".fnt")) File.Delete(fbd.SelectedPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 4, 4) + ".fnt");

                font.texData.data = OtherMethods.argb4444ToArgb8888(font.texData.data, font.texData.width, font.texData.height);

                Bitmap bmp = new Bitmap(font.texData.width, font.texData.height);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, font.texData.width, font.texData.height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                IntPtr bmpPtr = bmpData.Scan0;
                Marshal.Copy(font.texData.data, 0, bmpPtr, font.texData.data.Length);
                bmp.UnlockBits(bmpData);

                bmp.Save(fbd.SelectedPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 4, 4) + ".png");

                FileStream fs = new FileStream(fbd.SelectedPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 4, 4) + ".fnt", FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine("info face=\"" + fi.Name.Remove(fi.Name.Length - 4, 4) + "\" size=" + Convert.ToInt32(font.lineHeight) + " bold=0 italic=0 charset=\"\" unicode=0 stretchH=100 smooth=1 aa=1 padding=0,0,0,0 spacing=1,1 outline=0");
                sw.WriteLine("common lineHeight=" + Convert.ToInt32(font.lineHeight) + " base=" + Convert.ToInt32(font.lineHeight) + " scaleW=" + font.texData.width + " scaleH=" + font.texData.height + " pages=1 packed=0 alphaChnl=0 redChnl=4 greenChnl=4 blueChnl=4");
                sw.WriteLine("page id=0 file=\"" + fi.Name.Remove(fi.Name.Length - 4, 4) + ".png\"");
                sw.WriteLine("chars count=" + font.coordsCount);
                for (int i = 0; i < font.coordsCount; i++)
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
                string[] strs = File.ReadAllLines(ofd.FileName);
                List<fntCoordinates> newCoords = new List<fntCoordinates>();
                int count = 0;
                short lineHeight = 0;
                short firstChar = 255;
                short lastChar = 0;

                //Check for xml tags and removing it for comfortable searching needed data (useful for xml fnt files)
                for (int n = 0; n < strs.Length; n++)
                {
                    if ((strs[n].IndexOf('<') >= 0) || (strs[n].IndexOf('<') >= 0 && strs[n].IndexOf('/') > 0))
                    {
                        strs[n] = strs[n].Remove(strs[n].IndexOf('<'), 1);
                        if (strs[n].IndexOf('/') >= 0) strs[n] = strs[n].Remove(strs[n].IndexOf('/'), 1);
                    }
                    if (strs[n].IndexOf('>') >= 0 || (strs[n].IndexOf('/') >= 0 && strs[n + 1].IndexOf('>') > 0))
                    {
                        strs[n] = strs[n].Remove(strs[n].IndexOf('>'), 1);
                        if (strs[n].IndexOf('/') >= 0) strs[n] = strs[n].Remove(strs[n].IndexOf('/'), 1);
                    }
                    if (strs[n].IndexOf('"') >= 0)
                    {
                        while (strs[n].IndexOf('"') >= 0) strs[n] = strs[n].Remove(strs[n].IndexOf('"'), 1);
                    }
                }

                for (int m = 0; m < strs.Length; m++)
                {
                    if (strs[m].ToLower().Contains("common lineheight"))
                    {
                        string[] splitted = strs[m].Split(new char[] { ' ', '=', '\"', ',' });
                        for (int k = 0; k < splitted.Length; k++)
                        {
                            switch (splitted[k].ToLower())
                            {
                                case "lineheight":
                                    lineHeight = Convert.ToInt16(splitted[k + 1]);
                                    break;

                                case "pages":
                                    if (Convert.ToInt16(splitted[k + 1]) != 1)
                                    {
                                        MessageBox.Show("Count textures error", "Count of texture is only one!");
                                        return;
                                    }
                                    break;
                            }
                        }
                    }
                    if (strs[m].Contains("page id"))
                    {
                        string[] splitted = strs[m].Split(new char[] { ' ', '=', '\"', ',' });
                        FileInfo fi = new FileInfo(ofd.FileName);

                        for (int k = 0; k < splitted.Length; k++)
                        {
                            if (splitted[k] == "file") {
                                if (File.Exists(fi.DirectoryName + Path.DirectorySeparatorChar + splitted[k + 1]))
                                {
                                    Image img = Bitmap.FromFile(fi.DirectoryName + Path.DirectorySeparatorChar + splitted[k + 1]);

                                    if (img.Width == 128)
                                    {
                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                                            img.Save(ms, ImageFormat.Bmp);
                                            font.texData.data = ms.ToArray();
                                        }

                                        byte[] tmp = new byte[4];
                                        Array.Copy(font.texData.data, 10, tmp, 0, tmp.Length);
                                        int offset = BitConverter.ToInt32(tmp, 0);

                                        tmp = new byte[font.texData.data.Length - offset];
                                        Array.Copy(font.texData.data, offset, tmp, 0, tmp.Length);

                                        font.texData.data = OtherMethods.argb8888ToArgb4444(tmp, img.Width, img.Height);
                                        font.texData.height = img.Height;
                                        font.texData.textureSize = font.texData.data.Length;
                                        img.Dispose();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Texture error", "Texture width must be equal 128 pixels!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("No png texture", "There is no a PNG texture.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                    }
                    if (strs[m].Contains("chars count"))
                    {
                        string[] splitted = strs[m].Split(new char[] { ' ', '=', '\"', ',' });
                        for (int k = 0; k < splitted.Length; k++)
                        {
                            switch (splitted[k].ToLower())
                            {
                                case "count":
                                    count = Convert.ToInt32(splitted[k + 1]);
                                    break;
                            }
                        }
                    }
                    if (strs[m].Contains("char id"))
                    {
                        string[] splitted = strs[m].Split(new char[] { ' ', '=', '\"', ',' });
                        fntCoordinates tmpcoord = new fntCoordinates();

                        for (int k = 0; k < splitted.Length; k++)
                        {
                            switch (splitted[k].ToLower())
                            {
                                case "id":
                                    tmpcoord.charNum = Convert.ToInt16(splitted[k + 1]);
                                    firstChar = tmpcoord.charNum < firstChar ? tmpcoord.charNum : firstChar;
                                    lastChar = tmpcoord.charNum > lastChar ? tmpcoord.charNum : lastChar;
                                    break;
                                case "x":
                                    tmpcoord.x = Convert.ToInt16(splitted[k + 1]);
                                    break;
                                case "y":
                                    tmpcoord.y = Convert.ToInt16(splitted[k + 1]);
                                    break;
                                case "width":
                                    tmpcoord.width = Convert.ToInt16(splitted[k + 1]);
                                    break;
                                case "height":
                                    tmpcoord.height = Convert.ToInt16(splitted[k + 1]);
                                    break;
                                case "xoffset":
                                    tmpcoord.xOffset = Convert.ToInt16(splitted[k + 1]);
                                    break;
                                case "yoffset":
                                    tmpcoord.yOffset = Convert.ToInt16(splitted[k + 1]);
                                    break;
                                case "xadvance":
                                    tmpcoord.xAdvance = Convert.ToInt16(splitted[k + 1]);
                                    break;
                            }
                        }

                        newCoords.Add(tmpcoord);
                    }
                }

                newCoords.Sort((x, y) => x.charNum.CompareTo(y.charNum));
                newCoords = newCoords.GroupBy(c => c.charNum).Select(j => j.First()).ToList();

                short ch = firstChar;
                int i = 0;

                while (ch < lastChar)
                {
                    if (newCoords[i].charNum != ch)
                    {
                        short tmpCh = newCoords[i].charNum > ch ? ch : newCoords[i].charNum;
                        short tmpMaxCh = newCoords[i].charNum < ch ? ch : newCoords[i].charNum;

                        while (tmpCh != tmpMaxCh)
                        {
                            fntCoordinates tmpCoords = new fntCoordinates();
                            tmpCoords.charNum = tmpCh;
                            tmpCoords.x = 0;
                            tmpCoords.y = (short)(font.texData.height + 10);
                            tmpCoords.width = 0;
                            tmpCoords.xAdvance = 0;
                            tmpCoords.height = (short)font.texData.height;
                            tmpCoords.xOffset = 0;
                            tmpCoords.yOffset = 0;

                            newCoords.Add(tmpCoords);

                            tmpCh++;
                            count++;
                        }

                        ch = tmpCh;
                    }

                    ch++;
                    i++;
                }

                newCoords.Sort((x, y) => x.charNum.CompareTo(y.charNum));
                newCoords = newCoords.GroupBy(c => c.charNum).Select(j => j.First()).ToList();

                font.coordsCount = count < newCoords.Count ? count : newCoords.Count;
                font.lineHeight = Convert.ToByte(lineHeight);
                font.height = Convert.ToByte(lineHeight);
                font.textureOffset = (uint)(OtherMethods.padSize(24 + (8 * font.coordsCount), 16));
                font.firstCharNum = firstChar;

                font.coordinates = newCoords.ToArray();

                newCoords.Clear();

                edited = true;

                textureTable(edited);
                coordinatesTable(edited);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(edited && fontName != "")
            {
                SaveFont(font, fontName);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Font file (*.fnt) | *.fnt";

            if(sfd.ShowDialog() == DialogResult.OK)
            {
                SaveFont(font, sfd.FileName);
            }
        }
    }
}
