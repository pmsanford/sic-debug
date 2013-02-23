using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIC_Debug
{
    public class SICEvent
    {
        public Instruction instruction;
        public bool Continue;
        public int PC;

        public SICEvent(Instruction inst, int PC)
        {
            this.instruction = inst;
            this.Continue = true;
            this.PC = PC;
        }
    }
}
