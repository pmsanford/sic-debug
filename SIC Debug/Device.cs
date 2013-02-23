using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIC_Debug
{
    public abstract class Device
    {
        public abstract byte Read();
        public abstract void Write(byte outbyte);
        public abstract bool TestDevice();
    }
}
