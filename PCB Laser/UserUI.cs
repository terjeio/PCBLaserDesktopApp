/*
 * UserUI.cs - PCB Laser Printer
 *
 * v1.04 / 2018-01-19 / Io Engineering (Terje Io)
 *
 */

/*

Copyright (c) 2015-2017, Io Engineering (Terje Io)
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
//#define USEELTIMA

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Xml;

namespace PCB_Laser
{

    public struct FileEndings 
    {
        public string topCopper, bottomCopper, topMask, bottomMask;
    }

    public partial class UserUI : Form
    {

        const int BYTEPIXELS = 7;
        const int BYTEOFFSET = 59;
        const int TXBUFFERSIZE = 4096;
        const double PIXELSIZE = 25.4 / 1200;

        enum EmulsionType {
            Negative = 0,
            Positive
        }

        private string PortParams = "com15:38400,N,8,1,P";
        private int PollInterval = 50, y = 0, z = 50, Xoffset = 0, XBacklashComp = -1;
        private bool zFocusClicked = false, mirrored = false, started = false, cancel = false, solderMask = false, uknownFileEnding = true;
        private byte[] rowBuffer = null;
        private Bitmap layout = null, board = null;
        private Graphics gr = null;  
        private System.Timers.Timer pollTimer = null;
        private StringBuilder input = new StringBuilder(100);
        private Rectangle rect;
        private EmulsionType emulsionEtchMask = EmulsionType.Negative;
        private EmulsionType emulsionSolderMask = EmulsionType.Negative;
        private FileEndings fileEnding; 

#if USEELTIMA
        private SPortLib.SPortAx SerialPort;
#else
        private SerialPort SerialPort;
#endif

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
            
            // default to KiCad file name endings
            this.fileEnding.topCopper = "F.Cu";
            this.fileEnding.bottomCopper = "B.Cu";
            this.fileEnding.topMask = "F.Mask";
            this.fileEnding.bottomMask = "B.Mask";

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

                        case "EtchMaskEmulsion":
                            this.emulsionEtchMask = this.parseEmulsionType(N.InnerText);
                            break;

                        case "SolderMaskEmulsion":
                            this.emulsionSolderMask = this.parseEmulsionType(N.InnerText);
                            break;

                        case "BacklashCompensation":
                            this.XBacklashComp = int.Parse(N.InnerText);
                            break;

                    }

                }

                foreach (XmlNode N in config.SelectNodes("PCBConfig/FileNameEnding/*"))
                {
                    switch (N.Name)
                    {
                        case "TopCopper":
                            this.fileEnding.topCopper = N.InnerText;
                            break;

                        case "BottomCopper":
                            this.fileEnding.bottomCopper = N.InnerText;
                            break;

                        case "TopMask":
                            this.fileEnding.topMask = N.InnerText;
                            break;

                        case "BottomMask":
                            this.fileEnding.bottomMask = N.InnerText;
                            break;
                    }
                }

            }
            catch
            {
                MessageBox.Show("Config file not found or invalid.", this.Text);
                System.Environment.Exit(1);
            }

#if USEELTIMA
            try
            {
                this.SerialPort = new SPortLib.SPortAx();
            }
            catch
            {
                MessageBox.Show("Failed to load serial port driver.", this.Text);
                System.Environment.Exit(1);
            }

            this.SerialPort.InitString(PortParams.Substring(PortParams.IndexOf(":") + 1));
            this.SerialPort.HandShake = 0x08;
            this.SerialPort.FlowReplace = 0x80;
            this.SerialPort.CharEvent = 10;
            this.SerialPort.BlockMode = false;
            this.SerialPort.OnRxFlag += new SPortLib._ISPortAxEvents_OnRxFlagEventHandler(this.SerialRead);

            this.SerialPort.Open(PortParams.Substring(0, PortParams.IndexOf(":")));
#else
            string[] parameter = PortParams.Substring(PortParams.IndexOf(":") + 1).Split(',');

            if (parameter.Count() < 4)
            {
                this.disableUI();
                MessageBox.Show("Unable to open printer: " + PortParams, this.Text);
                System.Environment.Exit(2);
            }

            this.SerialPort = new SerialPort();
            this.SerialPort.PortName = PortParams.Substring(0, PortParams.IndexOf(":"));
            this.SerialPort.BaudRate = int.Parse(parameter[0]);
            this.SerialPort.Parity = ParseParity(parameter[1]);
            this.SerialPort.DataBits = int.Parse(parameter[2]);
            this.SerialPort.StopBits = int.Parse(parameter[3]) == 1 ? StopBits.One : StopBits.Two;
            if(parameter.Count() > 4)
                this.SerialPort.Handshake = parameter[4] == "X" ? Handshake.XOnXOff : Handshake.RequestToSend;
            this.SerialPort.ReceivedBytesThreshold = 1;
            this.SerialPort.NewLine = "\r\n";
            this.SerialPort.WriteBufferSize = TXBUFFERSIZE;
            this.SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            this.SerialPort.Open();
#endif

            if (this.SerialOpen)
            {

                this.SerialOut("?");

                this.pollTimer = new System.Timers.Timer();
                this.pollTimer.Interval = this.PollInterval;
                this.pollTimer.Elapsed += new System.Timers.ElapsedEventHandler(RenderRow);
                this.pollTimer.SynchronizingObject = this;

                this.SerialOut("XHome:" + this.Xoffset.ToString());
                this.setLaserPower(false);

            } else {
                this.disableUI();
                MessageBox.Show("Unable to open printer: " + PortParams, this.Text);
                System.Environment.Exit(2);
            }

            this.enableUI();

            this.zFocus.Value = this.z;
            this.cbxEtchEmulsion.SelectedIndex = (int)this.emulsionEtchMask;
            this.cbxSolderEmulsion.SelectedIndex = (int)this.emulsionSolderMask;
            this.chkInvert.Checked = solderMask ? this.emulsionSolderMask == EmulsionType.Negative : this.emulsionEtchMask == EmulsionType.Positive;

            this.cbxEtchEmulsion.SelectedIndexChanged += new EventHandler(cbxEtchEmulsion_SelectedIndexChanged);
            this.cbxSolderEmulsion.SelectedIndexChanged += new EventHandler(cbxSolderEmulsion_SelectedIndexChanged);

            this.Visible = true;
 
        }

#if USEELTIMA
        bool SerialOpen { get { return this.SerialPort.IsOpened; } }
        int SerialOutCount { get { return this.SerialPort.OutCount; } }

        void SerialOut(string s)
        {
            this.SerialPort.WriteStr(s + "\r\n");
        }
#else
        bool SerialOpen { get { return this.SerialPort.IsOpen; } }
        int SerialOutCount { get { return this.SerialPort.BytesToWrite; } }

        void SerialOut(string s)
        {
            this.SerialPort.WriteLine(s);
        }

        Parity ParseParity (string parity)
        {
            Parity res = Parity.None;

            switch(parity) {

                case "E":
                    res = Parity.Even;
                    break;

                case "O":
                    res = Parity.Odd;
                    break;

                case "M":
                    res = Parity.Mark;
                    break;

                case "S":
                    res = Parity.Space;
                    break;

            }

            return res;
        }
#endif

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
            this.cbxEtchEmulsion.Enabled = false;
            this.cbxSolderEmulsion.Enabled = false;
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
            this.cbxEtchEmulsion.Enabled = !this.solderMask || this.uknownFileEnding;
            this.cbxSolderEmulsion.Enabled = this.solderMask || this.uknownFileEnding;
            this.aboutMenuItem.Enabled = true;
        }

        void LaserPower(bool on) {
            this.SerialOut("Laser:" + (on ? "1" : "0"));
        }

        void setLaserFocus() {

            int newz = this.zFocus.Value;

            if (!this.zOn.Checked)
            {
                this.zOn.Checked = true;
                LaserPower(true);
            }

            if (newz != this.z)
                this.SerialOut("MoveZ:" + ((newz - this.z) * 10).ToString());

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
            this.SerialOut("Power:" + this.zPower.Value.ToString());
        }

#region UIevents

        void OnExit (object sender, FormClosedEventArgs e) {

            if (this.pollTimer != null)
                this.pollTimer.Stop();

            if (this.SerialOpen)
                this.SerialPort.Close();

        }

        void chkMirror_CheckedChanged(object sender, EventArgs e)
        {
            if (this.board != null)
                ShowBoard();
        }

        void cbxSolderEmulsion_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.emulsionSolderMask = (EmulsionType)this.cbxSolderEmulsion.SelectedIndex;
            if (this.solderMask)
                this.chkInvert.Checked = !this.chkInvert.Checked;
        }

        void cbxEtchEmulsion_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.emulsionEtchMask = (EmulsionType)this.cbxEtchEmulsion.SelectedIndex;
            if (!this.solderMask)
                this.chkInvert.Checked = !this.chkInvert.Checked;;
        }

        void btnHome_Click(object sender, EventArgs e)
        {
            this.SerialOut("HomeXY");
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
                this.SerialOut("XBComp:" + this.XBacklashComp.ToString());

            this.SerialOut("XPix:" + this.layout.Width.ToString());
            this.SerialOut("YPix:" + this.layout.Height.ToString());
            this.SerialOut("Start");

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

            string filename;
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
                filename = files[0].Substring(0, files[0].LastIndexOf('.'));

                if ((solderMask = filename.EndsWith(this.fileEnding.topMask) || filename.EndsWith(this.fileEnding.bottomMask)))
                    this.chkInvert.Checked = this.emulsionSolderMask == EmulsionType.Negative;
                else
                    this.chkInvert.Checked = this.emulsionEtchMask == EmulsionType.Positive;

                this.chkMirror.Checked = filename.EndsWith(this.fileEnding.bottomCopper) ||
                                          filename.EndsWith(this.fileEnding.bottomMask);

                this.uknownFileEnding = !(solderMask || this.chkMirror.Checked || filename.EndsWith(this.fileEnding.topCopper));

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

                if (this.uknownFileEnding)
                    MessageBox.Show("File ending not recognized, set image inversion and mirroring manually!", this.Text);
            }
        }

#endregion

        private bool getPixel(int x, int y)
        {
            x -= 150;
            return x < 0 || x >= this.board.Width ? true : (this.board.GetPixel(x, y).ToArgb() & 0xFFFFFF) == 0xFFFFFF;
        }

        private void RenderRow (object sender, System.Timers.ElapsedEventArgs e) {

            int i, x, p = BYTEPIXELS, pixels = 0;
            bool white;

            // one full length line (x-axis) is about 1KB
            if ((TXBUFFERSIZE - this.SerialOutCount) > 1200 && y < this.layout.Height)
            {

                this.started = true;
                this.pollTimer.Stop();

                char[] s = (y.ToString("D5") + ":").ToCharArray();

                for(i = 0; i < 6; i++)
                    this.rowBuffer[i] = (byte)s[i];

                if ((this.y & 0x01) == 0) {

                    for (x = 0; x < this.layout.Width; x++) {

                        white = this.getPixel(x, this.y);

                        if (white)
                            pixels |= 0x40;

                        if (--p == 0) {
                            p = BYTEPIXELS;
                            if(this.chkInvert.Checked)
                                pixels = ~pixels;
                            this.rowBuffer[i++] = (byte)(BYTEOFFSET + (pixels & 0x7F));
                            pixels = 0;
                        } else
                            pixels = pixels >> 1;

                        if (white)
                            this.layout.SetPixel(x, this.y, Color.LightGreen);
                    }
 
                } else {

                    for (x = this.layout.Width - 1; x >= 0; x--) {

                        white = this.getPixel(x, this.y);

                        if (white)
                            pixels |= 0x40;

                        if (--p == 0) {
                            p = BYTEPIXELS;
                            if (this.chkInvert.Checked)
                                pixels = ~pixels;
                            this.rowBuffer[i++] = (byte)(BYTEOFFSET + (pixels & 0x7F));
                            pixels = 0;
                        } else
                            pixels = pixels >> 1;

                        if (white)
                            this.layout.SetPixel(x, this.y, Color.LightGreen);
                    }

                }

                this.rowBuffer[i++] = (byte)'\n';
#if USEELTIMA
                this.SerialPort.Write(ref this.rowBuffer[0], i);
#else
                this.SerialPort.Write(this.rowBuffer, 0, i);
#endif
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
                while (this.SerialOutCount != 0) ;
                this.rowBuffer[0] = (byte)0x1A;
                p = 102;
                while (--p > 0)
#if USEELTIMA
                    this.SerialPort.Write(ref this.rowBuffer[0], 1); // trigger fill buffer
#else
                    this.SerialPort.Write(this.rowBuffer, 0, 1); // trigger fill buffer
#endif
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
                    c = b.GetPixel(x, y).ToArgb() & 0xFFFFFF;
                    if ((c = (b.GetPixel(x, y).ToArgb() & 0xFFFFFF)) != 0xFFFFFF)
                    {
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
                        if ((b.GetPixel(x, y).ToArgb() & 0xFFFFFF) != 0xFFFFFF)
                        {
                            xhit = 1;
                            xmax = Math.Max(xmax, x);
                        } else
                            x--;
                    }

                }

            }

            return new Rectangle(xmin, ymin, xmax - xmin + 1, ymax - ymin + 1);

        }

        private EmulsionType parseEmulsionType (string type) {
           return type == EmulsionType.Positive.ToString() ? EmulsionType.Positive : EmulsionType.Negative;
        }

        private void SetStatus (string s) {

            if (this.txtStatus.InvokeRequired)
                this.Invoke(new SetTextCallback(SetStatus), new object[] { s });
            else
                this.txtStatus.Text = s;

        }

#if USEELTIMA
        private void SerialRead ()
        {
            int pos = 0;
            string s;

            lock (this.input)
            {
                this.input.Append(this.SerialPort.ReadStr());

                while ((pos = this.input.ToString().IndexOf('\n')) > 0) {
                    s = input.ToString(0, pos - 1);
                    SetStatus(s);
                    this.input.Remove(0, pos + 1);
                }

            }
        }
#else
        void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int pos = 0;
            string s;

            lock (this.input)
            {
                this.input.Append(this.SerialPort.ReadExisting());

                while ((pos = this.input.ToString().IndexOf('\n')) > 0)
                {
                    s = input.ToString(0, pos - 1);
                    SetStatus(s);
                    this.input.Remove(0, pos + 1);
                }
            }
        }
#endif
    }

}
