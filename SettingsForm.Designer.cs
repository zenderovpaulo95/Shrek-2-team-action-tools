namespace Shrek_2_team_action_tools
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.inputBrowseBtn = new System.Windows.Forms.Button();
            this.outputBrowseBtn = new System.Windows.Forms.Button();
            this.asciiNumeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.asciiNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input folder:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Output folder:";
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(101, 38);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(315, 20);
            this.inputTextBox.TabIndex = 2;
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(101, 81);
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.Size = new System.Drawing.Size(315, 20);
            this.outputTextBox.TabIndex = 3;
            // 
            // inputBrowseBtn
            // 
            this.inputBrowseBtn.Location = new System.Drawing.Point(446, 36);
            this.inputBrowseBtn.Name = "inputBrowseBtn";
            this.inputBrowseBtn.Size = new System.Drawing.Size(38, 23);
            this.inputBrowseBtn.TabIndex = 4;
            this.inputBrowseBtn.Text = "...";
            this.inputBrowseBtn.UseVisualStyleBackColor = true;
            this.inputBrowseBtn.Click += new System.EventHandler(this.inputBrowseBtn_Click);
            // 
            // outputBrowseBtn
            // 
            this.outputBrowseBtn.Location = new System.Drawing.Point(446, 79);
            this.outputBrowseBtn.Name = "outputBrowseBtn";
            this.outputBrowseBtn.Size = new System.Drawing.Size(38, 23);
            this.outputBrowseBtn.TabIndex = 5;
            this.outputBrowseBtn.Text = "...";
            this.outputBrowseBtn.UseVisualStyleBackColor = true;
            this.outputBrowseBtn.Click += new System.EventHandler(this.outputBrowseBtn_Click);
            // 
            // asciiNumeric
            // 
            this.asciiNumeric.Location = new System.Drawing.Point(119, 144);
            this.asciiNumeric.Maximum = new decimal(new int[] {
            1258,
            0,
            0,
            0});
            this.asciiNumeric.Minimum = new decimal(new int[] {
            1250,
            0,
            0,
            0});
            this.asciiNumeric.Name = "asciiNumeric";
            this.asciiNumeric.ReadOnly = true;
            this.asciiNumeric.Size = new System.Drawing.Size(77, 20);
            this.asciiNumeric.TabIndex = 6;
            this.asciiNumeric.Value = new decimal(new int[] {
            1250,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Encoding";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(53, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Windows - ";
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(253, 132);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 9;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(362, 132);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 10;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 177);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.asciiNumeric);
            this.Controls.Add(this.outputBrowseBtn);
            this.Controls.Add(this.inputBrowseBtn);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.asciiNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Button inputBrowseBtn;
        private System.Windows.Forms.Button outputBrowseBtn;
        private System.Windows.Forms.NumericUpDown asciiNumeric;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button cancelBtn;
    }
}