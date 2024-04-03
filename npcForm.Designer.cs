namespace Shrek_2_team_action_tools
{
    partial class npcForm
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
            this.extractBtn = new System.Windows.Forms.Button();
            this.importBtn = new System.Windows.Forms.Button();
            this.listBoxNPC = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // extractBtn
            // 
            this.extractBtn.Location = new System.Drawing.Point(178, 29);
            this.extractBtn.Name = "extractBtn";
            this.extractBtn.Size = new System.Drawing.Size(75, 23);
            this.extractBtn.TabIndex = 0;
            this.extractBtn.Text = "Extract";
            this.extractBtn.UseVisualStyleBackColor = true;
            this.extractBtn.Click += new System.EventHandler(this.extractBtn_Click);
            // 
            // importBtn
            // 
            this.importBtn.Location = new System.Drawing.Point(541, 29);
            this.importBtn.Name = "importBtn";
            this.importBtn.Size = new System.Drawing.Size(75, 23);
            this.importBtn.TabIndex = 1;
            this.importBtn.Text = "Import";
            this.importBtn.UseVisualStyleBackColor = true;
            // 
            // listBoxNPC
            // 
            this.listBoxNPC.FormattingEnabled = true;
            this.listBoxNPC.Location = new System.Drawing.Point(13, 71);
            this.listBoxNPC.Name = "listBoxNPC";
            this.listBoxNPC.Size = new System.Drawing.Size(775, 368);
            this.listBoxNPC.TabIndex = 2;
            // 
            // npcForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listBoxNPC);
            this.Controls.Add(this.importBtn);
            this.Controls.Add(this.extractBtn);
            this.Name = "npcForm";
            this.Text = "NPC text extractor/importer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button extractBtn;
        private System.Windows.Forms.Button importBtn;
        private System.Windows.Forms.ListBox listBoxNPC;
    }
}