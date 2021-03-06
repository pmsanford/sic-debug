﻿namespace SIC_Debug
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
            this.btnBkPt = new System.Windows.Forms.Button();
            this.tbBkPt = new System.Windows.Forms.TextBox();
            this.lstBkpt = new System.Windows.Forms.ListBox();
            this.btnDelPt = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.tbRunAddr = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allowWritingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearMemoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lstInstructions = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnStep = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnLoadEXT = new System.Windows.Forms.Button();
            this.tbRegA = new System.Windows.Forms.TextBox();
            this.tbRegB = new System.Windows.Forms.TextBox();
            this.tbRegX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbRegL = new System.Windows.Forms.TextBox();
            this.tbRegS = new System.Windows.Forms.TextBox();
            this.tbRegT = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tbPC = new System.Windows.Forms.TextBox();
            this.tbSW = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.hbMemory = new Be.Windows.Forms.HexBox();
            this.btnBreak = new System.Windows.Forms.Button();
            this.mnuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbOutput
            // 
            this.tbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbOutput.Location = new System.Drawing.Point(12, 415);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutput.Size = new System.Drawing.Size(579, 74);
            this.tbOutput.TabIndex = 0;
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(597, 28);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnBkPt
            // 
            this.btnBkPt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBkPt.Location = new System.Drawing.Point(597, 214);
            this.btnBkPt.Name = "btnBkPt";
            this.btnBkPt.Size = new System.Drawing.Size(75, 23);
            this.btnBkPt.TabIndex = 4;
            this.btnBkPt.Text = "Add Bkpt";
            this.btnBkPt.UseVisualStyleBackColor = true;
            this.btnBkPt.Click += new System.EventHandler(this.btnBkPt_Click);
            // 
            // tbBkPt
            // 
            this.tbBkPt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBkPt.Location = new System.Drawing.Point(597, 191);
            this.tbBkPt.Name = "tbBkPt";
            this.tbBkPt.Size = new System.Drawing.Size(75, 20);
            this.tbBkPt.TabIndex = 5;
            // 
            // lstBkpt
            // 
            this.lstBkpt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstBkpt.FormattingEnabled = true;
            this.lstBkpt.Location = new System.Drawing.Point(597, 272);
            this.lstBkpt.Name = "lstBkpt";
            this.lstBkpt.Size = new System.Drawing.Size(75, 212);
            this.lstBkpt.TabIndex = 6;
            // 
            // btnDelPt
            // 
            this.btnDelPt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelPt.Location = new System.Drawing.Point(597, 498);
            this.btnDelPt.Name = "btnDelPt";
            this.btnDelPt.Size = new System.Drawing.Size(75, 23);
            this.btnDelPt.TabIndex = 7;
            this.btnDelPt.Text = "Del Bkpt";
            this.btnDelPt.UseVisualStyleBackColor = true;
            this.btnDelPt.Click += new System.EventHandler(this.btnDelPt_Click);
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Location = new System.Drawing.Point(597, 135);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 8;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // tbRunAddr
            // 
            this.tbRunAddr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRunAddr.Location = new System.Drawing.Point(597, 109);
            this.tbRunAddr.Name = "tbRunAddr";
            this.tbRunAddr.Size = new System.Drawing.Size(75, 20);
            this.tbRunAddr.TabIndex = 9;
            this.tbRunAddr.Text = "0";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(598, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Entry Pt:";
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(926, 24);
            this.mnuMain.TabIndex = 12;
            this.mnuMain.Text = "menuStrip1";
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
            this.openFilesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openFilesToolStripMenuItem.Text = "Open Files";
            this.openFilesToolStripMenuItem.Click += new System.EventHandler(this.openFilesToolStripMenuItem_Click);
            // 
            // allowWritingToolStripMenuItem
            // 
            this.allowWritingToolStripMenuItem.Checked = true;
            this.allowWritingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.allowWritingToolStripMenuItem.Name = "allowWritingToolStripMenuItem";
            this.allowWritingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.allowWritingToolStripMenuItem.Text = "Allow Writing";
            this.allowWritingToolStripMenuItem.Click += new System.EventHandler(this.allowWritingToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // clearMemoryToolStripMenuItem
            // 
            this.clearMemoryToolStripMenuItem.Name = "clearMemoryToolStripMenuItem";
            this.clearMemoryToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
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
            this.lstInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstInstructions.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstInstructions.FormattingEnabled = true;
            this.lstInstructions.Location = new System.Drawing.Point(679, 56);
            this.lstInstructions.Name = "lstInstructions";
            this.lstInstructions.Size = new System.Drawing.Size(234, 446);
            this.lstInstructions.TabIndex = 13;
            this.lstInstructions.SelectedIndexChanged += new System.EventHandler(this.lstInstructions_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(678, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Last instructions:";
            // 
            // btnStep
            // 
            this.btnStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStep.Location = new System.Drawing.Point(597, 165);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(75, 23);
            this.btnStep.TabIndex = 15;
            this.btnStep.Text = "Step";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(12, 495);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "Clear Output";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnLoadEXT
            // 
            this.btnLoadEXT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadEXT.Location = new System.Drawing.Point(597, 57);
            this.btnLoadEXT.Name = "btnLoadEXT";
            this.btnLoadEXT.Size = new System.Drawing.Size(75, 23);
            this.btnLoadEXT.TabIndex = 1;
            this.btnLoadEXT.Text = "Load Ext";
            this.btnLoadEXT.UseVisualStyleBackColor = true;
            this.btnLoadEXT.Click += new System.EventHandler(this.btnLoadEXT_Click);
            // 
            // tbRegA
            // 
            this.tbRegA.Location = new System.Drawing.Point(33, 25);
            this.tbRegA.Name = "tbRegA";
            this.tbRegA.ReadOnly = true;
            this.tbRegA.Size = new System.Drawing.Size(45, 20);
            this.tbRegA.TabIndex = 18;
            this.tbRegA.Text = "000000";
            // 
            // tbRegB
            // 
            this.tbRegB.Location = new System.Drawing.Point(107, 25);
            this.tbRegB.Name = "tbRegB";
            this.tbRegB.ReadOnly = true;
            this.tbRegB.Size = new System.Drawing.Size(45, 20);
            this.tbRegB.TabIndex = 18;
            this.tbRegB.Text = "000000";
            // 
            // tbRegX
            // 
            this.tbRegX.Location = new System.Drawing.Point(181, 25);
            this.tbRegX.Name = "tbRegX";
            this.tbRegX.ReadOnly = true;
            this.tbRegX.Size = new System.Drawing.Size(45, 20);
            this.tbRegX.TabIndex = 18;
            this.tbRegX.Text = "000000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "A:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(84, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "B:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(158, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "X:";
            // 
            // tbRegL
            // 
            this.tbRegL.Location = new System.Drawing.Point(33, 51);
            this.tbRegL.Name = "tbRegL";
            this.tbRegL.ReadOnly = true;
            this.tbRegL.Size = new System.Drawing.Size(45, 20);
            this.tbRegL.TabIndex = 18;
            this.tbRegL.Text = "000000";
            // 
            // tbRegS
            // 
            this.tbRegS.Location = new System.Drawing.Point(107, 51);
            this.tbRegS.Name = "tbRegS";
            this.tbRegS.ReadOnly = true;
            this.tbRegS.Size = new System.Drawing.Size(45, 20);
            this.tbRegS.TabIndex = 18;
            this.tbRegS.Text = "000000";
            // 
            // tbRegT
            // 
            this.tbRegT.Location = new System.Drawing.Point(181, 51);
            this.tbRegT.Name = "tbRegT";
            this.tbRegT.ReadOnly = true;
            this.tbRegT.Size = new System.Drawing.Size(45, 20);
            this.tbRegT.TabIndex = 18;
            this.tbRegT.Text = "000000";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "L:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(84, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "S:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(158, 54);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "T:";
            // 
            // tbPC
            // 
            this.tbPC.Location = new System.Drawing.Point(326, 25);
            this.tbPC.Name = "tbPC";
            this.tbPC.ReadOnly = true;
            this.tbPC.Size = new System.Drawing.Size(45, 20);
            this.tbPC.TabIndex = 18;
            this.tbPC.Text = "000000";
            // 
            // tbSW
            // 
            this.tbSW.Location = new System.Drawing.Point(315, 51);
            this.tbSW.Name = "tbSW";
            this.tbSW.ReadOnly = true;
            this.tbSW.Size = new System.Drawing.Size(56, 20);
            this.tbSW.TabIndex = 18;
            this.tbSW.Text = "00000000";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(231, 28);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "Program Counter:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(240, 54);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(69, 13);
            this.label12.TabIndex = 19;
            this.label12.Text = "Status Word:";
            // 
            // hbMemory
            // 
            this.hbMemory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hbMemory.ColumnInfoVisible = true;
            this.hbMemory.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hbMemory.InfoForeColor = System.Drawing.Color.Gray;
            this.hbMemory.LineInfoVisible = true;
            this.hbMemory.Location = new System.Drawing.Point(15, 77);
            this.hbMemory.Name = "hbMemory";
            this.hbMemory.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hbMemory.Size = new System.Drawing.Size(576, 332);
            this.hbMemory.StringViewVisible = true;
            this.hbMemory.TabIndex = 20;
            this.hbMemory.UseFixedBytesPerLine = true;
            this.hbMemory.VScrollBarVisible = true;
            // 
            // btnBreak
            // 
            this.btnBreak.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBreak.Enabled = false;
            this.btnBreak.Location = new System.Drawing.Point(597, 243);
            this.btnBreak.Name = "btnBreak";
            this.btnBreak.Size = new System.Drawing.Size(75, 23);
            this.btnBreak.TabIndex = 4;
            this.btnBreak.Text = "Break";
            this.btnBreak.UseVisualStyleBackColor = true;
            this.btnBreak.Click += new System.EventHandler(this.btnBreak_Click);
            // 
            // SicDBG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 526);
            this.Controls.Add(this.hbMemory);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbSW);
            this.Controls.Add(this.tbRegT);
            this.Controls.Add(this.tbPC);
            this.Controls.Add(this.tbRegX);
            this.Controls.Add(this.tbRegS);
            this.Controls.Add(this.tbRegB);
            this.Controls.Add(this.tbRegL);
            this.Controls.Add(this.tbRegA);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lstInstructions);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbRunAddr);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnDelPt);
            this.Controls.Add(this.lstBkpt);
            this.Controls.Add(this.tbBkPt);
            this.Controls.Add(this.btnBreak);
            this.Controls.Add(this.btnBkPt);
            this.Controls.Add(this.btnLoadEXT);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.mnuMain);
            this.MainMenuStrip = this.mnuMain;
            this.MinimumSize = new System.Drawing.Size(722, 426);
            this.Name = "SicDBG";
            this.Text = "SIC Debug";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SicDBG_FormClosing);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnBkPt;
        private System.Windows.Forms.TextBox tbBkPt;
        private System.Windows.Forms.ListBox lstBkpt;
        private System.Windows.Forms.Button btnDelPt;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TextBox tbRunAddr;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFilesToolStripMenuItem;
        private System.Windows.Forms.ListBox lstInstructions;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.ToolStripMenuItem allowWritingToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem clearMemoryToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Button btnLoadEXT;
        private System.Windows.Forms.TextBox tbRegA;
        private System.Windows.Forms.TextBox tbRegB;
        private System.Windows.Forms.TextBox tbRegX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbRegL;
        private System.Windows.Forms.TextBox tbRegS;
        private System.Windows.Forms.TextBox tbRegT;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbPC;
        private System.Windows.Forms.TextBox tbSW;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private Be.Windows.Forms.HexBox hbMemory;
        private System.Windows.Forms.Button btnBreak;
    }
}

