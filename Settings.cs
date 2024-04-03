using System;
using System.Xml;
using System.Xml.Serialization;

namespace Shrek_2_team_action_tools
{
    [Serializable()]
    public class Settings
    {
        public static void Save(Settings settings)
        {
            string xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + "config.xml";
            XmlSerializer xmlS = new XmlSerializer(typeof(Settings));
            System.IO.TextWriter xmlW = new System.IO.StreamWriter(xmlPath);
            xmlS.Serialize(xmlW, settings);
            xmlW.Flush();
            xmlW.Close();
        }

        public static void Load(Settings settings)
        {
            string xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + "config.xml";
            XmlReader xmlReader = new XmlTextReader(xmlPath);
            XmlSerializer xmlS = new XmlSerializer(typeof(Settings));
            MainForm.settings = (Settings)xmlS.Deserialize(xmlReader);
            xmlReader.Close();
        }

        private int _ascii;
        private string _inputPath;
        private string _outputPath;

        [XmlAttribute("ASCII")]
        public int ASCII
        {
            get
            {
                return _ascii;
            }

            set
            {
                _ascii = value;
            }
        }

        [XmlAttribute("inputPath")]
        public string inputPath
        {
            get
            {
                return _inputPath;
            }

            set
            {
                _inputPath = value;
            }
        }

        [XmlAttribute("outputPath")]
        public string outputPath
        {
            get
            {
                return _outputPath;
            }

            set { _outputPath = value; }
        }

        public Settings() { }

        public Settings(int _ascii, string _inputPath, string _outputPath)
        {
            this.ASCII = _ascii;
            this.inputPath = _inputPath;
            this.outputPath = _outputPath;
        }
    }
}
