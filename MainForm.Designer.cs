
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
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.rtxt = new System.Windows.Forms.RichTextBox();
            this.btnListFilesInFir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(13, 13);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFile.TabIndex = 0;
            this.btnOpenFile.Text = "Open File";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // rtxt
            // 
            this.rtxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxt.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxt.Location = new System.Drawing.Point(12, 42);
            this.rtxt.Name = "rtxt";
            this.rtxt.Size = new System.Drawing.Size(776, 396);
            this.rtxt.TabIndex = 1;
            this.rtxt.Text = "";
            // 
            // btnListFilesInFir
            // 
            this.btnListFilesInFir.Location = new System.Drawing.Point(367, 13);
            this.btnListFilesInFir.Name = "btnListFilesInFir";
            this.btnListFilesInFir.Size = new System.Drawing.Size(75, 23);
            this.btnListFilesInFir.TabIndex = 2;
            this.btnListFilesInFir.Text = "list files in dir";
            this.btnListFilesInFir.UseVisualStyleBackColor = true;
            this.btnListFilesInFir.Click += new System.EventHandler(this.btnListFilesInFir_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnListFilesInFir);
            this.Controls.Add(this.rtxt);
            this.Controls.Add(this.btnOpenFile);
            this.Name = "MainForm";
            this.Text = "Soundfont 2 tool";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.RichTextBox rtxt;
        private System.Windows.Forms.Button btnListFilesInFir;
    }
}

