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

        static List<char> HexChars = new List<char>(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', '\b' });

        public SicDBG()
        {
            InitializeComponent();
            vm = new SICVM();
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
            outstr.AppendFormat("{0,6}   0 1 2 3  4 5 6 7  8 9 A B  C D E F{1}", "Addr", Environment.NewLine);
            for (int i = start; i <= end; i++)
            {
                int beginaddr = i;
                int endaddr = beginaddr + 4;
                outstr.AppendFormat("0x{0:X4}: ", beginaddr);
                for (int k = 0; k < 4; k++)
                {
                    for (int j = beginaddr; j < endaddr; j++)
                    {
                        outstr.AppendFormat("{0:X2}", memory[j]);
                    }
                    outstr.Append(" ");
                    beginaddr = endaddr;
                    endaddr = beginaddr + 4;
                }
                outstr.Append(Environment.NewLine);
                i += 15;
                newlines++;
                if (newlines == 22)
                {
                    topofdump = outstr.Length;
                }
            }

            outstr.Append(Environment.NewLine);
            outstr.Append(Environment.NewLine);
            outstr.Append(Environment.NewLine);
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
                if (newpt < 0 || newpt > 32766)
                {
                    MessageBox.Show("Breakpoint out of range.");
                }
                else
                {
                    vm.Breakpoints.Add(newpt);
                    lstBkpt.Items.Add(string.Format("{0:X}", newpt));
                    tbBkPt.Text = "";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid address.");
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
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

            string errorMsgs = "";
            while (vm.Errors.Count > 0) 
                errorMsgs += vm.Errors.Dequeue() + System.Environment.NewLine;
            if (vm.BreakpointReached)
                errorMsgs += string.Format("Breakpoint reached at 0x{0:X4}.", vm.ProgramCounter);
            tbRunAddr.Text = string.Format("{0:X}", vm.ProgramCounter);
            OutputMemdump(Convert.ToInt32(tbStart.Text, 16), Convert.ToInt32(tbEnd.Text, 16), errorMsgs);
            foreach (Instruction instruction in vm.Stack)
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
                    vm.Breakpoints.Remove(Convert.ToInt32(lstBkpt.Items[lstBkpt.SelectedIndex].ToString(), 16));
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
            vm.Step();
            tbRunAddr.Text = string.Format("{0:X}", vm.ProgramCounter);
            string errorMsgs = "";
            while (vm.Errors.Count > 0)
                errorMsgs += vm.Errors.Dequeue() + System.Environment.NewLine;
            lstInstructions.Items.Clear();
            foreach (Instruction instruction in vm.Stack)
            {
                lstInstructions.Items.Add(instruction);
            }
            lstInstructions.SelectedIndex = lstInstructions.Items.Count - 1;
            SetRegisters();
            OutputMemdump(errorMsgs);
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
            //TODO: Move this to the VM object
            Queue<String> errors = new Queue<string>();
            byte[] memory = Enumerable.Repeat<byte>(0xFF, 32768).ToArray<byte>();
            //TODO: Above variables should be at object level.

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
                string[] lines = infilestr.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int startaddr = 0;
                int currentaddr = 0;
                int proglen = 0;
                Dictionary<string, int> extab = new Dictionary<string, int>();
                /* This is necessary to build the extdef table prior to loading. We must do two passes because
                 * programs may reference labels defined in other places. We can either build this table now,
                 * and do the rest in one pass, OR we could load and build the table and do modifications in
                 * a second pass. I chose this way.
                 */
                foreach (string line in lines)
                {
                    if (line[0] == 'D' || line[0] == 'H')
                    {
                        string[] words = line.Split(' ');
                        if (line[0] == 'H')
                        {
                            startaddr += proglen;
                            string proglenstr = line.Split()[1].Substring(6, 6);
                            proglen = Convert.ToInt32(proglenstr, 16);
                            extab.Add(words[0].Substring(1), startaddr);
                        }
                        if (line[0] == 'D')
                        {
                            extab.Add(words[0].Substring(1), Convert.ToInt32(words[1].Substring(0, 6), 16) + startaddr);
                            for (int k = 1; k < words.Length; k += 2)
                            {
                                if (words[k].Length <= 6)
                                {
                                    break;
                                }
                                extab.Add(words[k].Substring(6), Convert.ToInt32(words[k + 1].Substring(0, 6), 16) + startaddr);
                            }
                        }
                    }
                }
                tbStart.Text = "0";
                tbEnd.Text = string.Format("{0:X}", startaddr + proglen);
                int i;
                for (i = 0; i < lines.Length; i++)
                {
                    if (lines[i] == "")
                        continue;
                    if (lines[i][0] != 'H')
                    {
                        errors.Enqueue(string.Format("ERROR: Expected 'H' record on line {2}, encountered this instead:{0}{1}{0}",
                            Environment.NewLine, lines[i], i));
                        break;
                    }
                    startaddr = extab[lines[i].Split(' ')[0].Substring(1)];
                    i++;
                    while (lines[i][0] == 'D')
                    {
                        i++;
                    }
                    while (lines[i][0] == 'R')
                    {
                        i++;
                    }
                    while (lines[i][0] == 'T')
                    {
                        currentaddr = Convert.ToInt32(lines[i].Substring(1, 6), 16) + startaddr;
                        int len = Convert.ToInt32(lines[i].Substring(7, 2), 16) * 2;
                        for (int j = 9; j < len + 9; j += 2)
                        {
                            memory[currentaddr] = Convert.ToByte(lines[i].Substring(j, 2), 16);
                            currentaddr++;
                        }
                        i++;
                    }
                    while (lines[i][0] == 'M')
                    {
                        int modaddr = Convert.ToInt32(lines[i].Substring(1, 6), 16) + startaddr;
                        int bytes = Convert.ToInt32(lines[i].Substring(7, 2), 16);
                        uint mask = bytes > 0 ? (uint)15 : 0;
                        for (int j = 0; j < bytes; j++)
                        {
                            mask = mask << 4;
                            mask += 15;
                        }
                        char op = lines[i][9];
                        string symbol = lines[i].Substring(10);
                        byte[] membytes = { memory[modaddr + 2], memory[modaddr + 1], memory[modaddr], 0 };
                        uint memval = BitConverter.ToUInt32(membytes, 0);
                        if (((memval & mask) + (extab[symbol] & mask) > mask) && op == '+')
                            errors.Enqueue(string.Format("Overflow in add instruction at address {0}.", modaddr));
                        if (op == '+')
                        {
                            memval += (uint)extab[symbol] & mask;
                        }
                        else
                        {
                            memval -= (uint)extab[symbol] & mask;
                        }
                        byte[] result = BitConverter.GetBytes(memval);
                        memory[modaddr + 2] = result[0];
                        memory[modaddr + 1] = result[1];
                        memory[modaddr] = result[2];
                        i++;
                    }
                }
                if (i != lines.Length)
                    errors.Enqueue("WARNING: File did not end in E record.");
                string errorMsgs = "";
                while (errors.Count > 0)
                    errorMsgs += errors.Dequeue();

                OutputMemdump(errorMsgs);
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
