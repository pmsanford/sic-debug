using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIC_Debug
{
    public interface IDevice
    {
        byte Read();
        void Write(byte outbyte);
        bool TestDevice();
    }
}
