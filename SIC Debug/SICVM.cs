﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public int RegisterA { get; set; }
        public int RegisterB { get; set; }
        public int RegisterX { get; set; }
        public int RegisterS { get; set; }
        public int RegisterL { get; set; }
        public int RegisterT { get; set; }
        public int ProgramCounter { get; set; }
        public int StatusWord { get; set; }
        public bool AllowWriting { get; set; }
        public TracingLevel Trace { get; set; }
        public Queue<String> Errors { get { return errors; } }
        public Queue<Instruction> Stack { get { return lastInst; } }
        public List<int> Breakpoints { get { return breakpoints; } }
        public Device[] Devices { get { return devices; } }
        public bool BreakpointReached { get; set; }
        public byte[] Memory { get { return memory; } }
        public bool DeviceWritten { get { return devicewrite; } }

        private byte[] memory;
        private List<int> breakpoints;
        public Device[] devices; //TODO: This should be private.
        private Queue<Instruction> lastInst;
        private Queue<String> errors;
        private bool devicewrite;

        private static List<char> HexChars = new List<char>(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F', '\b' });

        public SICVM()
        {
            Trace = TracingLevel.Partial;
            RegisterA = RegisterB = RegisterX = RegisterS = RegisterL = RegisterT = ProgramCounter = StatusWord = 0;
            AllowWriting = BreakpointReached = false;
            memory = Enumerable.Repeat<byte>(0xFF, 32768).ToArray<byte>(); // 32768 bytes, 32Kb, Beck p5
            errors = new Queue<string>();
            lastInst = new Queue<Instruction>();
            devices = new Device[7];
            for (int i = 0; i < devices.Length; i++)
            {
                devices[i] = new Device();
            }
            breakpoints = new List<int>();
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
                if (!current.pcrel) // Immediate mode combined with an addressing mode gets you the address as data. See Beck, p61
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
            this.devicewrite = false;
            byte[] instruction = new byte[4];
            try
            {
                byte[] buildinstruction = { memory[ProgramCounter], memory[ProgramCounter + 1], memory[ProgramCounter + 2], memory[ProgramCounter + 3] };
                instruction = buildinstruction;
            }
            catch (IndexOutOfRangeException)
            {
                //TODO: handle instruction out of memory range.
                return false;
            }

            Instruction current = new Instruction(instruction);
            current.addrof = ProgramCounter;
            IncrementPC(current);
            if (current.twobyte)
            {
                DoTwoBye(current);
                if (lastInst.Count >= 15 && Trace == TracingLevel.Partial)
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
                        int location = ProgramCounter;
                        if (current.extended)
                            location -= 4;
                        else
                            location -= 3;
                        //TODO: Handle unrecognized opcode.
                        return false;
                }
            }
            catch (IndexOutOfRangeException)
            {
                errors.Enqueue(string.Format("Error: Instruction at 0x{0:X3} references memory that's out of range (0x{1:X3}).", ProgramCounter - 3, calcaddr));
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
            byte deviceno = (byte)((memval & 0x00FF0000) >> 16); switch (op)
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

        public bool Run(int startingaddr)
        {
            this.devicewrite = false;
            ProgramCounter = startingaddr;
            int currentaddr = startingaddr;
            bool shouldbreak = false;
            int counter = 0;
            if (ProgramCounter > 32768 || ProgramCounter < 0)
            {
                errors.Enqueue(string.Format("Error: Program Counter value 0x{0:X3} outside memory range.", ProgramCounter));
                return false;
            }
            BreakpointReached = false;
            while (memory[ProgramCounter] != 0xFF)
            {
                counter++;
                if (counter >= 50000)
                {
                    errors.Enqueue("Infinite loop encountered.");
                    return false;
                }


                if (breakpoints.Contains(ProgramCounter) && shouldbreak)
                {
                    BreakpointReached = true;
                    return true;
                }

                if (!Step())
                {
                    errors.Enqueue(string.Format("Fatal error at location {0:X}{1}", ProgramCounter, Environment.NewLine));
                    return false;
                }
                shouldbreak = true;
            }
            return true;
        }
    }
}
