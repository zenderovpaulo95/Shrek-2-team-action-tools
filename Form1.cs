using System;
using System.IO;
using System.Windows.Forms;

namespace Shrek_2_team_action_tools
{
    public partial class MainForm : Form
    {
        public static Settings settings = new Settings(1251, "", "");
        public MainForm()
        {
            InitializeComponent();
        }

        private void settingsBtn_Click(object sender, EventArgs e)
        {
            SettingsForm settings = new SettingsForm();
            settings.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "config.xml"))
            {
                Settings.Load(settings);
            }
        }

        private void npcToolBtn_Click(object sender, EventArgs e)
        {
            npcForm npc = new npcForm();
            npc.Show();
        }

        private void resToolBtn_Click(object sender, EventArgs e)
        {
            resForm res = new resForm();
            res.Show();
        }

        private void fontEditorBtn_Click(object sender, EventArgs e)
        {
            fontEditorForm fontEditor = new fontEditorForm();
            fontEditor.Show();
        }
    }
}
