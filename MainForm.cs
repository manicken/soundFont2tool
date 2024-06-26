﻿using System;
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
        const int rtxtformInfo_Width = 320;
        const int rtxtformLog_Width = 320;
        const int lstboxformInst_Width = 320;
        const int lstboxformIbag_Width = 275;
        const int lstboxformIgen_Width = 515;
        const int lstboxformShdr_Width = 1110;
        const int lstboxformPhdr_Width = 750;

        string soundFontsRootDir = @"G:\_git\__TEENSY\SF2_SoundFonts-master";
        Soundfont2_reader sfReader;

        private RichTextBoxForm rtxtformInfo;
        private RichTextBoxForm rtxtformLog;
        private FastColoredTextBoxForm rtxtformCppOutput;
        private FastColoredTextBoxForm rtxtformHppOutput;

        private ListBoxForm lstboxformInst;
        private ListBoxForm lstboxformIbag;
        private ListBoxForm lstboxformIgen;
        private ListBoxForm lstboxformImod;
        private ListBoxForm lstboxformShdr;


        private Form presetDataForms;
        private ListBoxForm lstboxformPhdr;
        private ListBoxForm lstboxformPbag;
        private ListBoxForm lstboxformPgen;
        private ListBoxForm lstboxformPmod;

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
            rtxtformCppOutput = new FastColoredTextBoxForm("Export to Teensy - Development Test - cpp file");
            rtxtformCppOutput.StartPosition = FormStartPosition.CenterScreen;
            rtxtformHppOutput = new FastColoredTextBoxForm("Export to Teensy - Development Test - h file");
            rtxtformHppOutput.StartPosition = FormStartPosition.CenterScreen;

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

            // preset data view
            lstboxformPhdr = new ListBoxForm("phdr:s");
            lstboxformPbag = new ListBoxForm("pbag:s");
            lstboxformPgen = new ListBoxForm("pgen:s");
            lstboxformPmod = new ListBoxForm("pmod:s");

            presetDataForms = new Form();
            presetDataForms.Text = "Presets";
            presetDataForms.IsMdiContainer = true;
            presetDataForms.AddToMdiForm(new Form[] { lstboxformPhdr, lstboxformPbag, lstboxformPgen, lstboxformPmod });
            presetDataForms.FormClosing += PresetDataForms_FormClosing;
            presetDataForms.Shown += PresetDataForms_Shown;
        }

        private void PresetDataForms_Shown(object sender, EventArgs e)
        {
            lstboxformPhdr.Width = rtxtformInfo_Width + lstboxformInst_Width;
            lstboxformPgen.Width = lstboxformIgen_Width;
            lstboxformPbag.Width = lstboxformIbag_Width;
            lstboxformPbag.Left = lstboxformPhdr.Right;
            lstboxformPgen.Left = lstboxformPbag.Right;

            lstboxformPhdr.Height = globalHeigth;
            lstboxformPgen.Height = globalHeigth;
            lstboxformPbag.Height = globalHeigth;
        }

        private void PresetDataForms_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                presetDataForms.Visible = false;
            }
                
        }

        private void addAllToThisForm()
        {
            this.AddToMdiForm(new Form[]{ rtxtformLog, rtxtformInfo, lstboxformInst, lstboxformIbag, lstboxformIgen, lstboxformShdr });
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
            
            //int sampleCount = sfReader.getInstrumenSampleCount(selectedIndex);
            //int sampleTotalSize = sfReader.getInstrumentTotalSampleSize(selectedIndex);
            //Debug.rtxt.AppendLine("sample count:" + sampleCount.ToString() + ", sample size:" + (sampleTotalSize*2).ToString());
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
            lstboxformIgen.lstBox.ClearSelected();
            for (int i = 0; i < sfReader.fileData.sfbk.pdta.igen.Length; i++)
            {
                if (sfReader.fileData.sfbk.pdta.igen[i].sfGenOper == SFGenerator.sampleID)
                {
                    if (sfReader.fileData.sfbk.pdta.igen[i].genAmount.UAmount == item.index)
                    {
                        lstboxformIgen.lstBox.SetSelected(i, true);
                    }
                }

            }
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
            string[] instList = pdta.inst.GetAllToStringsAsArray();
            for (int i = 0; i < instList.Length-1; i++)
                instList[i] += $", sample count:{sfReader.getInstrumenSampleCount(i).ToString().PadLeft(2)}, sampleSize(bytes):{(sfReader.getInstrumentTotalSampleSize(i)*2).ToString().PadLeft(8)}";
            lstboxformInst.Show("(" + pdta.inst.Length + ")", instList);
            lstboxformIbag.Show("(" + pdta.ibag.Length + ")", pdta.ibag.GetAllToStringsAsArray());
            lstboxformIgen.Show("(" + pdta.igen.Length + ")", pdta.igen.GetAllToStringsAsArray());
            //lstboxformImod.Show("(" + pdta.imod.Length+")", pdta.imod.GetAllToStringsAsArray());
            lstboxformShdr.Show("(" + pdta.shdr.Length + ")", pdta.shdr.GetAllToStringsAsArray());

            presetDataForms.Show();
            lstboxformPhdr.Show("(" + pdta.phdr.Length + ")", pdta.phdr.GetAllToStringsAsArray());
            lstboxformPbag.Show("(" + pdta.pbag.Length + ")", pdta.pbag.GetAllToStringsAsArray());
            lstboxformPgen.Show("(" + pdta.pgen.Length + ")", pdta.pgen.GetAllToStringsAsArray());
            //lstboxformPmod.Show("(" + pdta.pmod.Length+")", pdta.pmod.GetAllToStringsAsArray());
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
            Soundfont2to_cpp.CodeFiles files = Soundfont2to_cpp.getcpp(sfReader, instrumentIndex);
            rtxtformHppOutput.Show(files.hpp.data);
            rtxtformHppOutput.Text = files.hpp.fileName;
            rtxtformHppOutput.Top -= 32;
            rtxtformCppOutput.Show(files.cpp.data);
            rtxtformCppOutput.Text = files.cpp.fileName;
            
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
                ofd.Filter = "Soundfont2 files (*.sf2)|*.sf2|Soundfont3 files (*.sf3)|*.sf3";
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

            presetDataForms.Left = this.Left;
            presetDataForms.Top = this.Top;
            presetDataForms.Width = newWidth;
            presetDataForms.Height = globalHeigth + 45;
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
            rtxtformInfo.Width = rtxtformInfo_Width;
            rtxtformLog.Left = leftOffset;
            rtxtformLog.Top = rtxtformInfo.Bottom;
            rtxtformLog.Width = rtxtformLog_Width;
            rtxtformLog.Height = globalHeigth;

            lstboxformInst.Width = lstboxformInst_Width;
            lstboxformInst.Height = globalHeigth;
            lstboxformIbag.Width = lstboxformIbag_Width;
            lstboxformIbag.Height = globalHeigth;
            lstboxformIgen.Width = lstboxformIgen_Width;
            lstboxformIgen.Height = globalHeigth;
            lstboxformShdr.Width = lstboxformShdr_Width;
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