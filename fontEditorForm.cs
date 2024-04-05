using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        struct texture
        {
            public int width;
            public int height;
            public int textureSize; //After copying 1 byte and recalculations
            public byte[] data;
        }

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
                FileStream fs = new FileStream(ofd.FileName, FileMode.Open);
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
                    
                    tmp = br.ReadBytes(1);
                    font.coordinates[i].width = Convert.ToInt16(tmp[0]);
                    tmp = br.ReadBytes(1);
                    font.coordinates[i].xAdvance = Convert.ToInt16(tmp[0]);
                    tmp = br.ReadBytes(1);
                    font.coordinates[i].xOffset = Convert.ToInt16(tmp[0]);
                    tmp = br.ReadBytes(1);
                    font.coordinates[i].yOffset = Convert.ToInt16(tmp[0]);
                    font.coordinates[i].height = Convert.ToInt16(font.height);
                }

                br.BaseStream.Seek(font.textureOffset, SeekOrigin.Begin);

                font.texData.data = br.ReadBytes(font.texData.textureSize);

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
            }
        }
    }
}
