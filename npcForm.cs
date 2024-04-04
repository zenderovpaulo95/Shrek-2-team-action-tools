using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

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

        private byte[] fillPadded(string str, int paddedSize)
        {
            byte[] tmp = new byte[paddedSize];
            byte[] binStr = Encoding.GetEncoding(MainForm.settings.ASCII).GetBytes(str);
            Array.Copy(binStr, 0, tmp, 0, binStr.Length);

            for(int i = binStr.Length; i < paddedSize; i++)
            {
                tmp[i] = (byte)'U';
            }

            return tmp;
        }

        private int padSize(int size, int pad)
        {
            while (size % pad != 0) size++;
            return size;
        }

        private NPCText readNPC(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

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
                if (npc.texts[i].originalText.Contains("\n"))
                {
                    npc.texts[i].originalText = npc.texts[i].originalText.Replace("\n", "\\n");
                }
                if (OtherMethods.hasSpecificChars(npc.texts[i].originalText))
                {
                    for (int c = 0; c < npc.texts[i].originalText.Length; c++)
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

            return npc;
        }

        private void Extract(FileInfo fi)
        {
            if(File.Exists(fi.FullName))
            {
                try
                {
                    NPCText npc = readNPC(fi.FullName);

                    if (File.Exists(MainForm.settings.outputPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 3, 3) + "txt")) File.Delete(MainForm.settings.outputPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 3, 3) + "txt");
                    FileStream fs = new FileStream(MainForm.settings.outputPath + Path.DirectorySeparatorChar + fi.Name.Remove(fi.Name.Length - 3, 3) + "txt", FileMode.CreateNew);
                    
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
                    AddNewReport("Unknown error with file " + fi.Name + ". Please check file.");
                }
            }
        }

        private void Import(FileInfo npcFI, FileInfo txtFI)
        {
            NPCText npc = readNPC(npcFI.FullName);

            string[] txts = File.ReadAllLines(txtFI.FullName);
            byte[] tmp = null;

            for (int i = 0; i < txts.Length; i += 3)
            {
                try
                {
                    int count = Convert.ToInt32(txts[i]);
                    string translated = txts[i + 2].Substring(11, txts[i + 2].Length - 11);

                    if(translated.Contains("\\r"))
                    {
                        translated = translated.Replace("\\r", "\r");
                    }
                    if(translated.Contains("\\n"))
                    {
                        translated = translated.Replace("\\n", "\n");
                    }
                    if(translated.IndexOf("\\x") != -1)
                    {
                        int index = 0;
                        while (index < translated.Length && index != -1)
                        {
                            index = translated.IndexOf("\\x", index);
                            if (index != -1)
                            {
                                translated = translated.Remove(index, 2);
                                string subVal = "0x" + translated.Substring(index, 2);
                                int val = Convert.ToInt32(subVal, 16);
                                translated = translated.Remove(index, 2);
                                tmp = BitConverter.GetBytes(val);
                                translated = translated.Substring(0, index) + (char)tmp[0] + translated.Substring(index, translated.Length - index);
                            }
                        }
                    }

                    if(count > 0 && count <= npc.count)
                    {
                        npc.texts[count - 1].translatedText = translated;
                        tmp = Encoding.GetEncoding(MainForm.settings.ASCII).GetBytes(npc.texts[count - 1].translatedText);
                        npc.texts[count - 1].textLength = tmp.Length;
                    }
                }
                catch
                {
                    AddNewReport("Something wrong with import. Please check text file or check npc file for correct strings.");
                }
            }

            for(int i = 0; i < npc.count; i++)
            {
                if(i + 1 < npc.count)
                {
                    int tmpSize = padSize(npc.texts[i].textLength + 1, 4);
                    npc.texts[i + 1].textOffset = npc.texts[i].textOffset + tmpSize;
                }
            }

            npc.offsetBlock = (uint)(npc.texts[npc.count - 1].textOffset + padSize(npc.texts[npc.count - 1].textLength + 1, 16));

            if (File.Exists(MainForm.settings.outputPath + Path.DirectorySeparatorChar + npcFI.Name)) File.Delete(MainForm.settings.outputPath + Path.DirectorySeparatorChar + npcFI.Name);

            FileStream fs = new FileStream(MainForm.settings.outputPath + Path.DirectorySeparatorChar + npcFI.Name, FileMode.CreateNew);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(npc.count);
            bw.Write(npc.unknown);
            bw.Write(npc.sizeBlock);
            bw.Write(npc.offsetBlock);
            
            for(int i = 0; i < npc.count; i++)
            {
                bw.Write(npc.texts[i].textLength);
                bw.Write(npc.texts[i].textOffset);
            }

            for(int i = 0; i < npc.count - 1; i++)
            {
                tmp = fillPadded(npc.texts[i].translatedText + "\0", padSize(npc.texts[i].translatedText.Length + 1, 4));
                bw.Write(tmp);
            }

            tmp = fillPadded(npc.texts[npc.count - 1].translatedText + "\0", padSize(npc.texts[npc.count - 1].translatedText.Length + 1, 16));
            bw.Write(tmp);

            bw.Write(npc.block);

            bw.Close();
            fs.Close();

            AddNewReport("File " + npcFI.Name + " successfully imported.");
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

        private void importBtn_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(MainForm.settings.inputPath);
            FileInfo[] npcFI = di.GetFiles("*.npc", SearchOption.AllDirectories);
            FileInfo[] txtFI = di.GetFiles("*.txt", SearchOption.AllDirectories);

            if(npcFI.Length > 0 && txtFI.Length > 0)
            {
                listBoxNPC.Items.Clear();

                for (int i = 0; i < npcFI.Length; i++)
                {
                    for(int j = 0; j < txtFI.Length; j++)
                    {
                        if (npcFI[i].Name.Remove(npcFI[i].Name.Length - 3, 3) + "txt" == txtFI[j].Name)
                        {
                            Import(npcFI[i], txtFI[j]);
                        }
                    }
                }
            }
        }
    }
}
