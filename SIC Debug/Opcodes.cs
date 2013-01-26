using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIC_Debug
{
    public enum OpCode
    {                   // In below, "memval" refers to value of memory at calculated address
                        // Unless otherwise noted, instructions are 3 or 4 byte
        ADD = 0x18,     // Add, store A + memval in A
        SUB = 0x1C,     // Subtract, store A - memval in A
        J = 0x3C,       // Jump, load calculated address into Program Counter
        JEQ = 0x30,     // Jump if equal, load calculated address into Program Counter if Status Word is =
        JGT = 0x34,     // Jump if greater, load calculated address into Program Counter if Status Word is >
        JLT = 0x38,     // Jump if lesser, load calculated address into Program Counter if Status Word is <
        MUL = 0x20,     // Multiply, store A * memval in A
        DIV = 0x24,     // Divide, store A / memval in A
        STA = 0x0C,     // Store A, store value in A at calculated address
        LDA = 0x00,     // Load A, load memval into A
        COMP = 0x28,    // Compare A to memval, set Status Word appropriately
        TIX = 0x2C,     // ???, Add one to X, compare X to memval, set Status Word appropriately
        LDB = 0x68,     // Load B, load memval into B
        STB = 0x78,     // Store B, store value in B at calculated address
        STL = 0x14,     // Store L, store value in L at calculated address
        LDL = 0x08,     // Load L, load memval into L
        RSUB = 0x4C,    // Return from Subroutine, load value in L into Program Counter
        JSUB = 0x48,    // Jump to Subroutine, load value from Program Counter into L, load memval into Program Counter
        LDS = 0x6C,     // Load S, load memval into S (Extended only)
        LDT = 0x74,     // Load T, load memval into T (Extended only)
        STS = 0x7C,     // Store S, store value in S at calculated address
        STT = 0x84,     // Store T, store value in T at calculated address
        ADDR = 0x90,    // Add from Registers, two byte, add the values in register 1 and register 2 together and store them in register 2 (Extended only)
        CLEAR = 0xB4,   // Clear register, two byte, load 0 into register 1
        COMPR = 0xA0,   // Compare registers, two byte, compare register 1 to register 2, store value in Status Word (Extended only)
        DIVR = 0x9C,    // Divide registers, two byte, store r2 / r1 into r2 (Extended only)
        MULR = 0x98,    // Multiply registers, two byte, store r2 * r1 into r2 (Extended only)
        RMO = 0xAC,     // Register Move, two byte, store value from r1 in r2 (Extended only)
        SUBR = 0x94,    // Subtract registers, two byte, store r2 - r1 in r2 (Extended only)
        TIXR = 0xB8,    // ??? register, two byte, as TIX but compare X to r1 (Extended only)
        LDX = 0x04,     // Load X, load memval into X
        STX = 0x10,     // Store X, store value from X at the calculated address
        LDCH = 0x50,    // Load Character, loads a single byte from memory into the rightmost byte of A
        STCH = 0x54,    // Store Character, stores a single byte from A at the calculated address
        SHIFTL = 0xA4,  // Shift left, left circular shift of r1 by r2+1 bits (Extended only)
        SHIFTR = 0xA8,  // Shift right, right circular shift of r1 by r2+1 bits (Extended only)
        RD = 0xD8,      // Read Device, read one byte of data from device specified by memval into the rightmost char of A
        TD = 0xE0,      // Test Device, test device specified by memval, set Status Word appropriately
        WD = 0xDC,      // Write Device, write the rightmost byte of A to the device specified by memval
        AND = 0x40,     // AND, load value of A & memval into A
        OR = 0x44,      // OR, load value of A | memval into A
    }

    public enum Register
    {
        A = 0, // Accumulator
        B = 3, // Base (for addressing)
        L = 2, // Linkage (for subroutines)
        S = 4, // General purpose
        T = 5, // General purpose
        X = 1, // Index (for addressing)
    }
}
