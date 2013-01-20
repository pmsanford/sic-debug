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
        Device[] devices;
        TextBox[] tbs;
        public Config(ref Device[] devices)
        {
            InitializeComponent();
            this.devices = devices;
            if (devices[0].fs != null)
                tbFile00.Text = devices[0].fs.Name;
            if (devices[1].fs != null)
                tbFileF1.Text = devices[1].fs.Name;
            if (devices[2].fs != null)
                tbFileF2.Text = devices[2].fs.Name;
            if (devices[3].fs != null)
                tbFileF3.Text = devices[3].fs.Name;
            if (devices[4].fs != null)
                tbFile04.Text = devices[4].fs.Name;
            if (devices[5].fs != null)
                tbFile05.Text = devices[5].fs.Name;
            if (devices[6].fs != null)
                tbFile06.Text = devices[6].fs.Name;
            tbs = new TextBox[7];
            tbs[0] = tbFile00;
            tbs[1] = tbFileF1;
            tbs[2] = tbFileF2;
            tbs[3] = tbFileF3;
            tbs[4] = tbFile04;
            tbs[5] = tbFile05;
            tbs[6] = tbFile06;
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
                if (devices[i].fs != null)
                    devices[i].fs.Close();
                devices[i].fs = null;
            }
            for (int i = 0; i < tbs.Length; i++)
            {
                if (tbs[i].Text != "")
                {
                    try
                    {
                        devices[i].fs = new FileStream(tbs[i].Text, FileMode.OpenOrCreate);
                    }
                    catch (Exception)
                    {
                        string fileid;
                        if (i == 1 || i == 2 || i == 3)
                            fileid = "F" + (i + 2).ToString();
                        else
                            fileid = "0" + (i + 2).ToString();
                        MessageBox.Show("Error opening file " + fileid + ". If you don't want a file open, leave it blank");
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
