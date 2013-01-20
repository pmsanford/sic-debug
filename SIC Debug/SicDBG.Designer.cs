namespace SIC_Debug
{
    partial class SicDBG
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
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.tbStart = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbEnd = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBkPt = new System.Windows.Forms.Button();
            this.tbBkPt = new System.Windows.Forms.TextBox();
            this.lstBkpt = new System.Windows.Forms.ListBox();
            this.btnDelPt = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.tbRunAddr = new System.Windows.Forms.TextBox();
            this.btnView = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allowWritingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearMemoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lstInstructions = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnStep = new System.Windows.Forms.Button();
            this.cbFT = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnLoadEXT = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbOutput
            // 
            this.tbOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbOutput.Location = new System.Drawing.Point(12, 56);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutput.Size = new System.Drawing.Size(350, 295);
            this.tbOutput.TabIndex = 0;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(368, 28);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // tbStart
            // 
            this.tbStart.Location = new System.Drawing.Point(75, 30);
            this.tbStart.Name = "tbStart";
            this.tbStart.Size = new System.Drawing.Size(61, 20);
            this.tbStart.TabIndex = 2;
            this.tbStart.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Start Addr:";
            // 
            // tbEnd
            // 
            this.tbEnd.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbEnd.Location = new System.Drawing.Point(220, 30);
            this.tbEnd.Name = "tbEnd";
            this.tbEnd.Size = new System.Drawing.Size(61, 20);
            this.tbEnd.TabIndex = 2;
            this.tbEnd.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(160, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "End Addr:";
            // 
            // btnBkPt
            // 
            this.btnBkPt.Location = new System.Drawing.Point(368, 217);
            this.btnBkPt.Name = "btnBkPt";
            this.btnBkPt.Size = new System.Drawing.Size(75, 23);
            this.btnBkPt.TabIndex = 4;
            this.btnBkPt.Text = "Add Bkpt";
            this.btnBkPt.UseVisualStyleBackColor = true;
            this.btnBkPt.Click += new System.EventHandler(this.btnBkPt_Click);
            // 
            // tbBkPt
            // 
            this.tbBkPt.Location = new System.Drawing.Point(368, 191);
            this.tbBkPt.Name = "tbBkPt";
            this.tbBkPt.Size = new System.Drawing.Size(75, 20);
            this.tbBkPt.TabIndex = 5;
            // 
            // lstBkpt
            // 
            this.lstBkpt.FormattingEnabled = true;
            this.lstBkpt.Location = new System.Drawing.Point(368, 246);
            this.lstBkpt.Name = "lstBkpt";
            this.lstBkpt.Size = new System.Drawing.Size(75, 108);
            this.lstBkpt.TabIndex = 6;
            // 
            // btnDelPt
            // 
            this.btnDelPt.Location = new System.Drawing.Point(368, 360);
            this.btnDelPt.Name = "btnDelPt";
            this.btnDelPt.Size = new System.Drawing.Size(75, 23);
            this.btnDelPt.TabIndex = 7;
            this.btnDelPt.Text = "Del Bkpt";
            this.btnDelPt.UseVisualStyleBackColor = true;
            this.btnDelPt.Click += new System.EventHandler(this.btnDelPt_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(368, 135);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 8;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // tbRunAddr
            // 
            this.tbRunAddr.Location = new System.Drawing.Point(368, 109);
            this.tbRunAddr.Name = "tbRunAddr";
            this.tbRunAddr.Size = new System.Drawing.Size(75, 20);
            this.tbRunAddr.TabIndex = 9;
            this.tbRunAddr.Text = "0";
            // 
            // btnView
            // 
            this.btnView.Location = new System.Drawing.Point(287, 28);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(75, 23);
            this.btnView.TabIndex = 10;
            this.btnView.Text = "View";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(369, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Entry Pt:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(696, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // filesToolStripMenuItem
            // 
            this.filesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFilesToolStripMenuItem,
            this.allowWritingToolStripMenuItem,
            this.toolStripSeparator1,
            this.clearMemoryToolStripMenuItem});
            this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            this.filesToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.filesToolStripMenuItem.Text = "Files";
            // 
            // openFilesToolStripMenuItem
            // 
            this.openFilesToolStripMenuItem.Name = "openFilesToolStripMenuItem";
            this.openFilesToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.openFilesToolStripMenuItem.Text = "Open Files";
            this.openFilesToolStripMenuItem.Click += new System.EventHandler(this.openFilesToolStripMenuItem_Click);
            // 
            // allowWritingToolStripMenuItem
            // 
            this.allowWritingToolStripMenuItem.Checked = true;
            this.allowWritingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.allowWritingToolStripMenuItem.Name = "allowWritingToolStripMenuItem";
            this.allowWritingToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.allowWritingToolStripMenuItem.Text = "Allow Writing";
            this.allowWritingToolStripMenuItem.Click += new System.EventHandler(this.allowWritingToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(146, 6);
            // 
            // clearMemoryToolStripMenuItem
            // 
            this.clearMemoryToolStripMenuItem.Name = "clearMemoryToolStripMenuItem";
            this.clearMemoryToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.clearMemoryToolStripMenuItem.Text = "Clear Memory";
            this.clearMemoryToolStripMenuItem.Click += new System.EventHandler(this.clearMemoryToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.helpToolStripMenuItem.Text = "About";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // lstInstructions
            // 
            this.lstInstructions.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstInstructions.FormattingEnabled = true;
            this.lstInstructions.Location = new System.Drawing.Point(450, 56);
            this.lstInstructions.Name = "lstInstructions";
            this.lstInstructions.Size = new System.Drawing.Size(234, 290);
            this.lstInstructions.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(449, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Last instructions:";
            // 
            // btnStep
            // 
            this.btnStep.Location = new System.Drawing.Point(368, 165);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(75, 23);
            this.btnStep.TabIndex = 15;
            this.btnStep.Text = "Step";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // cbFT
            // 
            this.cbFT.AutoSize = true;
            this.cbFT.Location = new System.Drawing.Point(604, 36);
            this.cbFT.Name = "cbFT";
            this.cbFT.Size = new System.Drawing.Size(73, 17);
            this.cbFT.TabIndex = 16;
            this.cbFT.Text = "Full Trace";
            this.cbFT.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 357);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "Clear Output";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnLoadEXT
            // 
            this.btnLoadEXT.Location = new System.Drawing.Point(368, 57);
            this.btnLoadEXT.Name = "btnLoadEXT";
            this.btnLoadEXT.Size = new System.Drawing.Size(75, 23);
            this.btnLoadEXT.TabIndex = 1;
            this.btnLoadEXT.Text = "Load Ext";
            this.btnLoadEXT.UseVisualStyleBackColor = true;
            this.btnLoadEXT.Click += new System.EventHandler(this.btnLoadEXT_Click);
            // 
            // SicDBG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 388);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cbFT);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lstInstructions);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.tbRunAddr);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnDelPt);
            this.Controls.Add(this.lstBkpt);
            this.Controls.Add(this.tbBkPt);
            this.Controls.Add(this.btnBkPt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbEnd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbStart);
            this.Controls.Add(this.btnLoadEXT);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SicDBG";
            this.Text = "SIC Debug";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TextBox tbStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbEnd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBkPt;
        private System.Windows.Forms.TextBox tbBkPt;
        private System.Windows.Forms.ListBox lstBkpt;
        private System.Windows.Forms.Button btnDelPt;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TextBox tbRunAddr;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFilesToolStripMenuItem;
        private System.Windows.Forms.ListBox lstInstructions;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.CheckBox cbFT;
        private System.Windows.Forms.ToolStripMenuItem allowWritingToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem clearMemoryToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Button btnLoadEXT;
    }
}

