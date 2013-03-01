using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SIC_Debug
{
    public partial class Config : Form
    {
        SimpleFileDevice[] devices;
        IDevice[] outDevices;
        TextBox[] tbs;
        public Config(ref IDevice[] devices)
        {
            InitializeComponent();
            this.devices = new SimpleFileDevice[devices.Length];
            this.outDevices = devices;
            tbs = new TextBox[7];
            tbs[0] = tbFile00;
            tbs[1] = tbFileF1;
            tbs[2] = tbFileF2;
            tbs[3] = tbFileF3;
            tbs[4] = tbFile04;
            tbs[5] = tbFile05;
            tbs[6] = tbFile06;

            for (int i = 0; i < 7; i++)
            {
                if (devices[i] is SimpleFileDevice)
                {
                    if (((SimpleFileDevice)devices[i]).fs != null)
                        tbs[i].Text = ((SimpleFileDevice)devices[i]).fs.Name;
                    this.devices[i] = (SimpleFileDevice)devices[i];
                }
                else
                {
                    this.devices[i] = null;
                    tbs[i].Text = "Not a file device.";
                }
            }
        }

        public void UpdateBoxes()
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = false;
            int senderid = Convert.ToInt32(((Button)sender).Tag.ToString());
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                tbs[senderid].Text = dialog.FileName;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tbs.Length; i++)
            {
                if (devices[i] != null)
                {
                    if (((SimpleFileDevice)outDevices[i]).fs != null)
                        ((SimpleFileDevice)outDevices[i]).fs.Close();
                    ((SimpleFileDevice)outDevices[i]).fs = null;
                }
            }
            for (int i = 0; i < tbs.Length; i++)
            {
                if (tbs[i].Text != "" && tbs[i].Text != "Not a file device.")
                {
                    try
                    {
                        outDevices[i] = new SimpleFileDevice(tbs[i].Text);
                    }
                    catch (Exception ex)
                    {
                        string fileid;
                        if (i == 1 || i == 2 || i == 3)
                            fileid = "F" + (i + 2).ToString();
                        else
                            fileid = "0" + (i + 2).ToString();
                        MessageBox.Show("Error opening file " + fileid + ". If you don't want a file open, leave it blank");
                        string a =ex.Message;
                        return;
                    }
                }
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Config_Load(object sender, EventArgs e)
        {
        }
    }
}
