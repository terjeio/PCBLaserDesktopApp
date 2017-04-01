/*
 * About.cs - for PCB Laser Printer
 *
 * v1.00 / 2016-05-08 / Io Engineering (Terje Io)
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

namespace PCB_Laser
{
    public partial class About : Form
    {
        
        private System.Windows.Forms.Form parent;

        public About(System.Windows.Forms.Form parent)
        {
            this.parent = parent;
            this.Load += new System.EventHandler(this.About_Load);

            InitializeComponent();
            this.ShowDialog();
        }

        private void About_Load(object sender, System.EventArgs e)
        {
            this.Left = this.parent.Left + (this.parent.Width - this.Width) / 2;
            this.Top = this.parent.Top + 100;
            this.Text = "About " + parent.Text + "...";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
            this.Dispose();
        }
    
    }

}
