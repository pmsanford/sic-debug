using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SIC_Debug
{
    class Highlight
    {
        public int Address;
        public int Length;
        public Color ForeColor;
        public Color BackColor;

        public Highlight(int address, int length, Color foreColor, Color backColor)
        {
            this.Address = address;
            this.Length = length;
            this.ForeColor = foreColor;
            this.BackColor = backColor;
        }
    }
}
