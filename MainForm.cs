using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Soundfont2;

namespace Soundfont2Tool
{
    public partial class MainForm : Form
    {
        Soundfont2_reader sfReader;
        public MainForm()
        {
            sfReader = new Soundfont2_reader();
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            rtxt.Clear();
            string filePath = "";
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Soundfont2 files|*.sf2";
                ofd.InitialDirectory = @"G:\_Projects\SF2_SoundFonts-master";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                filePath = ofd.FileName;
            }
            if (sfReader.readFile(filePath) == false)
            {
                rtxt.AppendLine(sfReader.lastError);
                return;
            }
            rtxt.AppendLine(sfReader.fileData.sfbk.info.ToString());

            rtxt.AppendLine(sfReader.debugInfo);
            
            //ReadAndShowSoundFontInfo(filePath);
        }

        private void ReadAndShowSoundFontInfo(string filePath)
        {
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

        private void btnListFilesInFir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.RootFolder = Environment.SpecialFolder.MyComputer;
                fbd.SelectedPath = @"G:\_Projects\SF2_SoundFonts-master";
                if (fbd.ShowDialog() != DialogResult.OK) return;

                string[] files = Directory.GetFiles(fbd.SelectedPath, "*.sf2");
                int maxWidth = 0;
                foreach (string file in files)
                {
                    int w = Path.GetFileName(file).Length;
                    if (w > maxWidth) maxWidth = w;
                }
                int divisior = 4;
                for (int i = 0;i<files.Length;i++)
                {
                    FileInfo fi = new FileInfo(files[i]);
                    rtxt.AppendLine(fi.Name.PadRight(maxWidth) + "  " + fi.Length.ToString().PadLeft(10) + " % "+ divisior .ToString()+ " = " + (fi.Length % divisior).ToString());
                    rtxt.AppendLine("".PadLeft(maxWidth+10+7+3, '-'));
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rtxt.Clear();
            string filePath = "";
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text files|*.txt";
                ofd.InitialDirectory = @"G:\_Projects\SF2_SoundFonts-master";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                filePath = ofd.FileName;
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                fs.Seek(10, SeekOrigin.Begin);
                long endPos = fs.Position + 8;
                using (BinaryReader br = new BinaryReader(fs))
                {
                    
                    while (fs.Position < endPos)
                    {
                        rtxt.AppendLine(br.ReadChar().ToString());
                    }
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
