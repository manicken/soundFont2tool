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

        private RichTextBoxForm rtxtformInfo;
        private RichTextBoxForm rtxtformLog;
        private RichTextBoxForm rtxtformCppOutput;

        private ListBoxForm lstboxformInst;
        private ListBoxForm lstboxformIbag;
        private ListBoxForm lstboxformIgen;
        private ListBoxForm lstboxformImod;
        private ListBoxForm lstboxformShdr;

        int topOffset = 26;
        int leftOffset = 2;
        int globalHeigth = 320;

        public MainForm()
        {
            sfReader = new Soundfont2_reader();
            InitializeComponent();
            globalHeigth = this.Height / 2;

            rtxtformInfo = new RichTextBoxForm("Info");
            rtxtformLog = new RichTextBoxForm("Debug Log");
            rtxtformCppOutput = new RichTextBoxForm("Export to Teensy - Development Test");
            rtxtformCppOutput.StartPosition = FormStartPosition.CenterScreen;
            lstboxformInst = new ListBoxForm("Instruments");
            lstboxformIbag = new ListBoxForm("ibag:s");
            lstboxformIgen = new ListBoxForm("igen:s");
            lstboxformImod = new ListBoxForm("imod:s");
            lstboxformShdr = new ListBoxForm("shdr:s");

            lstboxformInst.ListBoxFormItemSelected += Inst_LstBox_SelectedIndexChanged;
            lstboxformIbag.ListBoxFormItemSelected += IBAG_LstBox_SelectedIndexChanged;
            lstboxformIgen.ListBoxFormItemSelected += IGEN_LstBox_SelectedIndexChanged;
            lstboxformShdr.ListBoxFormItemSelected += SHDR_LstBox_SelectedIndexChanged;

            rtxtformLog.Show();
            rtxtformInfo.Show();
            Debug.rtxt = rtxtformLog.rtxt;
            addAllToThisForm();
        }

        private void addAllToThisForm()
        {
            rtxtformLog.TopLevel = false;
            rtxtformInfo.TopLevel = false;
            
            lstboxformInst.TopLevel = false;
            lstboxformIbag.TopLevel = false;
            lstboxformIgen.TopLevel = false;
            lstboxformShdr.TopLevel = false;
            this.Controls.Add(rtxtformInfo);
            this.Controls.Add(rtxtformLog);
            
            this.Controls.Add(lstboxformInst);
            this.Controls.Add(lstboxformIbag);
            this.Controls.Add(lstboxformIgen);
            this.Controls.Add(lstboxformShdr);
        }

        bool skipIGEN_LstBox_SelectedIndexChanged = false;
        bool skipIBAG_LstBox_SelectedIndexChanged = false;
        bool skipSHDR_LstBox_SelectedIndexChanged = false;
        private void Inst_LstBox_SelectedIndexChanged(object sender, ListItemWithIndex item)
        {
            skipIBAG_LstBox_SelectedIndexChanged = true;
            skipIGEN_LstBox_SelectedIndexChanged = true;
            skipSHDR_LstBox_SelectedIndexChanged = true;

            lstboxformIbag.lstBox.ClearSelected();
            lstboxformIgen.lstBox.ClearSelected();
            lstboxformShdr.lstBox.ClearSelected();
            int selectedIndex = item.index;
            
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
            InstrumentSelected(selectedIndex);
            
            int sampleCount = sfReader.getInstrumenSampleCount(selectedIndex);
            Debug.rtxt.AppendLine("sample count:" + sampleCount.ToString());
            skipIBAG_LstBox_SelectedIndexChanged = false;
            skipIGEN_LstBox_SelectedIndexChanged = false;
            skipSHDR_LstBox_SelectedIndexChanged = false;
            //lstboxformInst.lstBox.SelectionMode = SelectionMode.MultiExtended;
        }
        
        private void IBAG_LstBox_SelectedIndexChanged(object sender, ListItemWithIndex item)
        {
            if (skipIBAG_LstBox_SelectedIndexChanged) return;
            skipIGEN_LstBox_SelectedIndexChanged = true;
            skipSHDR_LstBox_SelectedIndexChanged = true;
            lstboxformIgen.lstBox.ClearSelected();
            lstboxformShdr.lstBox.ClearSelected();
            IbagSelected(item.index);
            skipIGEN_LstBox_SelectedIndexChanged = false;
            skipSHDR_LstBox_SelectedIndexChanged = false;
        }

        private void IGEN_LstBox_SelectedIndexChanged(object sender, ListItemWithIndex item)
        {
            if (skipIGEN_LstBox_SelectedIndexChanged) return;
            skipSHDR_LstBox_SelectedIndexChanged = true;
            lstboxformShdr.lstBox.ClearSelected();
            IgenSelected(item.index);
            skipSHDR_LstBox_SelectedIndexChanged = false;
        }

        private void SHDR_LstBox_SelectedIndexChanged(object sender, ListItemWithIndex item)
        {
            if (skipSHDR_LstBox_SelectedIndexChanged) return;
        }

        private void InstrumentSelected(int index)
        {
            pdta_rec pdta = sfReader.fileData.sfbk.pdta;
            int start = pdta.inst[index].wInstBagNdx;
            int end = pdta.inst[index + 1].wInstBagNdx;

            for (int i = start; i < end; i++)
            {
                lstboxformIbag.lstBox.SetSelected(i, true);
                IbagSelected(i);
            }
        }
        private void IbagSelected(int index)
        {
            pdta_rec pdta = sfReader.fileData.sfbk.pdta;
            int start = pdta.ibag[index].wGenNdx;
            int end = pdta.ibag[index + 1].wGenNdx;
            for (int i = start; i < end; i++)
            {
                lstboxformIgen.lstBox.SetSelected(i, true);
                IgenSelected(i);
            }
        }

        private void IgenSelected(int index)
        {
            pdta_rec pdta = sfReader.fileData.sfbk.pdta;
            if (pdta.igen[index].sfGenOper == SFGenerator.sampleID)
            {
                int sampleIndex = pdta.igen[index].genAmount.UAmount;
                lstboxformShdr.lstBox.SetSelected(sampleIndex, true);
            }
        }

        private void ReadAndShowFile(string filePath)
        {
            Debug.rtxt.Clear();
            if (sfReader.readFile(filePath) == false)
            {
                Debug.rtxt.AppendLine(sfReader.lastError);
                //return;
            }
            sfbk_rec sfbk = sfReader.fileData.sfbk;
            rtxtformInfo.rtxt.Clear();
            rtxtformInfo.rtxt.AppendLine(sfbk.info.ToString());
            rtxtformInfo.rtxt.AppendLine(sfbk.sdta.ToString());

            pdta_rec pdta = sfbk.pdta;

            rtxtformInfo.Show();
            lstboxformInst.Show("(" + pdta.inst.Length + ")", pdta.inst.GetAllToStringsAsArray());
            lstboxformIbag.Show("(" + pdta.ibag.Length + ")", pdta.ibag.GetAllToStringsAsArray());
            lstboxformIgen.Show("(" + pdta.igen.Length + ")", pdta.igen.GetAllToStringsAsArray());
            //lstboxformImod.Show("(" + pdta.imod.Length+")", pdta.imod.GetAllToStringsAsArray());
            lstboxformShdr.Show("(" + pdta.shdr.Length + ")", pdta.shdr.GetAllToStringsAsArray());

        }

        public enum SFGeneratorItemType
        {
            ushort_t,
            short_t,
            two_bytes
        }

        private void generateSFGeneratorTypeTestListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string code = "";
            string[] names = Enum.GetNames(typeof(SFGenerator));
            code = "if (type == SFGenerator." + names[0] + ")" + Environment.NewLine + "{" + Environment.NewLine + Environment.NewLine + "}" + Environment.NewLine;
            for (int i = 1; i < names.Length; i++)
            {
                code += "else if (type == SFGenerator." + names[i] + ")" + Environment.NewLine + "{" + Environment.NewLine + Environment.NewLine + "}" + Environment.NewLine;
            }
            Debug.rtxt.Text = code;
        }

        private void exportToCppTestinDevelopmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int instrumentIndex = lstboxformInst.lstBox.SelectedIndex;
            if (instrumentIndex == -1) { MessageBox.Show("you have not selected any instrument!"); return; }
            Soundfont2to_cpp.CodeFiles files = Soundfont2to_cpp.getcpp(sfReader.fileData.sfbk, instrumentIndex);
            rtxtformCppOutput.Show(files.ToString());
        }

        private void devTestOpenDirectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadAndShowFile(soundFontsRootDir + @"\AWE ROM gm.sf2");
        }

        private void openAnyFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
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

        private void MainForm_Shown(object sender, EventArgs e)
        {
            
            UpdateSizes();
            int newWidth = rtxtformLog.Width + lstboxformShdr.Width + 20;
            int newHeight = (int)((double)Screen.PrimaryScreen.Bounds.Height * 0.8f);
            this.Left -= (newWidth - this.Width)/2; // center screen
            this.Top -= (newHeight - this.Height) / 2;
            this.Width = newWidth;
            this.Height = newHeight;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            UpdateSizes();
        }
        private void UpdateSizes()
        {
            globalHeigth = this.Height / 2 - 35;
            rtxtformInfo.Left = leftOffset;
            rtxtformInfo.Top = topOffset;
            rtxtformInfo.Height = globalHeigth;
            rtxtformInfo.Width = 320;
            rtxtformLog.Left = leftOffset;
            rtxtformLog.Top = rtxtformInfo.Bottom;
            rtxtformLog.Width = 320;
            rtxtformLog.Height = globalHeigth;

            lstboxformInst.Width = 320;
            lstboxformInst.Height = globalHeigth;
            lstboxformIbag.Width = 275;
            lstboxformIbag.Height = globalHeigth;
            lstboxformIgen.Width = 515;
            lstboxformIgen.Height = globalHeigth;
            lstboxformShdr.Width = 1110;
            lstboxformShdr.Height = globalHeigth;
            lstboxformInst.Top = topOffset;
            lstboxformInst.Left = rtxtformInfo.Right;
            lstboxformIbag.Top = topOffset;
            lstboxformIbag.Left = lstboxformInst.Right;
            lstboxformIgen.Top = topOffset;
            lstboxformIgen.Left = lstboxformIbag.Right;
            lstboxformShdr.Top = lstboxformInst.Bottom;
            lstboxformShdr.Left = rtxtformLog.Right;
        }
    }
}

public static class Debug
{
    public static RichTextBox rtxt;
}