﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIC_Debug
{
    public class Instruction
    {
        // 3/4 byte instruction layout:
        // Top 6 bits: opcode
        // bottom two bits of 1st byte: indirect immediate
        // if these two are unset, we're in simple addressing mode, and only the index bit below matters
        // from top to bottom of first 4 bits of the next byte: index, base-relative, pc-relative, extended
        // following 12 bits (3 byte instruction) or 20 bits (4 byte instruction): address or argument
        public byte[] instruction;
        public OpCode opcode;
        public bool indirect;
        public bool immediate;
        public bool indexed;
        public bool baserel;
        public bool pcrel;
        public bool extended;
        public int address;
        public bool twobyte;
        public Register r1;
        public Register r2;
        public int addrof;
        public int? calculatedaddr = null;
        public int length;

        public Instruction(string instruction)
        {
            if (instruction.Length < 4 || instruction.Length > 8 || instruction.Length % 2 != 0)
                throw new InvalidInstructionException();

            byte[] instbytes = new byte[instruction.Length / 2];

            instbytes[0] = Convert.ToByte(instruction.Substring(0, 2), 16);
            if (instruction.Length > 2)
                instbytes[1] = Convert.ToByte(instruction.Substring(2, 2), 16);
            if (instruction.Length > 4)
                instbytes[2] = Convert.ToByte(instruction.Substring(4, 2), 16);
            if (instruction.Length > 6)
                instbytes[3] = Convert.ToByte(instruction.Substring(6, 2), 16);

            buildInstruction(instbytes);
        }

        public Instruction(byte[] inputbytes)
        {
            buildInstruction(inputbytes);
        }

        private void buildInstruction(byte[] inputbytes) {
            instruction = inputbytes;
            opcode = (OpCode)(inputbytes[0] & 0xFC);
            if (isTwoByte(opcode))
            {
                twobyte = true;
                r1 = (Register)((inputbytes[1] & 0xF0) / 16);
                r2 = (Register)(inputbytes[1] & 0x0F);
                length = 2;
            }
            else
            {
                twobyte = false;
                indirect = (inputbytes[0] & 2) > 0 ? true : false;
                immediate = (inputbytes[0] & 1) > 0 ? true : false;
                indexed = (inputbytes[1] & 0x80) > 0 ? true : false;
                baserel = (inputbytes[1] & 0x40) > 0 ? true : false;
                pcrel = (inputbytes[1] & 0x20) > 0 ? true : false;
                extended = (inputbytes[1] & 0x10) > 0 ? true : false;
                if (extended)
                    length = 4;
                else
                    length = 3;
                if (!indirect && !immediate) // Simple SIC instruction
                {
                    baserel = false;
                    pcrel = false;
                    extended = false;
                }
                if (extended)
                {
                    byte top = (byte)(inputbytes[1] & 0x0F);
                    byte[] addr = { inputbytes[3], inputbytes[2], top, 0x0 };
                    address = BitConverter.ToInt32(addr, 0);
                }
                else if ((inputbytes[1] & 0x08) > 0 && pcrel) // A 1 in the top bit of the address means we're doing a negative PC Relative calculation
                {
                    if (inputbytes[2] == 0x00)
                    {
                        inputbytes[1] = (byte)(inputbytes[1] - 1);
                    }
                    inputbytes[1] = (byte)~inputbytes[1];
                    inputbytes[2] = (byte)(inputbytes[2] - 1);
                    inputbytes[2] = (byte)~inputbytes[2];
                    address = (inputbytes[1] & 0x0F) * 256 + inputbytes[2];
                    address *= -1;
                }
                else if (!immediate && !indirect) // Simple SIC (direct) addressing mode
                {
                    address = (inputbytes[1] * 256) + inputbytes[2];
                }
                else // Standard addressing mode
                {
                    address = (inputbytes[1] & 0x0F) * 256 + inputbytes[2];
                }
            }
        }

        public override string ToString()
        {
            if (twobyte)
                if (opcode == OpCode.CLEAR)
                    return string.Format("0x{0:X3}: {1,-6} {2}", addrof, opcode.ToString(), r1.ToString());
                else if (opcode == OpCode.SHIFTL || opcode == OpCode.SHIFTR)
                    return string.Format("0x{0:X3}: {1,-6} {2},{3}", addrof, opcode.ToString(), r1.ToString(), ((int)r2 + 1).ToString());
                else
                    return string.Format("0x{0:X3}: {1,-6} {2},{3}", addrof, opcode.ToString(), r1.ToString(), r2.ToString());
            if (extended)
            {
                return string.Format("0x{0:X3}: {1,-6} {2}{3} {4}{5}{6}{7} 0x{8:X4}", addrof, opcode.ToString(), indirect ? "1" : "0", immediate ? "1" : "0",
                    indexed ? "1" : "0", baserel ? "1" : "0", pcrel ? "1" : "0", extended ? "1" : "0", address);
            }
            else
            {
                return string.Format("0x{0:X3}: {1,-6} {2}{3} {4}{5}{6}{7} 0x{8:X3}", addrof, opcode.ToString(), indirect ? "1" : "0", immediate ? "1" : "0",
                    indexed ? "1" : "0", baserel ? "1" : "0", pcrel ? "1" : "0", extended ? "1" : "0", address);
            }
        }

        public bool isTwoByte(OpCode code)
        {
            OpCode[] twobytes = { OpCode.ADDR, OpCode.CLEAR, OpCode.COMPR, OpCode.DIVR,
                                    OpCode.MULR, OpCode.RMO, OpCode.SUBR, OpCode.TIXR,
                                    OpCode.SHIFTL, OpCode.SHIFTR };
            if (twobytes.Contains<OpCode>(code))
                return true;
            else
                return false;
        }

    }

    class InvalidInstructionException : Exception
    {
    }
}
