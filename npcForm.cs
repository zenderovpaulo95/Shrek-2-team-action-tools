using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace Shrek_2_team_action_tools
{
    public delegate void ReportHandler(string report);
    public partial class npcForm : Form
    {
        public npcForm()
        {
            InitializeComponent();
        }

        void AddNewReport(string report)
        {
            if (listBoxNPC.InvokeRequired)
            {
                listBoxNPC.Invoke(new ReportHandler(AddNewReport), report);
            }
            else
            {
                listBoxNPC.Items.Add(report);
                listBoxNPC.SelectedIndex = listBoxNPC.Items.Count - 1;
                listBoxNPC.SelectedIndex = -1;
            }
        }

        public struct TextStruct
        {
            public int textOffset;
            public int textLength;
            public string originalText;
            public string translatedText;
        }

        public class NPCText
        {
            public int count;
            public int unknown;
            public int sizeBlock;
            public uint offsetBlock;
            public TextStruct[] texts;
            public byte[] block;

            public NPCText() { }
        }

        private void Extract(FileInfo fi)
        {
            if(File.Exists(fi.FullName))
            {
                FileStream fs = new FileStream(fi.FullName, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);

                try
                {
                    NPCText npc = new NPCText();

                    npc.count = br.ReadInt32();
                    npc.unknown = br.ReadInt32();
                    npc.sizeBlock = br.ReadInt32();
                    npc.offsetBlock = br.ReadUInt32();

                    npc.texts = new TextStruct[npc.count];

                    var tmp_pos = br.BaseStream.Position;

                    for (int i = 0; i < npc.count; i++)
                    {
                        npc.texts[i].textLength = br.ReadInt32();
                        npc.texts[i].textOffset = br.ReadInt32();
                        
                        tmp_pos = br.BaseStream.Position;

                        br.BaseStream.Seek(npc.texts[i].textOffset, SeekOrigin.Begin);

                        byte[] data = br.ReadBytes(npc.texts[i].textLength);
                        npc.texts[i].originalText = Encoding.GetEncoding(MainForm.settings.ASCII).GetString(data);

                        if (npc.texts[i].originalText.Contains('\r'))
                        {
                            npc.texts[i].originalText = npc.texts[i].originalText.Replace("\r", "\\r");
                        }
                        if(npc.texts[i].originalText.Contains("\n"))
                        {
                            npc.texts[i].originalText = npc.texts[i].originalText.Replace("\n", "\\n");
                        }
                        if (OtherMethods.hasSpecificChars(npc.texts[i].originalText))
                        {
                            for(int c = 0; c < npc.texts[i].originalText.Length; c++)
                            {
                                if (npc.texts[i].originalText[c] < ' ')
                                {
                                    int b = Convert.ToInt16(npc.texts[i].originalText[c]);

                                    string rb = "\\x" + b.ToString("X2");

                                    npc.texts[i].originalText = npc.texts[i].originalText.Replace(npc.texts[i].originalText[c].ToString(), rb);
                                }
                            }
                        }

                        npc.texts[i].translatedText = npc.texts[i].originalText;

                        br.BaseStream.Seek(tmp_pos, SeekOrigin.Begin);
                    }

                    br.BaseStream.Seek(npc.offsetBlock, SeekOrigin.Begin);
                    npc.block = br.ReadBytes(npc.sizeBlock);

                    br.Close();
                    fs.Close();


                    if (File.Exists(MainForm.settings.outputPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 3, 3) + "txt")) File.Delete(MainForm.settings.outputPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 3, 3) + "txt");
                    fs = new FileStream(MainForm.settings.outputPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 3, 3) + "txt", FileMode.CreateNew);
                    
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    
                    for(int i = 0; i < npc.texts.Length; i++)
                    {
                        sw.Write((i + 1).ToString() + "\r\n");
                        sw.Write("original=" + npc.texts[i].originalText + "\r\n");
                        sw.Write("translated=" + npc.texts[i].translatedText);

                        if(i + 1 < npc.texts.Length)
                        {
                            sw.Write("\r\n");
                        }
                    }

                    sw.Close();
                    fs.Close();

                    AddNewReport("File " + fi.Name + " successfully extracted");
                }
                catch 
                {
                    if(br != null) br.Close();
                    if (fs != null) fs.Close();
                    AddNewReport("Unknown error with file " + fi.Name + ". Please check file.");
                }
            }
        }

        private void extractBtn_Click(object sender, EventArgs e)
        {
            if(Directory.Exists(MainForm.settings.inputPath) && Directory.Exists(MainForm.settings.outputPath))
            {
                listBoxNPC.Items.Clear();

                DirectoryInfo di = new DirectoryInfo(MainForm.settings.inputPath);

                FileInfo[] fi = di.GetFiles("*.npc", SearchOption.AllDirectories);
                

                for(int i = 0; i < fi.Length; i++)
                {
                    Extract(fi[i]);
                }
            }
        }
    }
}
