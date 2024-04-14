namespace Shrek_2_team_action_tools
{
    partial class resForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(resForm));
            this.extractBtn = new System.Windows.Forms.Button();
            this.importBtn = new System.Windows.Forms.Button();
            this.listBoxRES = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // extractBtn
            // 
            this.extractBtn.Location = new System.Drawing.Point(121, 22);
            this.extractBtn.Name = "extractBtn";
            this.extractBtn.Size = new System.Drawing.Size(75, 23);
            this.extractBtn.TabIndex = 0;
            this.extractBtn.Text = "Export";
            this.extractBtn.UseVisualStyleBackColor = true;
            this.extractBtn.Click += new System.EventHandler(this.extractBtn_Click);
            // 
            // importBtn
            // 
            this.importBtn.Location = new System.Drawing.Point(527, 22);
            this.importBtn.Name = "importBtn";
            this.importBtn.Size = new System.Drawing.Size(75, 23);
            this.importBtn.TabIndex = 1;
            this.importBtn.Text = "Import";
            this.importBtn.UseVisualStyleBackColor = true;
            this.importBtn.Click += new System.EventHandler(this.importBtn_Click);
            // 
            // listBoxRES
            // 
            this.listBoxRES.FormattingEnabled = true;
            this.listBoxRES.Location = new System.Drawing.Point(12, 65);
            this.listBoxRES.Name = "listBoxRES";
            this.listBoxRES.Size = new System.Drawing.Size(776, 368);
            this.listBoxRES.TabIndex = 2;
            // 
            // resForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listBoxRES);
            this.Controls.Add(this.importBtn);
            this.Controls.Add(this.extractBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "resForm";
            this.Text = "RES/TPK extractor/importer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button extractBtn;
        private System.Windows.Forms.Button importBtn;
        private System.Windows.Forms.ListBox listBoxRES;
    }
}