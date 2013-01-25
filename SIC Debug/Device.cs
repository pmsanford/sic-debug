using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SIC_Debug
{
    public class Device
    {
        public FileStream fs = null;
        int failurecount = 0;

        public byte Read()
        {
            if (fs == null)
                throw new DeviceNotInitialized();
            if (failurecount == 0)
            {
                failurecount = new Random().Next(1, 5); // This exists so that the device is periodically not ready
                return (byte)fs.ReadByte();             // Otherwise ready check code would never get hit
            }
            else
            {
                throw new DeviceNotReady();
            }
        }

        public void Write(byte outbyte)
        {
            if (fs == null)
                throw new DeviceNotInitialized();
            if (failurecount == 0)
            {
                if (outbyte == 0x10)
                {
                    char[] cnewline = Environment.NewLine.ToCharArray();
                    byte[] newline = new byte[cnewline.Length];
                    for (int i = 0; i < cnewline.Length; i++)
                        newline[i] = (byte)cnewline[i];
                    fs.Write(newline, 0, newline.Length);
                }
                else
                    fs.WriteByte(outbyte);
                fs.Flush();
            }
            else
            {
                throw new DeviceNotReady();
            }
            failurecount = new Random().Next(1, 5);
        }

        public bool TestDevice()
        {
            if (failurecount == 0)
            {
                return true;
            }
            else
            {
                failurecount--;
                return false;
            }
        }

        public byte GetByte()
        {
            int ifromfile = fs.ReadByte();
            byte fromfile = (byte)ifromfile;
            if (ifromfile == -1)
                return 0x04;
            if (fromfile == '\r')
            {
                fromfile = (byte)fs.ReadByte();
                if (fromfile == '\n')
                    return 0x10;
                else
                {
                    fs.Seek(-1, SeekOrigin.Current);
                    return (byte)'\r';
                }
            }
            if (fromfile == '\n')
            {
                return 0x10;
            }
            return fromfile;
        }
    }
}
