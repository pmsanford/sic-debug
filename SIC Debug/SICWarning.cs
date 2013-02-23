using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIC_Debug
{
    class SICWarning
    {
        public bool Abort;
        public string Message;
        public int? MemoryAddress;
        public string Location;

        public SICWarning(string message, string location, int? memoryAddress = null)
        {
            this.Abort = false;
            this.Message = message;
            this.Location = location;
            this.MemoryAddress = memoryAddress;
        }
    }
}
