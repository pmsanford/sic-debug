﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SIC_Debug
{
    class SICVM
    {
        public enum TracingLevel {
            None,
            Partial,
            Full
        }

        //TODO: "Each register is 24 bits in length" Beck p5
        public int RegisterA
        {
            get
            {
                return registerA;
            }
            set
            {
                if ((value & 0x800000) > 1)
                {
                    registerA = unchecked((int)((value & 0xFFFFFF) + 0xFF000000));
                }
                else
                    registerA = value;
            }
        }
        public int RegisterB
        {
            get
            {
                return registerB;
            }
            set
            {
                if ((value & 0x800000) > 1)
                {
                    registerB = unchecked((int)((value & 0xFFFFFF) + 0xFF000000));
                }
                else
                    registerB = value;
            }
        }
        public int RegisterX
        {
            get
            {
                return registerX;
            }
            set
            {
                if ((value & 0x800000) > 1)
                {
                    registerX = unchecked((int)((value & 0xFFFFFF) + 0xFF000000));
                }
                else
                    registerX = value;
            }
        }
        public int RegisterS
        {
            get
            {
                return registerS;
            }
            set
            {
                if ((value & 0x800000) > 1)
                {
                    registerS = unchecked((int)((value & 0xFFFFFF) + 0xFF000000));
                }
                else
                    registerS = value;
            }
        }
        public int RegisterL
        {
            get
            {
                return registerL;
            }
            set
            {
                if ((value & 0x800000) > 1)
                {
                    registerL = unchecked((int)((value & 0xFFFFFF) + 0xFF000000));
                }
                else
                    registerL = value;
            }
        }
        public int RegisterT
        {
            get
            {
                return registerT;
            }
            set
            {
                if ((value & 0x800000) > 1)
                {
                    registerT = unchecked((int)((value & 0xFFFFFF) + 0xFF000000));
                }
                else
                    registerT = value;
            }
        }
        public int ProgramCounter { get; set; }
        public int StatusWord { get; set; }
        public bool AllowWriting { get; set; }
        public IDevice[] Devices { get { return devices; } }
        public byte[] Memory { get { return memory; } }
        public bool DeviceWritten { get { return devicewrite; } }
        public bool Running = false;

        int registerA, registerB, registerX, registerS, registerL, registerT;

        private byte[] memory;
        public IDevice[] devices; //TODO: This should be private.
        private bool devicewrite;
        public Instruction lastInstruction = null;
        private Instruction current = null;

        private static List<char> HexChars = new List<char>(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', '\b' });

        public delegate void PreInstruction(SICEvent args);
        public delegate void PostInstruction(SICEvent args);
        public delegate void WarningHandler(SICWarning args);
        public delegate void MemoryChanged(int address, int length);
        public delegate void RunningStarted();
        public delegate void RunningFinished(SICEvent args);

        public event PreInstruction PreInstructionHook;
        public event PostInstruction PostInstructionHook;
        public event WarningHandler WarningHook;
        public event MemoryChanged MemoryChangedHook;
        public event RunningStarted RunningStartedHook;
        public event RunningFinished RunningFinishedHook;

        public SICVM()
        {
            RegisterA = RegisterB = RegisterX = RegisterS = RegisterL = RegisterT = ProgramCounter = StatusWord = 0;
            AllowWriting = false;
            memory = Enumerable.Repeat<byte>(0xFF, 1048575).ToArray<byte>(); // 1048575 bytes, 1Mb, is the memory range for XE machines.
            devices = new IDevice[7];
            for (int i = 0; i < devices.Length; i++)
            {
                devices[i] = null;
            }
            PreInstructionHook = null;
            PostInstructionHook = null;
            WarningHook = null;
            MemoryChangedHook = null;
        }

        public Tuple<int, int> LoadXEObjectFile(string filecontents)
        {
            // TODO: Handle errors correctly.
            string[] lines = filecontents.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
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
                        string proglenstr = line.Substring(13, 6);
                        proglen = Convert.ToInt32(proglenstr, 16);
                        extab.Add(line.Substring(1, 6).Trim(), startaddr);
                    }
                    if (line[0] == 'D')
                    {
                        extab.Add(line.Substring(1, 6).TrimEnd(), Convert.ToInt32(line.Substring(7, 6), 16) + startaddr);
                        for (int k = 13; k < line.Length; k += 12)
                        {
                            if (line.Substring(k).Length <= 12)
                            {
                                break;
                            }
                            extab.Add(line.Substring(k, 6), Convert.ToInt32(line.Substring(k + 6, 6), 16) + startaddr);
                        }
                    }
                }
            }

            Tuple<int, int> rettup = new Tuple<int, int>(0, startaddr + proglen);
            int i;
            for (i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "")
                    continue;
                if (lines[i][0] != 'H')
                {
                    throw new FormatException(string.Format("ERROR: Expected 'H' record on line {2}, encountered this instead:{0}{1}{0}",
                        Environment.NewLine, lines[i], i));
                }
                startaddr = extab[lines[i].Substring(1, 6).Trim()];
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
                    for (int j = 1; j < bytes; j++)
                    {
                        mask = mask << 4;
                        mask += 15;
                    }
                    char op = lines[i][9];
                    string symbol = lines[i].Substring(10);

                    int fbytes = bytes % 2 == 0 ? bytes : bytes + 1;
                    fbytes /= 2;

                    List<byte> bytelist = new List<byte>();
                    for (int j = fbytes - 1; j >= 0; j--)
                    {
                        bytelist.Add(memory[modaddr + j]);
                    }
                    while (bytelist.Count < 4)
                        bytelist.Add(0);

                    uint memval = BitConverter.ToUInt32(bytelist.ToArray(), 0);
                    if (((memval & mask) + (extab[symbol] & mask) > mask) && op == '+')
                        throw new OverflowException(string.Format("Overflow in add instruction at address {0}.", modaddr));
                    if (op == '+')
                    {
                        memval += (uint)extab[symbol] & mask;
                    }
                    else
                    {
                        memval -= (uint)extab[symbol] & mask;
                    }


                    List<byte> result = BitConverter.GetBytes(memval).ToList();
                    result.Reverse();
                    result = result.Skip(result.Count - fbytes).ToList();

                    for (int j = 0; j < fbytes; j++)
                    {
                        memory[modaddr + j] = result[j];
                    }

                    i++;
                }
            }
            if (i != lines.Length)
            {
                SICWarning warning = new SICWarning("No E record found at end of file.", "XE Object Loader");
                WarningHook(warning);
                if (warning.Abort)
                    throw new Exception(string.Format("Loading aborted. {0}", warning.Message));
            }

            return rettup;
        }

        public Tuple<int, int> LoadObjectFile(string filecontents)
        {
            int addr = 0;
            string[] lines = filecontents.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            addr = Convert.ToInt32(lines[0], 16);
            int startaddr = addr;

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
            return new Tuple<int, int>(startaddr, addr);   
        }

        public bool ClearMemory()
        {
            memory = Enumerable.Repeat<byte>(0xFF, 32768).ToArray<byte>();
            ProgramCounter = 0;
            return true;
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

        public IDevice GetDevice(byte deviceno)
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
            if (MemoryChangedHook != null)
                MemoryChangedHook(address, 3);
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
                { // Immediate mode combined with an addressing mode gets you the address as data. See Beck, p61
                    memval = current.address;
                    if (current.extended)
                    {
                        if ((memval & 0x80000) > 0)
                            memval = unchecked((int)((memval & 0xFFFFF) + 0xFFF00000));
                    }
                    else
                    {
                        if ((memval & 0x800) > 0)
                            memval = unchecked((int)((memval & 0xFFF) + 0xFFFFF000));
                    }
                }
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

        private Instruction getInstruction()
        {
            byte[] instruction = new byte[4];
            try
            {
                byte[] buildinstruction = { memory[ProgramCounter], memory[ProgramCounter + 1], memory[ProgramCounter + 2], memory[ProgramCounter + 3] };
                instruction = buildinstruction;
            }
            catch (IndexOutOfRangeException)
            {
                //TODO: handle instruction out of memory range.
                return null;
            }

            return new Instruction(instruction);
        }

        public bool Step()
        {
            this.devicewrite = false;
            current = getInstruction();
            SICEvent instEvent = new SICEvent(current, ProgramCounter);
            PreInstructionHook(instEvent);
            if (!instEvent.Continue)
                return false;
            current.addrof = ProgramCounter;
            IncrementPC(current);
            if (current.twobyte)
            {
                DoTwoBye(current);

                lastInstruction = current;


                instEvent = new SICEvent(lastInstruction, ProgramCounter);
                PostInstructionHook(instEvent);
                if (!instEvent.Continue)
                    return false;

                return true;
            }

            int calcaddr, memval;

            calcaddr = GetAddress(current);
            current.calculatedaddr = calcaddr;
            try
            {
                memval = GetData(current);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new IndexOutOfRangeException(string.Format("Error: Instruction at 0x{0:X3} references memory that's out of range (0x{1:X3}).", current.addrof, calcaddr), ex);
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
                        byte[] memChar = BitConverter.GetBytes(memval);
                        byte[] newVal = { memChar[2], regA[1], regA[2], regA[3] };
                        RegisterA = BitConverter.ToInt32(newVal, 0);
                        break;
                    case OpCode.STCH:
                        byte[] regiA = BitConverter.GetBytes(RegisterA);
                        memory[calcaddr] = regiA[0];
                        MemoryChangedHook(calcaddr, 1);
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
                        int location = ProgramCounter;
                        if (current.extended)
                            location -= 4;
                        else
                            location -= 3;
                        throw new ArgumentException(string.Format("Error: Instruction at 0x{0} not a recognized opcode.", !current.extended ? location.ToString("X3") : location.ToString("X4")));
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new IndexOutOfRangeException(string.Format("Error: Instruction at 0x{0:X3} references memory that's out of range (0x{1:X3}).", ProgramCounter - 3, calcaddr), ex);
            }

            lastInstruction = current;


            instEvent = new SICEvent(lastInstruction, ProgramCounter);
            PostInstructionHook(instEvent);
            if (!instEvent.Continue)
                return false;
            return true;
        }

        public bool DoDevice(OpCode op, int memval)
        {
            byte[] regA = BitConverter.GetBytes(RegisterA);
            byte deviceno = (byte)((memval & 0x00FF0000) >> 16); 
            
            switch (op)
            {
                case OpCode.RD:
                    byte[] rdVal = { GetDevice(deviceno).Read(), regA[1], regA[2], regA[3] };
                    RegisterA = BitConverter.ToInt32(rdVal, 0);
                    return true;
                case OpCode.WD:
                    if (AllowWriting)
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

        public void Run(object startingaddr)
        {
            this.Run((int)startingaddr);
        }

        public bool Run(int startingaddr)
        {
            Running = true;
            RunningStartedHook();
            this.devicewrite = false;
            ProgramCounter = startingaddr;
            int currentaddr = startingaddr;
            if (ProgramCounter > 32768 || ProgramCounter < 0)
                throw new IndexOutOfRangeException(string.Format("Error: Program Counter value 0x{0:X3} outside memory range.", ProgramCounter));

            while (true)
            {
                try
                {
                    if (!Step())
                    {
                        RunningFinishedHook(new SICEvent(current, this.ProgramCounter));
                        Running = false;
                        return true;
                    }
                }
                catch (ThreadAbortException)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    RunningFinishedHook(new SICEvent(current, this.ProgramCounter, ex, string.Format("Error encountered at location {0:X6}: {1}", this.ProgramCounter, ex.Message)));
                    Running = false;
                    return false;
                }
            }
        }
    }
}
