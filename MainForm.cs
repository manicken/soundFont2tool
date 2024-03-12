using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq.Expressions;

using Soundfont2;

namespace Soundfont2Tool
{
    public partial class MainForm : Form
    {
        string soundFontsRootDir = @"G:\_git\__TEENSY\SF2_SoundFonts-master";
        Soundfont2_reader sfReader;

        private RichTextBoxForm rtxtformInst;
        private RichTextBoxForm rtxtformIbag;
        private RichTextBoxForm rtxtformIgen;
        private RichTextBoxForm rtxtformImod;
        private RichTextBoxForm rtxtformShdr;

        private ListBoxForm lstboxformInst;
        private ListBoxForm lstboxformIbag;
        private ListBoxForm lstboxformIgen;
        private ListBoxForm lstboxformImod;
        private ListBoxForm lstboxformShdr;
        public MainForm()
        {
            sfReader = new Soundfont2_reader();
            InitializeComponent();
            Debug.rtxt = rtxt;
            rtxtformInst = new RichTextBoxForm("Instruments");
            rtxtformIbag = new RichTextBoxForm("ibag:s");
            rtxtformIgen = new RichTextBoxForm("igen:s");
            rtxtformImod = new RichTextBoxForm("imod:s");
            rtxtformShdr = new RichTextBoxForm("shdr:s");

            lstboxformInst = new ListBoxForm("Instruments");
            lstboxformIbag = new ListBoxForm("ibag:s");
            lstboxformIgen = new ListBoxForm("igen:s");
            lstboxformImod = new ListBoxForm("imod:s");
            lstboxformShdr = new ListBoxForm("shdr:s");

            lstboxformInst.ListBoxFormItemSelected += Inst_LstBox_SelectedIndexChanged;
            lstboxformIbag.ListBoxFormItemSelected += IBAG_LstBox_SelectedIndexChanged;
            lstboxformIgen.ListBoxFormItemSelected += IGEN_LstBox_SelectedIndexChanged;
            lstboxformShdr.ListBoxFormItemSelected += SHDR_LstBox_SelectedIndexChanged;
        }

        private void Inst_LstBox_SelectedIndexChanged(object sender, ListItemWithIndex item)
        {
            lstboxformIbag.lstBox.ClearSelected();
            lstboxformIgen.lstBox.ClearSelected();
            lstboxformShdr.lstBox.ClearSelected();
            int selectedIndex = item.index;
            Debug.rtxt.AppendLine(selectedIndex.ToString());
            skipIBAG_LstBox_SelectedIndexChanged = true;
            skipIGEN_LstBox_SelectedIndexChanged = true;
            skipSHDR_LstBox_SelectedIndexChanged = true;
            pdta_rec pdta = sfReader.fileData.sfbk.pdta;
            if (selectedIndex == pdta.inst.Length - 1)
            {
                lstboxformIbag.lstBox.SetSelected(pdta.ibag.Length - 1, true);
                lstboxformIgen.lstBox.SetSelected(pdta.igen.Length - 1, true);
                lstboxformShdr.lstBox.SetSelected(pdta.shdr.Length - 1, true);
                skipIBAG_LstBox_SelectedIndexChanged = false;
                skipIGEN_LstBox_SelectedIndexChanged = false;
                skipSHDR_LstBox_SelectedIndexChanged = false;
                return;
            }

            int start = pdta.inst[selectedIndex].wInstBagNdx;
            int end = pdta.inst[selectedIndex+1].wInstBagNdx;
            
            for (int i = start; i < end; i++)
            {
                lstboxformIbag.lstBox.SetSelected(i, true);
                int start2 = pdta.ibag[i].wGenNdx;
                int end2 = pdta.ibag[i + 1].wGenNdx;
                for (int i2 = start2; i2 < end2; i2++)
                {
                    lstboxformIgen.lstBox.SetSelected(i2, true);
                    if (pdta.igen[i2].sfGenOper == SFGenerator.sampleID)
                    {
                        int sampleIndex = pdta.igen[i2].genAmount.UAmount;
                        lstboxformShdr.lstBox.SetSelected(sampleIndex, true);
                    }
                }

            }
            skipIBAG_LstBox_SelectedIndexChanged = false;
            skipIGEN_LstBox_SelectedIndexChanged = false;
            skipSHDR_LstBox_SelectedIndexChanged = false;
            //lstboxformInst.lstBox.SelectionMode = SelectionMode.MultiExtended;
        }
        bool skipIGEN_LstBox_SelectedIndexChanged = false;
        bool skipIBAG_LstBox_SelectedIndexChanged = false;
        bool skipSHDR_LstBox_SelectedIndexChanged = false;
        private void IBAG_LstBox_SelectedIndexChanged(object sender, ListItemWithIndex item)
        {
            if (skipIBAG_LstBox_SelectedIndexChanged) return;
            rtxt.AppendLine(item.text);
        }

        private void IGEN_LstBox_SelectedIndexChanged(object sender, ListItemWithIndex item)
        {
            if (skipIGEN_LstBox_SelectedIndexChanged) return;
        }

        private void SHDR_LstBox_SelectedIndexChanged(object sender, ListItemWithIndex item)
        {
            if (skipSHDR_LstBox_SelectedIndexChanged) return;
        }


        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            rtxt.Clear();
            string filePath = "";
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Soundfont2 files|*.sf2";
                ofd.InitialDirectory = soundFontsRootDir;
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
                //return;
            }
            sfbk_rec sfbk = sfReader.fileData.sfbk;
            rtxt.AppendLine(sfbk.info.ToString());
            rtxt.AppendLine(sfbk.sdta.ToString());
            //rtxt.AppendLine(sfbk.pdta.ToString());
            //ReadAndShowSoundFontInfo(filePath);
            pdta_rec pdta = sfbk.pdta;
            /*
            rtxtformInst.Show("inst count: " + pdta.inst.Length + Environment.NewLine + pdta.inst.GetAllToStrings());
            rtxtformIbag.Show("ibag count: " + pdta.ibag.Length + Environment.NewLine + pdta.ibag.GetAllToStrings());
            rtxtformIgen.Show("igen count: " + pdta.igen.Length + Environment.NewLine + pdta.igen.GetAllToStrings());
            //rtxtImod.Show("imod count: " + pdta.imod.Length + Environment.NewLine + pdta.imod.GetAllToStrings());
            rtxtformShdr.Show("shdr count: " + pdta.shdr.Length + Environment.NewLine + pdta.shdr.GetAllToStrings());
            */
            
            lstboxformInst.Show("(" + pdta.inst.Length + ")", pdta.inst.GetAllToStringsAsArray());
            lstboxformIbag.Show("(" + pdta.ibag.Length + ")", pdta.ibag.GetAllToStringsAsArray());
            lstboxformIgen.Show("(" + pdta.igen.Length + ")", pdta.igen.GetAllToStringsAsArray());
            //lstboxformImod.Show("(" + pdta.imod.Length+")", pdta.imod.GetAllToStringsAsArray());
            lstboxformShdr.Show("(" + pdta.shdr.Length + ")", pdta.shdr.GetAllToStringsAsArray());

            lstboxformInst.Width = 320;
            lstboxformIbag.Width = 275;
            lstboxformIgen.Width = 576;
            lstboxformShdr.Width = 1110;
            lstboxformInst.Top = 10;
            lstboxformInst.Left = 10;
            lstboxformIbag.Top = 10;
            lstboxformIbag.Left = lstboxformInst.Right;
            lstboxformIgen.Top = 10;
            lstboxformIgen.Left = lstboxformIbag.Right;
            lstboxformShdr.Top = lstboxformInst.Bottom;
            lstboxformShdr.Left = 10;

            this.TopMost = true;
            //this.TopLevel = true;
            this.TopMost = false;
            //this.TopLevel = false;
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
                fbd.SelectedPath = soundFontsRootDir;
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
                ofd.InitialDirectory = soundFontsRootDir;
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
            ReadAndShowFile(soundFontsRootDir + @"\AWE ROM gm.sf2");
        }
        public enum SFGeneratorItemType
        {
            ushort_t,
            short_t,
            two_bytes
        }
        
        private void button1_Click_1(object sender, EventArgs e)
        {
            string code = "";
            string[] names = Enum.GetNames(typeof(SFGenerator));
            code = "if (type == SFGenerator." + names[0] + ")" + Environment.NewLine + "{" + Environment.NewLine + Environment.NewLine + "}" + Environment.NewLine;
            for (int i=1;i<names.Length;i++)
            {
                code += "else if (type == SFGenerator." + names[i] + ")" + Environment.NewLine + "{" + Environment.NewLine + Environment.NewLine + "}" + Environment.NewLine;
            }
            rtxt.Text = code;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            rtxt.AppendLine(Soundfont2to_cpp.getcpp(sfReader.fileData.sfbk));
        }
    }
}

public static class Debug
{
    public static RichTextBox rtxt;
}