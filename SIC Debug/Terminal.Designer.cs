namespace SIC_Debug
{
    partial class Terminal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Terminal));
            this.rtbTerminal = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtbTerminal
            // 
            this.rtbTerminal.BackColor = System.Drawing.Color.Black;
            this.rtbTerminal.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbTerminal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbTerminal.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbTerminal.ForeColor = System.Drawing.Color.Lime;
            this.rtbTerminal.Location = new System.Drawing.Point(0, 0);
            this.rtbTerminal.Name = "rtbTerminal";
            this.rtbTerminal.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rtbTerminal.Size = new System.Drawing.Size(723, 456);
            this.rtbTerminal.TabIndex = 0;
            this.rtbTerminal.Text = resources.GetString("rtbTerminal.Text");
            // 
            // Terminal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 456);
            this.Controls.Add(this.rtbTerminal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Terminal";
            this.Text = "Terminal";
            this.Load += new System.EventHandler(this.Terminal_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbTerminal;
    }
}