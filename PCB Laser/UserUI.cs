/*
 * UserUI.cs - PCB Laser Printer
 *
 * v1.01 / 2016-08-22 / Io Engineering (Terje Io)
 *
 */

/*

Copyright (c) 2015, Io Engineering (Terje Io)
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

· Redistributions of source code must retain the above copyright notice, this
list of conditions and the following disclaimer.

· Redistributions in binary form must reproduce the above copyright notice, this
list of conditions and the following disclaimer in the documentation and/or
other materials provided with the distribution.

· Neither the name of the copyright holder nor the names of its contributors may
be used to endorse or promote products derived from this software without
specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace PCB_Laser
{

    public partial class UserUI : Form
    {

        const int BYTEPIXELS = 7;
        const int BYTEOFFSET = 59;
        const int TXBUFFERSIZE = 4096;
        const double PIXELSIZE = 25.4 / 1200;

        private string PortParams = "com14:38400,N,8,1,P";
        private int PollInterval = 50, y = 0, z = 50, Xoffset = 0, XBacklashComp = -1;
        private bool zFocusClicked = false, mirrored = false, started = false, cancel = false;
        private byte[] rowBuffer = null;
        private Bitmap layout = null, board = null;
        private Graphics gr = null;
        private System.Timers.Timer pollTimer = null;
        private SPortLib.SPortAx SerialPort;
        private StringBuilder input = new StringBuilder(100);
        private Rectangle rect;

        private delegate void SetTextCallback(string text); 

        public UserUI()
        {

            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(OnExit);
            this.btnStart.Click += new EventHandler(btnStart_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.DragEnter += new DragEventHandler(UserUI_DragEnter);
            this.DragDrop += new DragEventHandler(UserUI_DragDrop);
            this.zFocus.Scroll += new EventHandler(zFocus_Scrolled);
            this.zFocus.MouseDown += new MouseEventHandler(zFocus_MouseDown);
            this.zFocus.MouseUp += new MouseEventHandler(zFocus_MouseUp);
            this.zPower.Scroll += new EventHandler(zPower_Scrolled);
            this.zPower.MouseDown += new MouseEventHandler(zPower_MouseDown);
            this.zPower.MouseUp += new MouseEventHandler(zPower_MouseUp);
            this.zOn.Click += new EventHandler(zOn_Click);
            this.btnHome.Click += new EventHandler(btnHome_Click);
            this.chkMirror.CheckedChanged += new EventHandler(chkMirror_CheckedChanged);
            this.aboutMenuItem.Click += new EventHandler(aboutMenuItem_Click);
            this.exitMenuItem.Click += new EventHandler(exitMenuItem_Click);
            
            try
            {
                XmlDocument config = new XmlDocument();

                config.Load(Application.StartupPath + "\\PCBLaser.config");

                foreach (XmlNode N in config.SelectNodes("PCBConfig/*"))
                {

                    switch (N.Name)
                    {

                        case "PortParams":
                            this.PortParams = N.InnerText;
                            break;

                        case "DefaultPower":
                            this.zPower.Value = int.Parse(N.InnerText);
                            break;

                        case "XOffset":
                            this.Xoffset = int.Parse(N.InnerText);
                            break;

                        case "BacklashCompensation":
                            this.XBacklashComp = int.Parse(N.InnerText);
                            break;

                    }

                }

            }
            catch
            {
                MessageBox.Show("Config file not found.", this.Text);
                System.Environment.Exit(1);
            }

            this.SerialPort = new SPortLib.SPortAx();
            this.SerialPort.InitString(PortParams.Substring(PortParams.IndexOf(":") + 1));
            this.SerialPort.HandShake = 0x08;
            this.SerialPort.FlowReplace = 0x80;
            this.SerialPort.CharEvent = 10;
            this.SerialPort.BlockMode = false;
            this.SerialPort.OnRxFlag += new SPortLib._ISPortAxEvents_OnRxFlagEventHandler(this.SerialRead);

            this.SerialPort.Open(PortParams.Substring(0, PortParams.IndexOf(":")));

            if (this.SerialPort.IsOpened)
            {

                this.SerialPort.WriteStr("?\r\n");

                this.pollTimer = new System.Timers.Timer();
                this.pollTimer.Interval = this.PollInterval;
                this.pollTimer.Elapsed += new System.Timers.ElapsedEventHandler(RenderRow);
                this.pollTimer.SynchronizingObject = this;

                this.SerialPort.WriteStr("XHome:" + this.Xoffset.ToString() + "\r\n");
                this.setLaserPower(false);

            } else {
                this.disableUI();
                MessageBox.Show("Unable to open printer: " + PortParams, this.Text);
                System.Environment.Exit(2);
            }

            this.enableUI();

            this.zFocus.Value = this.z;
            this.Visible = true;
 
        }

        void exitMenuItem_Click(object sender, EventArgs e)
        {
            OnExit(sender, null);
            Application.Exit();
        }

        void aboutMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About(this);
        }

        void disableUI () {
            this.AllowDrop = false;
            this.btnHome.Enabled = false;
            this.btnStart.Enabled = false;
            this.btnCancel.Enabled = true;
            this.zFocus.Enabled = false;
            this.zOn.Enabled = false;
            this.zPower.Enabled = false;
            this.chkInvert.Enabled = false;
            this.chkMirror.Enabled = false;
            this.aboutMenuItem.Enabled = false;
        }

        void enableUI () {
            this.AllowDrop = true;
            this.btnHome.Enabled = true;
            this.btnCancel.Enabled = false;
            this.btnStart.Enabled = this.layout != null;
            this.zFocus.Enabled = true;
            this.zOn.Enabled = true;
            this.zPower.Enabled = true;
            this.chkInvert.Enabled = true;
            this.chkMirror.Enabled = true;
            this.aboutMenuItem.Enabled = true;
        }

        void LaserPower(bool on) {
            this.SerialPort.WriteStr("Laser:" + (on ? "1" : "0") + "\r\n");
        }

        void setLaserFocus() {

            int newz = this.zFocus.Value;

            if (!this.zOn.Checked)
            {
                this.zOn.Checked = true;
                LaserPower(true);
            }

            if (newz != this.z)
                this.SerialPort.WriteStr("MoveZ:" + ((newz - this.z) * 10).ToString() + "\r\n");

            this.z = newz;
        }

        void setLaserPower(bool laserOn)
        {
/*
            if (laserOn && !this.zOn.Checked)
            {
                this.zOn.Checked = true;
                LaserPower(true);
            }
*/
            this.textBox1.Text = this.zPower.Value.ToString();
            this.SerialPort.WriteStr("Power:" + this.zPower.Value.ToString() + "\r\n");

        }

#region UIevents

        void OnExit (object sender, FormClosedEventArgs e) {

            if (this.pollTimer != null)
                this.pollTimer.Stop();

            if (this.SerialPort.IsOpened)
                this.SerialPort.Close();

        }

        void chkMirror_CheckedChanged(object sender, EventArgs e)
        {
            if (this.board != null)
                ShowBoard();
        }

        void btnHome_Click(object sender, EventArgs e)
        {
            this.SerialPort.WriteStr("HomeXY\r\n");
        }

        void zOn_Click(object sender, EventArgs e)
        {
           this.LaserPower(this.zOn.Checked);
        }

        void zFocus_Scrolled(object sender, EventArgs e)
        {
            if (!this.zFocusClicked)
                this.setLaserFocus();
        }

        void zFocus_MouseDown (object sender, MouseEventArgs e) {
            this.zFocusClicked = true;
        }
        
        void zFocus_MouseUp (object sender, MouseEventArgs e) {
            this.zFocusClicked = false;
            this.setLaserFocus();
        }

        void zPower_Scrolled(object sender, EventArgs e)
        {
            if (!this.zFocusClicked)
                this.setLaserPower(true);
        }

        void zPower_MouseDown(object sender, MouseEventArgs e)
        {
            this.zFocusClicked = true;
        }

        void zPower_MouseUp(object sender, MouseEventArgs e)
        {
            this.zFocusClicked = false;
            this.setLaserPower(true);
        }

        void btnStart_Click(object sender, EventArgs e)
        {

            if(this.started)
                this.ShowBoard();

            this.zOn.Checked = true;

            if(this.XBacklashComp >= 0)
                this.SerialPort.WriteStr("XBComp:" + this.XBacklashComp.ToString() + "\r\n");

            this.SerialPort.WriteStr("XPix:" + this.layout.Width.ToString() + "\r\n");
            this.SerialPort.WriteStr("YPix:" + this.layout.Height.ToString() + "\r\n");
            this.SerialPort.WriteStr("Start\r\n");

            this.y      = 0;
            this.cancel = false;

            this.pollTimer.Start();

            disableUI();

        }

        void btnCancel_Click (object sender, EventArgs e) {
            this.cancel      = true;
            this.zOn.Checked = false;
            this.y           = this.layout.Height;
        }

        void UserUI_DragEnter (object sender, DragEventArgs e) {

            bool allow = false;

            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                allow = files.Count() == 1 && (files[0].EndsWith(".png") || files[0].EndsWith(".bmp"));
            }

            if(allow)
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        void UserUI_DragDrop (object sender, DragEventArgs e) {

            string filetype;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            Bitmap image = null;

            if (files.Count() == 1) {

                this.started = false;
                this.dropInfo.Visible = false;

                if (this.layout != null)
                    this.layout.Dispose();

                if(this.gr != null)
                    this.gr.Dispose();

                if (this.board != null)
                {
                    this.board.Dispose();
                    this.board = null;
                }

                this.mirrored = false;
                filetype = files[0].Substring(files[0].LastIndexOf('.'));
                this.chkInvert.Checked = files[0].EndsWith("smask" + filetype) ||
                                          files[0].EndsWith("Mask" + filetype);
                this.chkMirror.Checked = files[0].EndsWith("B_Cu" + filetype) ||
                                          files[0].EndsWith("B_Mask" + filetype) ||
                                           files[0].EndsWith("B.Cu" + filetype) ||
                                            files[0].EndsWith("B.Mask" + filetype);

                image = new Bitmap(files[0]);
                this.rect = GetPCBSize(image);

                this.board = new Bitmap(this.rect.Width, this.rect.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
                Graphics graphics = Graphics.FromImage(this.board);
                graphics.DrawImage(image, new Rectangle(0, 0, this.board.Width, this.board.Height), this.rect, GraphicsUnit.Pixel);

                this.txtWidth.Text = (this.board.Width * PIXELSIZE).ToString("###.00") + "mm (" + this.board.Width.ToString() + ")";
                this.txtHeight.Text = (this.board.Height * PIXELSIZE).ToString("###.00") + "mm (" + this.board.Height.ToString() + ")";

                this.rect.X = 0;
                this.rect.Y = 0;
                this.rect.Width += 300;
                this.rect.Width += (this.rect.Width % 7 == 0 ? 0 : 7 - this.rect.Width % 7);

                this.ShowBoard();

                this.rowBuffer = new byte[7 + this.layout.Width / 7];

                this.enableUI();

            }

        }

#endregion

        private int getPixel(int x, int y)
        {
            x -= 150;
            return x < 0 || x >= this.board.Width ? 1 : this.board.GetPixel(x, y).ToArgb() & 0xFFFFFF;
        }

        private void RenderRow (object sender, System.Timers.ElapsedEventArgs e) {

            int i, x, p = BYTEPIXELS, color, pixels = 0;

            // one full length line (x-axis) is about 1KB
            if ((TXBUFFERSIZE - this.SerialPort.OutCount) > 1200 && y < this.layout.Height) {

                this.started = true;
                this.pollTimer.Stop();

                char[] s = (y.ToString("D5") + ":").ToCharArray();

                for(i = 0; i < 6; i++)
                    this.rowBuffer[i] = (byte)s[i];

                if ((this.y & 0x01) == 0) {

                    for (x = 0; x < this.layout.Width; x++) {

                        color = this.getPixel(x, this.y);

                        if (color != 0)
                            pixels |= 0x40;

                        if (--p == 0) {
                            p = BYTEPIXELS;
                            if(this.chkInvert.Checked)
                                pixels = ~pixels;
                            this.rowBuffer[i++] = (byte)(BYTEOFFSET + (pixels & 0x7F));
                            pixels = 0;
                        } else
                            pixels = pixels >> 1;

                        if (color != 0)
                            this.layout.SetPixel(x, this.y, Color.LightGreen);
                    }
 
                } else {

                    for (x = this.layout.Width - 1; x >= 0; x--) {

                        color = this.getPixel(x, this.y);

                        if (color != 0)
                            pixels |= 0x40;

                        if (--p == 0) {
                            p = BYTEPIXELS;
                            if (this.chkInvert.Checked)
                                pixels = ~pixels;
                            this.rowBuffer[i++] = (byte)(BYTEOFFSET + (pixels & 0x7F));
                            pixels = 0;
                        } else
                            pixels = pixels >> 1;

                        if (color != 0)
                            this.layout.SetPixel(x, this.y, Color.LightGreen);
                    }

                }

                this.rowBuffer[i++] = (byte)'\n';
                this.SerialPort.Write(ref this.rowBuffer[0], i);

                if (++this.y != this.layout.Height) {
                    this.pollTimer.Interval = this.PollInterval;
                    this.pollTimer.Start();
                } else {
                    this.zOn.Checked = false;
                    enableUI();
                }

                pcb.Invalidate();
 
            }

            if (this.cancel) {
                this.pollTimer.Stop();
                while (this.SerialPort.OutCount != 0);
                this.rowBuffer[0] = (byte)0x1A;
                p = 102;
                while (--p > 0)
                    this.SerialPort.Write(ref this.rowBuffer[0], 1); // trigger fill buffer
                this.enableUI();
            }

        }

        private void ShowBoard ()
        {

            if (this.layout != null)
                this.layout.Dispose();

            if (this.gr != null)
                this.gr.Dispose();

            if (this.chkMirror.Checked != mirrored)
            {
                this.board.RotateFlip(RotateFlipType.RotateNoneFlipX);
                mirrored = this.chkMirror.Checked;
            }

            this.layout = new Bitmap(this.rect.Width, this.rect.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            this.gr = Graphics.FromImage(this.layout);
            this.gr.FillRectangle(Brushes.White, 0, 0, this.layout.Width, this.layout.Height);
            this.gr.DrawImage(this.board, new Rectangle(150, 0, this.layout.Width, this.layout.Height), this.rect, GraphicsUnit.Pixel);
            this.pcb.SizeMode = PictureBoxSizeMode.Zoom;
            this.pcb.Image = this.layout;

        }

        private Rectangle GetPCBSize (Bitmap b) {

            int c, x, y, xhit, xmin = b.Width - 1, xmax = 0, ymin = b.Height - 1, ymax = 0;

            for (y = 0; y < b.Height; y++) {

                xhit = 0;
                x = 0;
                    
                while(xhit == 0 && x < b.Width) {
                    if ((c = (b.GetPixel(x, y).ToArgb() & 0xFFFFFF)) == 0) {
                        xhit = 1;
                        xmin = Math.Min(xmin, x);
                    }
                    else
                        x++;
                }

                if(xhit != 0) {

                    ymax = y;
                    ymin = Math.Min(ymin, y);

                    x = b.Width - 1;
                    xhit = 0;

                    while(xhit == 0 && x > xmax) {
                        if ((b.GetPixel(x, y).ToArgb() & 0xFFFFFF) == 0) {
                            xhit = 1;
                            xmax = Math.Max(xmax, x);
                        } else
                            x--;
                    }

                }

            }

            return new Rectangle(xmin, ymin, xmax - xmin + 1, ymax - ymin + 1);

        }

        private void SetStatus (string s) {

            if (this.txtStatus.InvokeRequired)
                this.Invoke(new SetTextCallback(SetStatus), new object[] { s });
            else
                this.txtStatus.Text = s;

        }

        private void SerialRead () {

            int pos = 0;
            string s;

            lock (this.input) {

                this.input.Append(this.SerialPort.ReadStr());

                while ((pos = this.input.ToString().IndexOf('\n')) > 0) {
                    s = input.ToString(0, pos - 1);
                    SetStatus(s);
                    this.input.Remove(0, pos + 1);
                }

            }

        }

    }

}
