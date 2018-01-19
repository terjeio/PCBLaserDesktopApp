namespace PCB_Laser
{
    partial class About
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
            this.okButton = new System.Windows.Forms.Button();
            this.copyright = new System.Windows.Forms.Label();
            this.version = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(107, 69);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            // 
            // copyright
            // 
            this.copyright.Location = new System.Drawing.Point(12, 32);
            this.copyright.Name = "copyright";
            this.copyright.Size = new System.Drawing.Size(216, 24);
            this.copyright.TabIndex = 5;
            this.copyright.Text = "©2014-2018 Io Engineering (Terje Io)";
            // 
            // version
            // 
            this.version.Location = new System.Drawing.Point(12, 9);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(216, 23);
            this.version.TabIndex = 4;
            this.version.Text = "Version 1.04";
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 106);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.copyright);
            this.Controls.Add(this.version);
            this.Name = "About";
            this.Text = "...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label copyright;
        private System.Windows.Forms.Label version;
    }
}