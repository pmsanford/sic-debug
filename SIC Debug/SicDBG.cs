﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;


namespace SIC_Debug
{
    public partial class SicDBG : Form
    {
        SICVM vm;
        List<int> Breakpoints = new List<int>();
        Queue<Instruction> trace = new Queue<Instruction>();
        Queue<string> messages = new Queue<string>();
        int? lastBP = null;

        static List<char> HexChars = new List<char>(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', '\b' });

        long lastMemoryHighlight = -1;

        Dictionary<int, Highlight> activeHighlights = new Dictionary<int, Highlight>();

        Dictionary<int, Stack<Highlight>> oldHighlights = new Dictionary<int, Stack<Highlight>>();

        int prevInstructionIndex = -1;

        delegate void changeColorCallback(int startAddress, int length, Color foreColor, Color backColor);
        delegate void resetColorCallback(int startAddress);

        delegate void breakptHandlerCallback(SICEvent e);
        delegate void traceHandlerCallback(SICEvent e);
        delegate void stopOnFFCallback(SICEvent e);
        delegate void memoryChangeCallback(int address, int length);

        private bool breaker = false;

        Thread vmThread = null;

        public SicDBG()
        {
            InitializeComponent();
            vm = new SICVM();
            vm.PreInstructionHook += new SICVM.PreInstruction(breakptHandler);
            vm.PreInstructionHook += new SICVM.PreInstruction(stopOnFF);
            vm.PreInstructionHook += new SICVM.PreInstruction(userBreak);
            vm.PostInstructionHook += new SICVM.PostInstruction(traceHandler);
            vm.RunningStartedHook += new SICVM.RunningStarted(disableButtons);
            vm.RunningFinishedHook += new SICVM.RunningFinished(enableButtons);
            vm.RunningFinishedHook += new SICVM.RunningFinished(updateDisplay);
            hbMemory.ByteProvider = new MemoryByteProvider(vm.Memory);
            hbMemory.SelectionForeColor = Color.White;
            hbMemory.SelectionBackColor = Color.Blue;
            vm.MemoryChangedHook += new SICVM.MemoryChanged(memoryChange);
            vm.AllowWriting = true;
            SimpleCharacterTerminal term = new SimpleCharacterTerminal();
            vm.devices[6] = term;
            vmThread = null;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileStream infile;
                try
                {
                    infile = new FileStream(dialog.FileName, FileMode.Open);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to open file");
                    return;
                }
                StreamReader reader = new StreamReader(infile);
                string infilestr = reader.ReadToEnd();
                reader.Close();

                Tuple<int, int> addresses = vm.LoadObjectFile(infilestr);

                tbRunAddr.Text = string.Format("{0:X}", addresses.Item1);

                OutputMemdump();

                tbOutput.Text += System.Environment.NewLine;

                hbMemory.Refresh();
                hbMemory.ScrollByteIntoView(addresses.Item1);
            }
        }

        public void OutputMemdump()
        {
            OutputMemdump("");
        }

        public int CountNewlines(string input)
        {
            int count = 0;
            int start = 0;

            while ((start = input.IndexOf('\n', start)) != -1)
            {
                count++;
                start++;
            }
            return count;
        }

        public void OutputMemdump(string ErrorMsgs)
        {
            StringBuilder outstr = new StringBuilder(tbOutput.Text);
            int topofdump = tbOutput.Text.Length;
            int newlines = 8;
            byte[] memory = vm.Memory;
            newlines += CountNewlines(ErrorMsgs);
            outstr.AppendFormat("{0}{1}", ErrorMsgs, Environment.NewLine);
            if (!allowWritingToolStripMenuItem.Checked && vm.DeviceWritten)
            {
                outstr.AppendFormat("Note: Allow write not checked, but WD encountered. Output file not written to.{0}", Environment.NewLine);
                newlines++;
            }

            tbOutput.Text = outstr.ToString();
            tbOutput.SelectionStart = newlines >= 18 ? (topofdump - 1) : tbOutput.Text.Length;
            tbOutput.ScrollToCaret();
        }

        #region Event Handlers

        private void btnBkPt_Click(object sender, EventArgs e)
        {
            try
            {
                int newpt = Convert.ToInt32(tbBkPt.Text, 16);
                if (newpt < 0 || newpt > 1048575)
                {
                    MessageBox.Show("Breakpoint out of range.");
                }
                else
                {
                    Breakpoints.Add(newpt);
                    lstBkpt.Items.Add(string.Format("{0:X}", newpt));
                    tbBkPt.Text = "";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid address.");
            }
        }

        private void userBreak(SICEvent e)
        {
            if (breaker)
            {
                e.Continue = false;
                breaker = false;
            }
        }

        private void breakptHandler(SICEvent e)
        {
            if (InvokeRequired)
            {
                breakptHandlerCallback c = new breakptHandlerCallback(breakptHandler);
                Invoke(c, e);
                return;
            }
            if (Breakpoints.Contains(e.PC) && (lastBP != e.PC))
            {
                e.Continue = false;
                messages.Enqueue(string.Format("Breakpoint reached at 0x{0:X4}.", vm.ProgramCounter));
                tbRunAddr.Text = string.Format("{0:X}", vm.ProgramCounter);
                hbMemory.Refresh();
                hbMemory.ScrollByteIntoView(e.PC);
                changeColor(e.PC, e.instruction.length, Color.White, Color.DarkRed);
                lastBP = e.PC;
            }
            else if (lastBP != null)
            {
                resetColor((int)lastBP);
                lastBP = null;
            }
        }

        private void traceHandler(SICEvent e)
        {
            if (InvokeRequired)
            {
                traceHandlerCallback c = new traceHandlerCallback(traceHandler);
                Invoke(c, e);
                return;
            }
            else
            {
                if (trace.Count > 250)
                    trace.Dequeue();
                trace.Enqueue(e.instruction);
            }
        }

        private void stopOnFF(SICEvent e)
        {
            if (InvokeRequired)
            {
                stopOnFFCallback c = new stopOnFFCallback(stopOnFF);
                Invoke(c, e);
                return;
            }
            else
            {
                if (vm.Memory[e.PC] == 0xFF)
                {
                    e.Continue = false;
                }
            }
        }

        private void updateDisplay(SICEvent args)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => updateDisplay(args)));
            }
            else
            {
                StringBuilder builder = new StringBuilder();

                if (args.Message != null)
                    messages.Enqueue(args.Message);

                foreach (string msg in messages)
                    builder.AppendLine(msg);

                messages.Clear();

                OutputMemdump(builder.ToString());

                lstInstructions.Items.Clear();
                foreach (Instruction instruction in trace)
                {
                    lstInstructions.Items.Add(instruction);
                }
                lstInstructions.SelectedIndex = lstInstructions.Items.Count - 1;
                SetRegisters();
                hbMemory.Refresh();
            }
        }

        private void memoryChange(int address, int length)
        {
            if (vm.Running)
                return;
            if (InvokeRequired)
            {
                memoryChangeCallback c = new memoryChangeCallback(memoryChange);
                Invoke(c, address, length);
                return;
            }
            else
            {
                if (lastMemoryHighlight >= 0 && activeHighlights.Keys.Contains((int)lastMemoryHighlight))
                    resetColor((int)lastMemoryHighlight);
                lastMemoryHighlight = address;
                changeColor(address, length, Color.Red);
            }
        }

        private void changeColor(int startAddress, int length, Color foreColor)
        {
            if (this.hbMemory.InvokeRequired)
            {
                changeColorCallback c = new changeColorCallback(changeColor);
                this.Invoke(c, new object[] { startAddress, length, foreColor, Color.White });
            }
            else
            {
                changeColor(startAddress, length, foreColor, Color.White);
            }
        }

        private void changeColor(int startAddress, int length, Color foreColor, Color backColor)
        {
            if (this.hbMemory.InvokeRequired)
            {
                changeColorCallback c = new changeColorCallback(changeColor);
                this.Invoke(c, new object[] { startAddress, length, foreColor, backColor });
            }
            else
            {
                Highlight newhl = new Highlight(startAddress, length, foreColor, backColor);

                if (activeHighlights.Keys.Contains(startAddress))
                {
                    pushColor(activeHighlights[startAddress]);
                    hbMemory.RemoveHighlight(startAddress);
                    hbMemory.Invalidate();
                }

                hbMemory.AddHighlight(startAddress, length, foreColor, backColor);
                activeHighlights.Add(startAddress, newhl);

                hbMemory.Refresh();
            }
        }

        private void pushColor(Highlight highlight)
        {
            if (!oldHighlights.Keys.Contains(highlight.Address))
                oldHighlights.Add(highlight.Address, new Stack<Highlight>());

            oldHighlights[highlight.Address].Push(highlight);
            activeHighlights.Remove(highlight.Address);
        }

        private void resetColor(int startAddress)
        {
            if (this.hbMemory.InvokeRequired)
            {
                resetColorCallback c = new resetColorCallback(resetColor);
                this.Invoke(c, new object[] { startAddress });
            }
            else
            {
                hbMemory.RemoveHighlight(startAddress);
                activeHighlights.Remove(startAddress);

                if (oldHighlights.Keys.Contains(startAddress))
                {
                    Highlight oldhl = oldHighlights[startAddress].Pop();
                    if (oldHighlights[startAddress].Count == 0)
                        oldHighlights.Remove(startAddress);
                    hbMemory.AddHighlight(oldhl.Address, oldhl.Length, oldhl.ForeColor, oldhl.BackColor);
                    activeHighlights.Add(startAddress, oldhl);
                }

                hbMemory.Refresh();
            }
        }

        private void enableButtons(SICEvent args)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => enableButtons(args)));
            }
            else
            {
                btnRun.Enabled = true;
                btnLoad.Enabled = true;
                btnLoadEXT.Enabled = true;
                btnStep.Enabled = true;
                openFilesToolStripMenuItem.Enabled = true;
                btnBreak.Enabled = false;
                vmThread = null;
            }
        }

        private void disableButtons()
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => disableButtons()));
            }
            else
            {
                btnRun.Enabled = false;
                btnLoad.Enabled = false;
                btnLoadEXT.Enabled = false;
                btnStep.Enabled = false;
                openFilesToolStripMenuItem.Enabled = false;
                btnBreak.Enabled = true;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string memmsg = "";
            try
            {
                vmThread = new Thread(vm.Run);
                vmThread.Start(Convert.ToInt32(tbRunAddr.Text, 16));
            }
            catch (DeviceNotInitialized)
            {
                MessageBox.Show("Device not initalized properly.");
            }
            catch (DeviceNotReady)
            {
                MessageBox.Show("Device not ready. Remember to test your devices.");
            }
            catch (UnknownDevice)
            {
                MessageBox.Show("Unknown device.");
            }
            catch (Exception ex)
            {
                memmsg = string.Format("Error: {0} - {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
            }
        }

        private void SetRegisters()
        {
            tbRegA.Text = vm.RegisterA.ToString("X6");
            tbRegB.Text = vm.RegisterB.ToString("X6");
            tbRegX.Text = vm.RegisterX.ToString("X6");
            tbRegS.Text = vm.RegisterS.ToString("X6");
            tbRegL.Text = vm.RegisterL.ToString("X6");
            tbRegT.Text = vm.RegisterT.ToString("X6");
            tbPC.Text = vm.ProgramCounter.ToString("X6");
            tbSW.Text = vm.StatusWord.ToString("X6");
        }

        private void btnDelPt_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstBkpt.SelectedIndex >= 0)
                {
                    Breakpoints.Remove(Convert.ToInt32(lstBkpt.Items[lstBkpt.SelectedIndex].ToString(), 16));
                    lstBkpt.Items.RemoveAt(lstBkpt.SelectedIndex);
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            OutputMemdump();
        }

        private void openFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config config = new Config(ref vm.devices);
            config.ShowDialog();
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            if (vm.ProgramCounter == 0 && vm.ProgramCounter != Convert.ToInt32(tbRunAddr.Text, 16))
                vm.ProgramCounter = Convert.ToInt32(tbRunAddr.Text, 16);
            string memmsg = "";
            try
            {
                vm.Step();
            }
            catch (Exception ex)
            {
                memmsg = string.Format("Error: {0} - {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
            }
            tbRunAddr.Text = string.Format("{0:X}", vm.ProgramCounter);
            lstInstructions.Items.Clear();
            foreach (Instruction instruction in trace)
            {
                lstInstructions.Items.Add(instruction);
            }
            lstInstructions.SelectedIndex = lstInstructions.Items.Count - 1;
            SetRegisters();

            StringBuilder builder = new StringBuilder();
            foreach (string msg in messages)
                builder.AppendLine(msg);

            messages.Clear();

            builder.AppendLine(memmsg);
            OutputMemdump(builder.ToString());
        }

        private void allowWritingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = !item.Checked;
            vm.AllowWriting = item.Checked;
        }

        private void clearMemoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will fill memory with 0xFF. Are you sure?", "Clear Memory?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                vm.ClearMemory();
                tbRunAddr.Text = "0";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tbOutput.Text = "";
            lstInstructions.Items.Clear();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void btnLoadEXT_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileStream infile;
                try
                {
                    infile = new FileStream(dialog.FileName, FileMode.Open);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to open file");
                    return;
                }
                StreamReader reader = new StreamReader(infile);
                string infilestr = reader.ReadToEnd();
                reader.Close();

                Tuple<int, int> addresses = vm.LoadXEObjectFile(infilestr);

                tbRunAddr.Text = string.Format("{0:X}", addresses.Item1);

                OutputMemdump();

                tbOutput.Text += System.Environment.NewLine;

                hbMemory.Refresh();
                hbMemory.ScrollByteIntoView(addresses.Item1);
            }
        }

        private void hexinput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!HexChars.Contains(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion

        private void lstInstructions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (prevInstructionIndex >= 0)
            {
                Instruction prev = (Instruction)lstInstructions.Items[prevInstructionIndex];
                if (activeHighlights.Keys.Contains(prev.addrof))
                    resetColor(prev.addrof);
                if (prev.calculatedaddr != null && activeHighlights.Keys.Contains((int)prev.calculatedaddr))
                    resetColor((int)prev.calculatedaddr);
            }

            prevInstructionIndex = lstInstructions.SelectedIndex;
            Instruction current = (Instruction)lstInstructions.Items[lstInstructions.SelectedIndex];
            changeColor(current.addrof, current.length, Color.Black, Color.LightSkyBlue);
            if (current.calculatedaddr != null && !(current.immediate && !current.indirect))
            {
                    changeColor((int)current.calculatedaddr, getLengthOperatedOn(current.opcode), Color.Black, Color.LightGray);
            }
        }

        private int getLengthOperatedOn(OpCode code)
        {
            switch (code) {
                case OpCode.LDCH:
                case OpCode.STCH:
                case OpCode.TD:
                case OpCode.WD:
                case OpCode.RD:
                    return 1;
                case OpCode.J:
                case OpCode.JEQ:
                case OpCode.JLT:
                case OpCode.JGT:
                case OpCode.JSUB:
                    return 1;
                case OpCode.RSUB:
                    return 0; // Short of storing L for every RSUB there's no way to highlight were it was going. I'm considering it though.
                default:
                    return 3;
            }
        }

        private void btnBreak_Click(object sender, EventArgs e)
        {
            breaker = true;
        }

        private void SicDBG_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (vmThread != null)
            {
                vmThread.Abort();
            }
        }
    }

    public class DeviceNotInitialized : Exception
    {
    }
    public class DeviceNotReady : Exception
    {
    }

    public class UnknownDevice : Exception
    {
    }


}
