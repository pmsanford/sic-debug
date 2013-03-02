using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SIC_Debug
{
    class SimpleCharacterTerminal : IDevice
    {
        Queue<Keys> inputQueue = new Queue<Keys>();
        private Terminal termWindow;
        private RichTextBox termBox;
        private KeysConverter converter = new KeysConverter();

        public SimpleCharacterTerminal()
        {
            this.termWindow = new Terminal();
            this.termBox = this.termWindow.TerminalBox;
            this.termBox.KeyDown += new KeyEventHandler(keyDownHandler);
        }

        public byte Read()
        {
            if (inputQueue.Count == 0)
                throw new DeviceNotReady();
            Keys readKey = inputQueue.Dequeue();
            return (byte)readKey;
        }

        public void Write(byte outbyte)
        {
            if (this.termBox.SelectionStart == this.termBox.Text.Length && this.termBox.Lines.Length == 24 && this.termBox.Lines[23].Length == 80)
            {
                removeTopLine(this.termBox);
                termBox.AppendText(Environment.NewLine);
            }
            termBox.AppendText(((char)outbyte).ToString());
        }

        private static void removeTopLine(RichTextBox box)
        {
            int start = box.GetFirstCharIndexFromLine(0);
            int count = box.Lines[0].Length + 1;

            box.SelectionStart = start;
            box.SelectionLength = count;
            box.SelectedText = "";
        }

        public bool TestDevice()
        {
            if (inputQueue.Count > 0)
                return true;
            else
                return false;
        }

        private void keyDownHandler(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            inputQueue.Enqueue(e.KeyCode);
        }
    }
}
