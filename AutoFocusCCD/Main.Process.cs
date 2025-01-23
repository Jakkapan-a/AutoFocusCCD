using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFocusCCD
{
    partial class Main
    {
        private void InitializeProcess()
        {
        }

        private void StartProcess()
        {
            this.pictureBoxPredict.Visible = true;
            this.pictureBoxPredict.Image?.Dispose();
            this.pictureBoxPredict.Image = null;
            this.pictureBoxPredict.Image = Properties.Resources.Spinner_0_4s_800px;
        }
    }
}
