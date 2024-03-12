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
    public delegate void ListBoxFormItemSelectedEventHandler(object sender, ListItemWithIndex item);
    public partial class ListBoxForm : Form
    {
        public event ListBoxFormItemSelectedEventHandler ListBoxFormItemSelected;

        private string title = "";
        public ListBoxForm(string title)
        {
            InitializeComponent();
            this.Text = title;
            this.title = title;
            
        }

        public void Show(string[] items)
        {
            lstBox.Items.AddRange(items);
            this.Show();
        }

        public void Show(string info, string[] items)
        {

            SetInfo(info);
            ListItemWithIndex[] liwiItems = new ListItemWithIndex[items.Length];
            int indexPad = liwiItems.Length.ToString().Length;
            for (int i = 0; i < liwiItems.Length; i++)
                liwiItems[i] = new ListItemWithIndex(i, items[i], indexPad);
            lstBox.Items.Clear();
            lstBox.Items.AddRange(liwiItems);
            this.Show();
        }

        private void ListBoxForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Visible = false;
            }
        }

        public void SetInfo(string text)
        {
            this.Text = title + " - " + text;
        }

        private void lstBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstBox.SelectedItem == null) return;
            if (ListBoxFormItemSelected != null)
                ListBoxFormItemSelected(lstBox, (ListItemWithIndex)lstBox.SelectedItem);
        }

        private void ListBoxForm_Resize(object sender, EventArgs e)
        {
            //this.Text = this.Size.ToString(); // development 
        }
    }
    public class ListItemWithIndex
    {
        public int index = 0;
        public string text = "";
        private int indexPad = 0;

        public ListItemWithIndex(int index, string text, int indexPad)
        {
            this.index = index;
            this.text = text;
            this.indexPad = indexPad;
        }
        public override string ToString()
        {
            return index.ToString().PadLeft(indexPad) + " | " + text;
        }
    }
}
