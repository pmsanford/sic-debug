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

        #region Registers

        int registerA;

        public int RegisterA
        {
            get { return registerA; }
            set { 
                registerA = value;
                tbRegA.Text = registerA.ToString("X6");
            }
        }
        int registerB;

        public int RegisterB
        {
            get { return registerB; }
            set
            {
                registerB = value;
                tbRegB.Text = registerB.ToString("X6");
            }
        }
        int registerX;

        public int RegisterX
        {
            get { return registerX; }
            set
            {
                registerX = value;
                tbRegX.Text = registerX.ToString("X6");
            }
        }
        int registerL;

        public int RegisterL
        {
            get { return registerL; }
            set
            {
                registerL = value;
                tbRegL.Text = registerL.ToString("X6");
            }
        }
        int registerS;

        public int RegisterS
        {
            get { return registerS; }
            set
            {
                registerS = value;
                tbRegS.Text = registerS.ToString("X6");
            }
        }
        int registerT;

        public int RegisterT
        {
            get { return registerT; }
            set
            {
                registerT = value;
                tbRegT.Text = registerT.ToString("X6");
            }
        }
        int programCounter;

        public int ProgramCounter
        {
            get { return programCounter; }
            set
            {
                programCounter = value;
                tbPC.Text = programCounter.ToString("X6");
            }
        }
        int statusWord;

        public int StatusWord
        {
            get { return statusWord; }
            set
            {
                statusWord = value;
                tbSW.Text = statusWord.ToString("X6");
            }
        }

        #endregion

        byte[] memory;
        List<int> breakpoints;
        Device[] devices;
        Queue<Instruction> lastInst;
        Queue<String> errors;
        bool devicewrite;

        static List<char> HexChars = new List<char>(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', '\b' });

        public SicDBG()
        {
            memory = Enumerable.Repeat<byte>(0xFF, 32768).ToArray<byte>();
            breakpoints = new List<int>();
            devices = new Device[7];
            for (int i = 0; i < devices.Length; i++)
            {
                devices[i] = new Device();
            }
            lastInst = new Queue<Instruction>();
            InitializeComponent();
            errors = new Queue<string>();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            int addr;
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
                addr = Convert.ToInt32(lines[0], 16);

                for (int i = 2; i < lines.Length; i++)
                {
                    if (lines[i] == "!")
                    {
                        if (i + 1 < lines.Length)
                        {
                            i++;
                            addr = Convert.ToInt32(lines[i], 16);
                            i++;
                            continue;
                        }
                    }
                    if (lines[i].Length < 2)
                        continue;
                    memory[addr] = Convert.ToByte(lines[i].Substring(0, 2), 16);
                    addr++;
                    if (lines[i].Length > 2)
                    {
                        memory[addr] = Convert.ToByte(lines[i].Substring(2, 2), 16);
                        addr++;
                    }
                    if (lines[i].Length > 4)
                    {
                        memory[addr] = Convert.ToByte(lines[i].Substring(4, 2), 16);
                        addr++;
                    }
                    if (lines[i].Length > 6)
                    {
                        memory[addr] = Convert.ToByte(lines[i].Substring(6, 2), 16);
                        addr++;
                    }
                }

                if (addr - Convert.ToInt32(lines[0], 16) > 0x130)
                {
                    tbOutput.Text += string.Format("Loaded code covers more than 20 lines, showing first 20. You can show more above.{0}", Environment.NewLine);
                    addr = Convert.ToInt32(lines[0], 16) + 0x130;
                }
                tbStart.Text = string.Format("{0:X}", Convert.ToInt32(lines[0], 16));
                tbEnd.Text = string.Format("{0:X}", addr);
                tbRunAddr.Text = string.Format("{0:X}", Convert.ToInt32(lines[0], 16));

                OutputMemdump(Convert.ToInt32(lines[0], 16), addr);

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
            newlines += CountNewlines(ErrorMsgs);
            outstr.AppendFormat("{0}{1}", ErrorMsgs, Environment.NewLine);
            if (!allowWritingToolStripMenuItem.Checked && devicewrite)
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

        public int GetRegisterValue(Register reg)
        {
            switch (reg)
            {
                case Register.A:
                    return RegisterA;
                case Register.B:
                    return RegisterB;
                case Register.L:
                    return RegisterL;
                case Register.S:
                    return RegisterS;
                case Register.T:
                    return RegisterT;
                case Register.X:
                    return RegisterX;
            }
            return -1;
        }

        public Device GetDevice(byte deviceno)
        {
            switch (deviceno)
            {
                case 0xF1:
                    return devices[1];
                case 0xF2:
                    return devices[2];
                case 0xF3:
                    return devices[3];
                case 0x00:
                case 0x04:
                case 0x05:
                case 0x06:
                    return devices[deviceno];
                default:
                    throw new UnknownDevice();
            }
        }


        public void SetRegisterValue(Register reg, int value)
        {
            switch (reg)
            {
                case Register.A:
                    RegisterA = value;
                    break;
                case Register.B:
                    RegisterB = value;
                    break;
                case Register.L:
                    RegisterL = value;
                    break;
                case Register.S:
                    RegisterS = value;
                    break;
                case Register.T:
                    RegisterT = value;
                    break;
                case Register.X:
                    RegisterX = value;
                    break;
            }
        }

        public int GetMemoryValue(int memaddr)
        {
                    byte[] bytes = { memory[memaddr + 2], memory[memaddr + 1], memory[memaddr], 0x0 };
                    return BitConverter.ToInt32(bytes, 0);
        }

        public void IncrementPC(Instruction current)
        {
            if (current.twobyte)
                ProgramCounter += 2;
            else if (current.extended)
                ProgramCounter += 4;
            else
                ProgramCounter += 3;
        }

        public void StoreInt(int address, int value)
        {
            byte[] strvalue = BitConverter.GetBytes(value);
            memory[address + 2] = strvalue[0];
            memory[address + 1] = strvalue[1];
            memory[address] = strvalue[2];
        }

        public void Comp(int lh, int rh)
        {
                        if (lh < rh)
                            StatusWord = -1;
                        if (lh == rh)
                            StatusWord = 0;
                        if (lh > rh)
                            StatusWord = 1;
        }

        public int GetAddress(Instruction current)
        {
            int calcaddr;
            if (current.baserel)
                calcaddr = RegisterB + current.address;
            else if (current.extended)
                calcaddr = current.address;
            else
                calcaddr = ProgramCounter + current.address;

            if (!current.immediate && current.indirect)
            {
                calcaddr = GetMemoryValue(calcaddr);
            }

            if (current.indexed)
            {
                calcaddr += RegisterX;
            }

            return calcaddr;
        }

        public int GetData(Instruction current)
        {
            int address = GetAddress(current);
            int memval;

            if (current.immediate && !current.indirect)
            {
                if (!current.pcrel)
                    memval = current.address;
                else if (current.baserel)
                    memval = RegisterB + current.address;
                else
                    memval = ProgramCounter + current.address;
            }
            else
            {
                memval = GetMemoryValue(address);
            }

            return memval;
        }

        public void DoTwoBye(Instruction current)
        {
            Register r1 = current.r1;
            Register r2 = current.r2;
            switch (current.opcode)
            {
                case OpCode.ADDR:
                    SetRegisterValue(r2, (GetRegisterValue(r2) + GetRegisterValue(r1)));
                    break;
                case OpCode.CLEAR:
                    SetRegisterValue(r1, 0);
                    break;
                case OpCode.COMPR:
                    Comp(GetRegisterValue(r1), GetRegisterValue(r2));
                    break;
                case OpCode.DIVR:
                    SetRegisterValue(r2, (GetRegisterValue(r2) / GetRegisterValue(r1)));
                    break;
                case OpCode.MULR:
                    SetRegisterValue(r2, (GetRegisterValue(r2) * GetRegisterValue(r1)));
                    break;
                case OpCode.RMO:
                    SetRegisterValue(r2, GetRegisterValue(r1));
                    break;
                case OpCode.SUBR:
                    SetRegisterValue(r2, (GetRegisterValue(r2) - GetRegisterValue(r1)));
                    break;
                case OpCode.TIXR:
                    RegisterX++;
                    Comp(RegisterX, GetRegisterValue(r1));
                    break;
                case OpCode.SHIFTL:
                    uint lshift = (uint)GetRegisterValue(r1);
                    for (int i = 0; i < ((int)r2) + 1; i++)
                    {
                        lshift = lshift << 1;
                        if ((lshift & 0x01000000) > 0)
                            lshift = (lshift | 0x01);
                        lshift = lshift & 0x00FFFFFF;
                    }
                    SetRegisterValue(r1, (int)lshift);
                    break;
                case OpCode.SHIFTR:
                    uint rshift = (uint)GetRegisterValue(r1);
                    for (int i = 0; i < ((int)r2) + 1; i++)
                    {
                        bool ones = (rshift & 0x1) > 0;
                        rshift = rshift >> 1;
                        if (ones)
                            rshift = (rshift | 0x800000);
                    }
                    SetRegisterValue(r1, (int)rshift);
                    break;
            }
        }

        public bool Step()
        {
            byte[] instruction = new byte[4];
            try
            {
                byte[] buildinstruction = { memory[ProgramCounter], memory[ProgramCounter + 1], memory[ProgramCounter + 2], memory[ProgramCounter + 3] };
                instruction = buildinstruction;
            }
            catch (IndexOutOfRangeException)
            {
                tbOutput.Text += string.Format("Error: Program Counter value 0x{0:X3} outside memory range.", ProgramCounter);
                tbRunAddr.Text = string.Format("{0:X}", ProgramCounter);
                return false;
            }

            Instruction current = new Instruction(instruction);
            current.addrof = ProgramCounter;
            IncrementPC(current);
            if (current.twobyte)
            {
                DoTwoBye(current);
                    if (lastInst.Count >= 15 && !cbFT.Checked)
                    {
                        lastInst.Dequeue();
                    }
                    lastInst.Enqueue(current);
                return true;
            }

            int calcaddr, memval;

            calcaddr = GetAddress(current);
            try
            {
                memval = GetData(current);
            }
            catch (IndexOutOfRangeException)
            {
                errors.Enqueue(string.Format("Error: Instruction at 0x{0:X3} references memory that's out of range (0x{1:X3}).", ProgramCounter, calcaddr));
                //tbOutput.Text += string.Format("Error: Instruction at 0x{0:X3} references memory that's out of range (0x{1:X3}).", ProgramCounter, calcaddr);
                tbRunAddr.Text = string.Format("{0:X}", ProgramCounter);
                return false;
            }

            try
            {
                switch ((OpCode)current.opcode)
                {
                    case OpCode.ADD:
                        RegisterA += memval;
                        break;
                    case OpCode.SUB:
                        RegisterA -= memval;
                        break;
                    case OpCode.MUL:
                        RegisterA *= memval;
                        break;
                    case OpCode.DIV:
                        RegisterA /= memval;
                        break;
                    case OpCode.COMP:
                        Comp(RegisterA, memval);
                        break;
                    case OpCode.J:
                        ProgramCounter = calcaddr;
                        break;
                    case OpCode.JLT:
                        if (StatusWord == -1)
                            ProgramCounter = calcaddr;
                        break;
                    case OpCode.JGT:
                        if (StatusWord == 1)
                            ProgramCounter = calcaddr;
                        break;
                    case OpCode.JEQ:
                        if (StatusWord == 0)
                            ProgramCounter = calcaddr;
                        break;
                    case OpCode.STA:
                        StoreInt(calcaddr, RegisterA);
                        break;
                    case OpCode.STB:
                        StoreInt(calcaddr, RegisterB);
                        break;
                    case OpCode.STL:
                        StoreInt(calcaddr, RegisterL);
                        break;
                    case OpCode.STS:
                        StoreInt(calcaddr, RegisterS);
                        break;
                    case OpCode.STT:
                        StoreInt(calcaddr, RegisterT);
                        break;
                    case OpCode.STX:
                        StoreInt(calcaddr, RegisterX);
                        break;
                    case OpCode.LDB:
                        RegisterB = memval;
                        break;
                    case OpCode.LDX:
                        RegisterX = memval;
                        break;
                    case OpCode.LDA:
                        RegisterA = memval;
                        break;
                    case OpCode.LDL:
                        RegisterL = memval;
                        break;
                    case OpCode.LDS:
                        RegisterS = memval;
                        break;
                    case OpCode.LDT:
                        RegisterT = memval;
                        break;
                    case OpCode.TIX:
                        RegisterX++;
                        Comp(RegisterX, memval);
                        break;
                    case OpCode.JSUB:
                        RegisterL = ProgramCounter;
                        ProgramCounter = calcaddr;
                        break;
                    case OpCode.RSUB:
                        ProgramCounter = RegisterL;
                        break;
                    case OpCode.LDCH:
                        byte[] regA = BitConverter.GetBytes(RegisterA);
                            byte[] newVal = { memory[calcaddr], regA[1], regA[2], regA[3] };
                            RegisterA = BitConverter.ToInt32(newVal, 0);
                        break;
                    case OpCode.STCH:
                        byte[] regiA = BitConverter.GetBytes(RegisterA);
                            memory[calcaddr] = regiA[0];
                        break;
                    case OpCode.RD:
                    case OpCode.TD:
                    case OpCode.WD:
                        devicewrite = true;
                        if (!DoDevice(current.opcode, memval))
                        {
                            ProgramCounter -= 3;
                            return false;
                        }
                        break;
                    case OpCode.AND:
                        RegisterA = RegisterA & memval;
                        break;
                    case OpCode.OR:
                        RegisterA = RegisterA | memval;
                        break;
                    default:
                        tbOutput.Text += System.Environment.NewLine;
                        int location = ProgramCounter;
                        if (current.extended)
                            location -= 4;
                        else
                            location -= 3;
                        tbOutput.Text += string.Format("Unrecognized opcode encountered at {0:X3}", location);
                        tbOutput.Text += System.Environment.NewLine;
                        return false;
                }
            }
            catch (IndexOutOfRangeException)
            {
                errors.Enqueue(string.Format("Error: Instruction at 0x{0:X3} references memory that's out of range (0x{1:X3}).", ProgramCounter - 3, calcaddr));
                //tbOutput.Text += string.Format("Error: Instruction at 0x{0:X3} references memory that's out of range (0x{1:X3}).", ProgramCounter-3, calcaddr);
                tbRunAddr.Text = string.Format("{0:X}", ProgramCounter);
                return false;
            }

                if (lastInst.Count >= 15)
                {
                    lastInst.Dequeue();
                }
                lastInst.Enqueue(current);
            return true;
        }

        public bool DoDevice(OpCode op, int memval)
        {
            byte[] regA = BitConverter.GetBytes(RegisterA);
            byte deviceno = (byte)((memval & 0x00FF0000) >> 16);
            try
            {
                switch (op)
                {
                    case OpCode.RD:
                        byte[] rdVal = { GetDevice(deviceno).Read(), regA[1], regA[2], regA[3] };
                        RegisterA = BitConverter.ToInt32(rdVal, 0);
                        return true;
                    case OpCode.WD:
                        if (allowWritingToolStripMenuItem.Checked)
                            GetDevice(deviceno).Write(regA[0]);
                        return true;
                    case OpCode.TD:
                        if (GetDevice(deviceno).TestDevice())
                            StatusWord = -1;
                        else
                            StatusWord = 0;
                        return true;
                    default:
                        return false;
                }
            }
            catch (DeviceNotInitialized)
            {
                errors.Enqueue(string.Format("Device {0:X} not initialized. Use the menu option to open files.{1}",
                    deviceno, Environment.NewLine));
                //tbOutput.Text += string.Format("Device {0:X} not initialized. Use the menu option to open files.{1}",
                //    deviceno, Environment.NewLine);
                tbRunAddr.Text = string.Format("{0:X}", ProgramCounter);
                return false;
            }
            catch (DeviceNotReady)
            {
                errors.Enqueue(string.Format("Device {0:X} is not ready. Remember to test devices before addressing them.{1}",
                    deviceno, Environment.NewLine));
                //tbOutput.Text += string.Format("Device {0:X} is not ready. Remember to test devices before addressing them.{1}",
                //    deviceno, Environment.NewLine);
                tbRunAddr.Text = string.Format("{0:X}", ProgramCounter);
                return false;
            }
            catch (UnknownDevice)
            {
                errors.Enqueue(string.Format("{0:X} does not refer to a known device. Valid options:{1}F2, F3, 04, 05, 06{1}",
                    deviceno, Environment.NewLine));
                //tbOutput.Text += string.Format("{0:X} does not refer to a known device. Valid options:{1}F2, F3, 04, 05, 06{1}",
                //    deviceno, Environment.NewLine);
                tbRunAddr.Text = string.Format("{0:X}", ProgramCounter);
                return false;
            }
        }

        public bool Run(int startingaddr)
        {
            ProgramCounter = startingaddr;
            int currentaddr = startingaddr;
            bool shouldbreak = false;
            int counter = 0;
            if (ProgramCounter > 32768 || ProgramCounter < 0)
            {
                errors.Enqueue(string.Format("Error: Program Counter value 0x{0:X3} outside memory range.", ProgramCounter));
                //tbOutput.Text += string.Format("Error: Program Counter value 0x{0:X3} outside memory range.", ProgramCounter);
                tbRunAddr.Text = string.Format("{0:X}", ProgramCounter);
                return false;
            }
            while (memory[ProgramCounter] != 0xFF)
            {
                counter++;
                if (counter >= 50000)
                    if (MessageBox.Show("Probable infinite loop (50000 iterations passed). Break out?",
                        "Infinite Loop Detected", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        return false;
                        
                
                if (breakpoints.Contains(ProgramCounter) && shouldbreak)
                {
                    tbRunAddr.Text = string.Format("{0:X}", ProgramCounter);
                    tbOutput.Text += string.Format("Hit breakpoint at 0x{0:X}.{1}", ProgramCounter, System.Environment.NewLine);
                    return true;
                }

                if (!Step())
                {
                    errors.Enqueue(string.Format("Fatal error at location {0:X}{1}", ProgramCounter, Environment.NewLine));
                    //tbOutput.Text += string.Format("Fatal error at location {0:X}{1}", ProgramCounter, Environment.NewLine);
                    tbRunAddr.Text = string.Format("{0:X}", ProgramCounter);
                    return false;
                }
                shouldbreak = true;
            }
                foreach (Instruction instruction in lastInst)
                {
                    lstInstructions.Items.Add(instruction);
                }
            lstInstructions.SelectedIndex = lstInstructions.Items.Count - 1;
            return true;
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
                    breakpoints.Add(newpt);
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
            devicewrite = false;
            Run(Convert.ToInt32(tbRunAddr.Text, 16));

            string errorMsgs = "";
            while (errors.Count > 0) 
                errorMsgs += errors.Dequeue() + System.Environment.NewLine;
            OutputMemdump(Convert.ToInt32(tbStart.Text, 16), Convert.ToInt32(tbEnd.Text, 16), errorMsgs);
        }

        private void btnDelPt_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstBkpt.SelectedIndex >= 0)
                {
                    breakpoints.Remove(Convert.ToInt32(lstBkpt.Items[lstBkpt.SelectedIndex].ToString(), 16));
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
            Config config = new Config(ref devices);
            config.ShowDialog();
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            devicewrite = false;
            Step();
            tbRunAddr.Text = string.Format("{0:X}", ProgramCounter);
            string errorMsgs = "";
            while (errors.Count > 0) 
                errorMsgs += errors.Dequeue() + System.Environment.NewLine;
            OutputMemdump(errorMsgs);
        }

        private void allowWritingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = !item.Checked;
        }

        private void clearMemoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will fill memory with 0xFF. Are you sure?", "Clear Memory?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                memory = Enumerable.Repeat<byte>(0xFF, 32768).ToArray<byte>();
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
                            //string startaddrstr = line.Split()[1].Substring(0, 6);
                            string proglenstr = line.Split()[1].Substring(6, 6);
                            //startaddr = Convert.ToInt32(startaddrstr, 16);
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
                            ;// Overflow error
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
