using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Soundfont2Tool
{
    public partial class RtxtForm : Form
    {

        public RtxtForm(string title)
        {
            InitializeComponent();
            this.Text = title;
        }

        public void Show(string rtxtContents)
        {
            rtxt.Text = rtxtContents;
            this.Show();
        }

        private void RtxtForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) { 
                e.Cancel = true;
                this.Visible = false;
            }
        }
    }
}
