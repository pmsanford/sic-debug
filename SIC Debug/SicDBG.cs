using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;


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

        public SicDBG()
        {
            InitializeComponent();
            vm = new SICVM();
            vm.PreInstructionHook += new SICVM.PreInstruction(breakptHandler);
            vm.PostInstructionHook += new SICVM.PostInstruction(traceHandler);
            vm.PreInstructionHook += new SICVM.PreInstruction(stopOnFF);
            hbMemory.ByteProvider = new MemoryByteProvider(vm.Memory);
            hbMemory.SelectionForeColor = Color.White;
            hbMemory.SelectionBackColor = Color.Blue;
            vm.MemoryChangedHook += new SICVM.MemoryChanged(memoryChange);
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

                int endaddr = addresses.Item2;
                if (addresses.Item2 - addresses.Item1 > 0x130)
                {
                    tbOutput.Text += string.Format("Loaded code covers more than 20 lines, showing first 20. You can show more above.{0}", Environment.NewLine);
                    endaddr = addresses.Item1 + 0x130;
                }
                tbStart.Text = string.Format("{0:X}", addresses.Item1);
                tbEnd.Text = string.Format("{0:X}", endaddr);
                tbRunAddr.Text = string.Format("{0:X}", addresses.Item1);

                OutputMemdump(addresses.Item1, endaddr);

                tbOutput.Text += System.Environment.NewLine;

                hbMemory.Refresh();
                hbMemory.ScrollByteIntoView(addresses.Item1);
            }
        }

        public void OutputMemdump()
        {
            OutputMemdump(Convert.ToInt32(tbStart.Text, 16), Convert.ToInt32(tbEnd.Text, 16), "");
        }

        public void OutputMemdump(int start, int end)
        {
            OutputMemdump(start, end, "");
        }

        public void OutputMemdump(string ErrorMsgs)
        {
            OutputMemdump(Convert.ToInt32(tbStart.Text, 16), Convert.ToInt32(tbEnd.Text, 16), ErrorMsgs);
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

        public void OutputMemdump(int start, int end, string ErrorMsgs)
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

        private void breakptHandler(SICEvent e)
        {
            if (Breakpoints.Contains(e.PC) && (lastBP != e.PC))
            {
                e.Continue = false;
                messages.Enqueue(string.Format("Breakpoint reached at 0x{0:X4}.", vm.ProgramCounter));
                tbRunAddr.Text = string.Format("{0:X}", vm.ProgramCounter);
                hbMemory.Refresh();
                hbMemory.ScrollByteIntoView(vm.ProgramCounter);
                lastBP = e.PC;
            }
            else if (lastBP != null)
                lastBP = null;
        }

        private void traceHandler(SICEvent e)
        {
            trace.Enqueue(e.instruction);
        }

        private void stopOnFF(SICEvent e)
        {
            if (vm.Memory[e.PC] == 0xFF)
                e.Continue = false;
        }

        private void memoryChange(int address, int length)
        {
            hbMemory.Refresh();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string memmsg = "";
            try
            {
                bool success = vm.Run(Convert.ToInt32(tbRunAddr.Text, 16));
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
            StringBuilder builder = new StringBuilder();
            foreach (string msg in messages)
                builder.AppendLine(msg);

            messages.Clear();

            builder.AppendLine(memmsg);

            OutputMemdump(Convert.ToInt32(tbStart.Text, 16), Convert.ToInt32(tbEnd.Text, 16), builder.ToString());

            foreach (Instruction instruction in trace)
            {
                lstInstructions.Items.Add(instruction);
            }
            lstInstructions.SelectedIndex = lstInstructions.Items.Count - 1;
            SetRegisters();
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
                tbStart.Text = "0";
                tbEnd.Text = "0";
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

                int endaddr = addresses.Item2;
                if (addresses.Item2 - addresses.Item1 > 0x130)
                {
                    tbOutput.Text += string.Format("Loaded code covers more than 20 lines, showing first 20. You can show more above.{0}", Environment.NewLine);
                    endaddr = addresses.Item1 + 0x130;
                }
                tbStart.Text = string.Format("{0:X}", addresses.Item1);
                tbEnd.Text = string.Format("{0:X}", endaddr);
                tbRunAddr.Text = string.Format("{0:X}", addresses.Item1);

                OutputMemdump(addresses.Item1, endaddr);

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
