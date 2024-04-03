using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Shrek_2_team_action_tools
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            MainForm.settings.ASCII = Convert.ToInt32(asciiNumeric.Value);
            MainForm.settings.inputPath = inputTextBox.Text;
            MainForm.settings.outputPath = outputTextBox.Text;

            Settings.Save(MainForm.settings);
            Close();
        }

        private void inputBrowseBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if(fbd.ShowDialog() == DialogResult.OK)
            {
                inputTextBox.Text = fbd.SelectedPath;
            }
        }

        private void outputBrowseBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();


            if (fbd.ShowDialog() == DialogResult.OK)
            {
                outputTextBox.Text = fbd.SelectedPath;
            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            asciiNumeric.Value = MainForm.settings.ASCII;
            inputTextBox.Text = MainForm.settings.inputPath;
            outputTextBox.Text = MainForm.settings.outputPath;
        }
    }
}
