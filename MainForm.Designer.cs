
namespace Soundfont2Tool
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openAnyFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.devTestOpenDirectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToCppTestinDevelopmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otherDevToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateSFGeneratorTypeTestListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openAnyFileToolStripMenuItem,
            this.devTestOpenDirectToolStripMenuItem,
            this.exportToCppTestinDevelopmentToolStripMenuItem,
            this.otherDevToolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(816, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openAnyFileToolStripMenuItem
            // 
            this.openAnyFileToolStripMenuItem.Name = "openAnyFileToolStripMenuItem";
            this.openAnyFileToolStripMenuItem.Size = new System.Drawing.Size(89, 20);
            this.openAnyFileToolStripMenuItem.Text = "Open any file";
            this.openAnyFileToolStripMenuItem.Click += new System.EventHandler(this.openAnyFileToolStripMenuItem_Click);
            // 
            // devTestOpenDirectToolStripMenuItem
            // 
            this.devTestOpenDirectToolStripMenuItem.Name = "devTestOpenDirectToolStripMenuItem";
            this.devTestOpenDirectToolStripMenuItem.Size = new System.Drawing.Size(123, 20);
            this.devTestOpenDirectToolStripMenuItem.Text = "dev test open direct";
            this.devTestOpenDirectToolStripMenuItem.Click += new System.EventHandler(this.devTestOpenDirectToolStripMenuItem_Click);
            // 
            // exportToCppTestinDevelopmentToolStripMenuItem
            // 
            this.exportToCppTestinDevelopmentToolStripMenuItem.Name = "exportToCppTestinDevelopmentToolStripMenuItem";
            this.exportToCppTestinDevelopmentToolStripMenuItem.Size = new System.Drawing.Size(206, 20);
            this.exportToCppTestinDevelopmentToolStripMenuItem.Text = "export to cpp test (in development)";
            this.exportToCppTestinDevelopmentToolStripMenuItem.Click += new System.EventHandler(this.exportToCppTestinDevelopmentToolStripMenuItem_Click);
            // 
            // otherDevToolsToolStripMenuItem
            // 
            this.otherDevToolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateSFGeneratorTypeTestListToolStripMenuItem});
            this.otherDevToolsToolStripMenuItem.Name = "otherDevToolsToolStripMenuItem";
            this.otherDevToolsToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.otherDevToolsToolStripMenuItem.Text = "other dev tools";
            // 
            // generateSFGeneratorTypeTestListToolStripMenuItem
            // 
            this.generateSFGeneratorTypeTestListToolStripMenuItem.Name = "generateSFGeneratorTypeTestListToolStripMenuItem";
            this.generateSFGeneratorTypeTestListToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.generateSFGeneratorTypeTestListToolStripMenuItem.Text = "generate SFGenerator type test list";
            this.generateSFGeneratorTypeTestListToolStripMenuItem.Click += new System.EventHandler(this.generateSFGeneratorTypeTestListToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 569);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Soundfont 2 tool";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openAnyFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem devTestOpenDirectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToCppTestinDevelopmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem otherDevToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateSFGeneratorTypeTestListToolStripMenuItem;
    }
}

