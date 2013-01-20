using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIC_Debug
{
    public enum OpCode
    {
        ADD = 0x18,
        SUB = 0x1C,
        J = 0x3C,
        JEQ = 0x30,
        JGT = 0x34,
        JLT = 0x38,
        MUL = 0x20,
        DIV = 0x24,
        STA = 0x0C,
        LDA = 0x00,
        COMP = 0x28,
        TIX = 0x2C,
        LDB = 0x68,
        STB = 0x78,
        STL = 0x14,
        LDL = 0x08,
        RSUB = 0x4C,
        JSUB = 0x48,
        LDS = 0x6C,
        LDT = 0x74,
        STS = 0x7C,
        STT = 0x84,
        ADDR = 0x90,
        CLEAR = 0xB4,
        COMPR = 0xA0,
        DIVR = 0x9C,
        MULR = 0x98,
        RMO = 0xAC,
        SUBR = 0x94,
        TIXR = 0xB8,
        LDX = 0x04,
        STX = 0x10,
        LDCH = 0x50,
        STCH = 0x54,
        SHIFTL = 0xA4,
        SHIFTR = 0xA8,
        RD = 0xD8,
        TD = 0xE0,
        WD = 0xDC,
        AND = 0x40,
        OR = 0x44,
    }

    public enum Register
    {
        A = 0,
        B = 3,
        L = 2,
        S = 4,
        T = 5,
        X = 1,
    }
}
