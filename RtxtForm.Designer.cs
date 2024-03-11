
namespace Soundfont2Tool
{
    partial class RtxtForm
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
            this.rtxt = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtxt
            // 
            this.rtxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxt.Location = new System.Drawing.Point(12, 12);
            this.rtxt.Name = "rtxt";
            this.rtxt.Size = new System.Drawing.Size(776, 426);
            this.rtxt.TabIndex = 0;
            this.rtxt.Text = "";
            // 
            // RtxtForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.rtxt);
            this.Name = "RtxtForm";
            this.Text = "RtxtForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RtxtForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox rtxt;
    }
}