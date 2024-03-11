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

        private RtxtForm rtxtInst;
        private RtxtForm rtxtIbag;
        private RtxtForm rtxtIgen;
        private RtxtForm rtxtImod;
        private RtxtForm rtxtShdr;
        public MainForm()
        {
            sfReader = new Soundfont2_reader();
            InitializeComponent();
            Debug.rtxt = rtxt;
            rtxtInst = new RtxtForm("Instruments");
            rtxtIbag = new RtxtForm("ibag:s");
            rtxtIgen = new RtxtForm("igen:s");
            rtxtImod = new RtxtForm("imod:s");
            rtxtShdr = new RtxtForm("shdr:s");
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
            ReadAndShowFile(filePath);
        }
        private void ReadAndShowFile(string filePath)
        {
            if (sfReader.readFile(filePath) == false)
            {
                rtxt.AppendLine(sfReader.lastError);
                return;
            }
            sfbk_rec sfbk = sfReader.fileData.sfbk;
            rtxt.AppendLine(sfbk.info.ToString());
            rtxt.AppendLine(sfbk.sdta.ToString());
            //rtxt.AppendLine(sfbk.pdta.ToString());
            //ReadAndShowSoundFontInfo(filePath);
            pdta_rec pdta = sfbk.pdta;
            rtxtInst.Show("inst count: " + pdta.inst.Length + Environment.NewLine + pdta.inst.GetAllToStrings());
            rtxtIbag.Show("ibag count: " + pdta.ibag.Length + Environment.NewLine + pdta.ibag.GetAllToStrings());
            rtxtIgen.Show("igen count: " + pdta.igen.Length + Environment.NewLine + pdta.igen.GetAllToStrings());
            //rtxtImod.Show("imod count: " + pdta.imod.Length + Environment.NewLine + pdta.imod.GetAllToStrings());
            rtxtShdr.Show("shdr count: " + pdta.shdr.Length + Environment.NewLine + pdta.shdr.GetAllToStrings());
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

        private void btnDirectOpen_Click(object sender, EventArgs e)
        {
            ReadAndShowFile(@"G:\_Projects\SF2_SoundFonts-master\AWE ROM gm.sf2");
        }
    }
}

public static class Extensions
{
    public static void AppendLine(this RichTextBox thisRtxt, Exception ex)
    {
        thisRtxt.AppendText(ex.ToString() + Environment.NewLine);
    }
    public static void AppendLine(this RichTextBox thisRtxt, string text)
    {
        thisRtxt.AppendText(text + Environment.NewLine);
    }
    public static void AppendCharArrayAsHex(this RichTextBox thisRtxt, char[] items)
    {
        string hexStr = "";
        for (int i=0;i<items.Length;i++)
        {
            int item = items[i];
            hexStr += item.ToString("X2");
            if (i < items.Length - 1) hexStr += ", ";
        }
        thisRtxt.AppendLine(hexStr);
    }

    public static string GetAllToStrings<T>(this T[] thisObj)
    {
        if (thisObj == null) return "";
        string r = "";
        for (int i = 0; i < thisObj.Length; i++)
        {
            if (thisObj[i] == null) continue;
            r += thisObj[i].ToString() + Environment.NewLine;
        }
        return r;
    }
}

public static class Debug
{
    public static RichTextBox rtxt;
}