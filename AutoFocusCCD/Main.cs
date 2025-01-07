using AutoFocusCCD.SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
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
        }

        private void ShowAppVersion()
        {
            string version;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            else
            {
                version = "debug";
            }
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
    }
}
