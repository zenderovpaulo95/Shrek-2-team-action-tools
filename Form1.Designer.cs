namespace Shrek_2_team_action_tools
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.npcToolBtn = new System.Windows.Forms.Button();
            this.resToolBtn = new System.Windows.Forms.Button();
            this.fontEditorBtn = new System.Windows.Forms.Button();
            this.settingsBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // npcToolBtn
            // 
            this.npcToolBtn.Location = new System.Drawing.Point(24, 12);
            this.npcToolBtn.Name = "npcToolBtn";
            this.npcToolBtn.Size = new System.Drawing.Size(259, 23);
            this.npcToolBtn.TabIndex = 0;
            this.npcToolBtn.Text = "NPC extractor/importer";
            this.npcToolBtn.UseVisualStyleBackColor = true;
            this.npcToolBtn.Click += new System.EventHandler(this.npcToolBtn_Click);
            // 
            // resToolBtn
            // 
            this.resToolBtn.Location = new System.Drawing.Point(24, 46);
            this.resToolBtn.Name = "resToolBtn";
            this.resToolBtn.Size = new System.Drawing.Size(259, 23);
            this.resToolBtn.TabIndex = 1;
            this.resToolBtn.Text = "RES/TPK extractor/importer";
            this.resToolBtn.UseVisualStyleBackColor = true;
            this.resToolBtn.Click += new System.EventHandler(this.resToolBtn_Click);
            // 
            // fontEditorBtn
            // 
            this.fontEditorBtn.Location = new System.Drawing.Point(24, 81);
            this.fontEditorBtn.Name = "fontEditorBtn";
            this.fontEditorBtn.Size = new System.Drawing.Size(259, 23);
            this.fontEditorBtn.TabIndex = 2;
            this.fontEditorBtn.Text = "Font editor";
            this.fontEditorBtn.UseVisualStyleBackColor = true;
            this.fontEditorBtn.Click += new System.EventHandler(this.fontEditorBtn_Click);
            // 
            // settingsBtn
            // 
            this.settingsBtn.Location = new System.Drawing.Point(24, 120);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(258, 23);
            this.settingsBtn.TabIndex = 3;
            this.settingsBtn.Text = "Settings";
            this.settingsBtn.UseVisualStyleBackColor = true;
            this.settingsBtn.Click += new System.EventHandler(this.settingsBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 158);
            this.Controls.Add(this.settingsBtn);
            this.Controls.Add(this.fontEditorBtn);
            this.Controls.Add(this.resToolBtn);
            this.Controls.Add(this.npcToolBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Shrek 2 team action tools";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button npcToolBtn;
        private System.Windows.Forms.Button resToolBtn;
        private System.Windows.Forms.Button fontEditorBtn;
        private System.Windows.Forms.Button settingsBtn;
    }
}

