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
        public Exception Ex;
        public string Message;

        public SICEvent(Instruction inst, int PC)
        {
            this.instruction = inst;
            this.Continue = true;
            this.PC = PC;
            this.Ex = null;
            this.Message = null;
        }

        public SICEvent(Instruction inst, int PC, Exception ex, string message)
        {
            this.instruction = inst;
            this.Continue = false;
            this.PC = PC;
            this.Ex = ex;
            this.Message = message;
        }
    }
}
