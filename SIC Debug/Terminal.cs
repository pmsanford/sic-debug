using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SIC_Debug
{
    public partial class Terminal : Form
    {
        public RichTextBox TerminalBox
        {
            get
            {
                return this.rtbTerminal;
            }
        }

        public Terminal()
        {
            InitializeComponent();
        }

        private void Terminal_Load(object sender, EventArgs e)
        {
            this.rtbTerminal.Text = "";
        }
    }
}
