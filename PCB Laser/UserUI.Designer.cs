namespace PCB_Laser
{
    partial class UserUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserUI));
            this.pcb = new System.Windows.Forms.PictureBox();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.zFocus = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.dropInfo = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpBox = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.zOn = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.zPower = new System.Windows.Forms.TrackBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.chkMirror = new System.Windows.Forms.CheckBox();
            this.chkInvert = new System.Windows.Forms.CheckBox();
            this.btnHome = new System.Windows.Forms.Button();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pcb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zFocus)).BeginInit();
            this.grpBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pcb
            // 
            this.pcb.BackColor = System.Drawing.Color.Lavender;
            this.pcb.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pcb.Location = new System.Drawing.Point(11, 27);
            this.pcb.Name = "pcb";
            this.pcb.Size = new System.Drawing.Size(640, 400);
            this.pcb.TabIndex = 0;
            this.pcb.TabStop = false;
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(690, 32);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.ReadOnly = true;
            this.txtWidth.Size = new System.Drawing.Size(115, 20);
            this.txtWidth.TabIndex = 1;
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(690, 58);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.ReadOnly = true;
            this.txtHeight.Size = new System.Drawing.Size(115, 20);
            this.txtHeight.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(660, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "W:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(660, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "H:";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(657, 385);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // zFocus
            // 
            this.zFocus.Location = new System.Drawing.Point(23, 47);
            this.zFocus.Maximum = 100;
            this.zFocus.Name = "zFocus";
            this.zFocus.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.zFocus.Size = new System.Drawing.Size(45, 104);
            this.zFocus.TabIndex = 6;
            this.zFocus.TabStop = false;
            this.zFocus.Value = 50;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Focus:";
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(657, 414);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(156, 20);
            this.txtStatus.TabIndex = 8;
            // 
            // dropInfo
            // 
            this.dropInfo.AutoSize = true;
            this.dropInfo.BackColor = System.Drawing.Color.Lavender;
            this.dropInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dropInfo.Location = new System.Drawing.Point(206, 211);
            this.dropInfo.Name = "dropInfo";
            this.dropInfo.Size = new System.Drawing.Size(287, 13);
            this.dropInfo.TabIndex = 9;
            this.dropInfo.Text = "Drop 1200 dpi .bmp or .png image to expose here";
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(738, 385);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // grpBox
            // 
            this.grpBox.Controls.Add(this.textBox1);
            this.grpBox.Controls.Add(this.zOn);
            this.grpBox.Controls.Add(this.label4);
            this.grpBox.Controls.Add(this.zPower);
            this.grpBox.Controls.Add(this.zFocus);
            this.grpBox.Controls.Add(this.label3);
            this.grpBox.Location = new System.Drawing.Point(668, 105);
            this.grpBox.Name = "grpBox";
            this.grpBox.Size = new System.Drawing.Size(137, 179);
            this.grpBox.TabIndex = 11;
            this.grpBox.TabStop = false;
            this.grpBox.Text = "Laser";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(45, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 11;
            // 
            // zOn
            // 
            this.zOn.AutoSize = true;
            this.zOn.Location = new System.Drawing.Point(23, 156);
            this.zOn.Name = "zOn";
            this.zOn.Size = new System.Drawing.Size(40, 17);
            this.zOn.TabIndex = 10;
            this.zOn.Text = "On";
            this.zOn.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(68, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Power:";
            // 
            // zPower
            // 
            this.zPower.LargeChange = 400;
            this.zPower.Location = new System.Drawing.Point(74, 47);
            this.zPower.Maximum = 4095;
            this.zPower.Minimum = 1500;
            this.zPower.Name = "zPower";
            this.zPower.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.zPower.Size = new System.Drawing.Size(45, 104);
            this.zPower.SmallChange = 100;
            this.zPower.TabIndex = 8;
            this.zPower.TabStop = false;
            this.zPower.Value = 2400;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(657, 290);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(156, 89);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // chkMirror
            // 
            this.chkMirror.AutoSize = true;
            this.chkMirror.Location = new System.Drawing.Point(691, 82);
            this.chkMirror.Name = "chkMirror";
            this.chkMirror.Size = new System.Drawing.Size(52, 17);
            this.chkMirror.TabIndex = 13;
            this.chkMirror.Text = "Mirror";
            this.chkMirror.UseVisualStyleBackColor = true;
            // 
            // chkInvert
            // 
            this.chkInvert.AutoSize = true;
            this.chkInvert.Location = new System.Drawing.Point(752, 82);
            this.chkInvert.Name = "chkInvert";
            this.chkInvert.Size = new System.Drawing.Size(53, 17);
            this.chkInvert.TabIndex = 14;
            this.chkInvert.Text = "Invert";
            this.chkInvert.UseVisualStyleBackColor = true;
            // 
            // btnHome
            // 
            this.btnHome.Location = new System.Drawing.Point(657, 440);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(75, 23);
            this.btnHome.TabIndex = 15;
            this.btnHome.Text = "Home";
            this.btnHome.UseVisualStyleBackColor = true;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(826, 24);
            this.menuStrip.TabIndex = 16;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenuItem,
            this.exitMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutMenuItem.Text = "About...";
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(116, 22);
            this.exitMenuItem.Text = "Exit";
            // 
            // UserUI
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 476);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.chkInvert);
            this.Controls.Add(this.chkMirror);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.grpBox);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.dropInfo);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtHeight);
            this.Controls.Add(this.txtWidth);
            this.Controls.Add(this.pcb);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "UserUI";
            this.Text = "PCB Laser Exposer";
            ((System.ComponentModel.ISupportInitialize)(this.pcb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zFocus)).EndInit();
            this.grpBox.ResumeLayout(false);
            this.grpBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pcb;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TrackBar zFocus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label dropInfo;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar zPower;
        private System.Windows.Forms.CheckBox zOn;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox chkMirror;
        private System.Windows.Forms.CheckBox chkInvert;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
    }
}

