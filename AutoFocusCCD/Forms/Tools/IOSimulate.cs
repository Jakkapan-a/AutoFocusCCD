using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFocusCCD.Forms.Tools
{
    public partial class IOSimulate : Form
    {
        private Main main;
        public IOSimulate(Main main)
        {
            InitializeComponent();
            this.main = main;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(txtTextData.Text.Length == 0)
            {
                MessageBox.Show("Please enter data to send.");
                return;
            }

            main.SendText(txtTextData.Text);
        }
    }
}
