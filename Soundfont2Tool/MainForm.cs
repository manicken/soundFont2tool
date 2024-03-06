using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Soundfont2Tool
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            rtxt.Clear();
            string filePath = "";
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Soundfont2 files|*.sf2";
                ofd.InitialDirectory = @"G:\_Projects\soundfonts\New folder\OBIE1";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                filePath = ofd.FileName;
            }
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    string riffTag = new String(br.ReadChars(4));
                    if (riffTag != "RIFF")
                    {
                        MessageBox.Show("this is not a RIFF fileformat");
                        return;
                    }
                    
                    UInt32 dataSize = br.ReadUInt32();

                    string sfbkTag = new string(br.ReadChars(4));

                    if (sfbkTag != "sfbk") // sound font bank format, which is the most common used format
                    {
                        MessageBox.Show("this is not sfbk fileformat");
                        return;
                    }
                    string firstFOURCC = new string(br.ReadChars(4));
                    UInt32 firstFOURCCsize = br.ReadUInt32();
                    string secondFOURCC = "";
                    if (firstFOURCC == "LIST")
                    {
                        secondFOURCC = new string(br.ReadChars(4));

                    }
                    


                    rtxt.AppendLine(riffTag);
                    rtxt.AppendLine("Data size: " + dataSize.ToString());
                    rtxt.AppendLine(firstFOURCC + " size: " + firstFOURCCsize.ToString());
                    rtxt.AppendLine(secondFOURCC + " size: ");
                }
            }
        }

        
    }

    public static class Extensions
    {
        public static void AppendLine(this RichTextBox thisRtxt, string text)
        {
            thisRtxt.AppendText(text + Environment.NewLine);
        }
    }
}
