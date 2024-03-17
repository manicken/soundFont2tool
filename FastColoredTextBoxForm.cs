using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace Soundfont2Tool
{
    public partial class FastColoredTextBoxForm : Form
    {
        Style BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        public FastColoredTextBoxForm(string title)
        {
            InitializeComponent();
            this.Text = title;
        }
        public void Show(string contents)
        {
            //rtxt.Text = rtxtContents;
            fctb.Text = contents;
            this.Show();
        }

        private void FastColoredTextBoxForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Visible = false;
            }
        }

        private void fctb_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            //foreach (var r in fctb.GetRanges(@"\buint32_t\b", System.Text.RegularExpressions.RegexOptions.Singleline))
            {
                e.ChangedRange.ClearStyle(BlueStyle);
                e.ChangedRange.SetStyle(BlueStyle, @"\b(uint32_t|int32_t|int16_t|uint16_t)\b", System.Text.RegularExpressions.RegexOptions.Singleline);
            }
        }
    }
}
