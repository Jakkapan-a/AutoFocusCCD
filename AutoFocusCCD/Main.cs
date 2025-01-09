using AutoFocusCCD.SQLite;
using GitHub.secile.Video;
using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFocusCCD
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            ShowAppVersion();
        }
        public string[] baudList = { "9600", "19200", "38400", "57600", "115200" };

        private void Main_Load(object sender, EventArgs e)
        {

            using (Product db = new Product())
            {
#if DEBUG
                db.SyncTable();
#else
                db.CreateTable();
#endif
            }

            using (Boxes db = new Boxes())
            {
#if DEBUG
                db.SyncTable();
#else
                db.CreateTable();
#endif
            }

            /**
             * init Device
             */
            LoadDevices();
            CheckCreateFolder();
        }


        private void CheckCreateFolder()
        {

            // Path history log and test
            string path = Properties.Settings.Default.PATH == "" ? Properties.Settings.Default.PATH : "./";


            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if(!Directory.Exists(Path.Combine(path, Properties.Resources.PATH_IMAGE_CAPTURE)))
            {
                Directory.CreateDirectory(Path.Combine(path, Properties.Resources.PATH_IMAGE_CAPTURE));
            }

            if (!Directory.Exists(Path.Combine(path, Properties.Resources.PATH_IMAGE_CAPTURE)))
            {
                Directory.CreateDirectory(Path.Combine(path, Properties.Resources.PATH_IMAGE_CAPTURE));
            }
        }


        private void LoadDevices()
        {
            string[] devices = UsbCamera.FindDevices();
            if (devices.Length == 0) return;

            this.RefreshComboBoxWithList(cmbDevices, devices);
            this.RefreshComboBoxWithList(cmbCOMPort, SerialPort.GetPortNames(), true);
            this.RefreshComboBoxWithList(cmbBaud, baudList);
        }

        private void RefreshComboBoxWithList(System.Windows.Forms.ComboBox comboBox, IList<string> items, bool selectLast = false)
        {
            int oldSelectedIndex = comboBox.SelectedIndex;
            comboBox.Items.Clear();
            comboBox.Items.AddRange(items.ToArray());
            if (comboBox.Items.Count <= 0) return;

            if (oldSelectedIndex > 0 && oldSelectedIndex < comboBox.Items.Count)
            {
                comboBox.SelectedIndex = oldSelectedIndex;
            }
            else
            {
                comboBox.SelectedIndex = selectLast ? comboBox.Items.Count - 1 : 0;
            }
        }

        private void ShowAppVersion()
        {
            string version;
#if DEBUG
            version = "debug";
#else
            // version = ApplicationDeployment.CurrentDeployment.CurrentVersion == null ? "debug" : ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            
            // version = ApplicationDeployment.CurrentDeployment.CurrentVersion;

            version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
#endif
            this.Text += $" - {version}";
        }

        private void txtEmp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (string.IsNullOrEmpty(txtEmp.Text))
                {
                    MessageBox.Show("Please enter employee.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.ActiveControl = txtQr;
                this.txtQr.Focus();
            }
        }

        private void txtQr_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (string.IsNullOrEmpty(txtQr.Text))
                {
                    MessageBox.Show("Please enter QR code.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            }
        }


        private void modelsCCDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(btnConnect.Text == "Disconnect")
            {
                btnConnect.PerformClick();
            }

            var product = new Forms.Setting.Product();
            product.ShowDialog();

        }

        private void cmbDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            int cameraIndex = 0;

            if (cmb.SelectedIndex >= 0)
            {
                cameraIndex = cmb.SelectedIndex;
            }

            var formats = UsbCamera.GetVideoFormat(cameraIndex);

            string[] formatList = new string[formats.Length];

            for (int i = 0; i < formats.Length; i++)
            {
                formatList[i] = formats[i].ToString();
            }

            RefreshComboBoxWithList(cmbFormats, formatList);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Stop();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            try
            {
                if(btn.Text == "Connect")
                {

                    this.ConnectCamera();
                    btn.Text = "Disconnect";
                }
                else
                {
                    this.Stop();
                    btn.Text = "Connect";
                }
            }
            catch (Exception ex)
            {
                this.Stop();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btn.Text = "Connect";
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new Forms.Setting.Preferences();

            form.ShowDialog();

        }
    }
}
