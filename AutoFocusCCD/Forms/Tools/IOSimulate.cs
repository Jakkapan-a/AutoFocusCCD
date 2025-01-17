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

        private void btnLED_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (button == btnLEDOff)
            {
                btnLEDRed.BackColor = Color.White;
                btnLEDGreen.BackColor = Color.White;
                btnLEDBlue.BackColor = Color.White;
                main?.deviceControl.TurnOffAllLEDs();
            }
            else if (button == btnLEDRed)
            {
                if(button.BackColor == Color.White)
                {
                    btnLEDRed.BackColor = Color.Red;
                    btnLEDGreen.BackColor = Color.White;
                    btnLEDBlue.BackColor = Color.White;
                    main?.deviceControl.SetLED(Utilities.DeviceControl.Mode2Type.LED_RED, true);
                }
                else
                {
                    btnLEDRed.BackColor = Color.White;
                    main?.deviceControl.SetLED(Utilities.DeviceControl.Mode2Type.LED_RED, false);
                }
            }
            else if (button == btnLEDGreen)
            {
                if (button.BackColor == Color.White)
                {
                    btnLEDRed.BackColor = Color.White;
                    btnLEDGreen.BackColor = Color.Green;
                    btnLEDBlue.BackColor = Color.White;
                    main?.deviceControl.SetLED(Utilities.DeviceControl.Mode2Type.LED_GREEN, true);
                }
                else
                {
                    btnLEDGreen.BackColor = Color.White;
                    main?.deviceControl.SetLED(Utilities.DeviceControl.Mode2Type.LED_GREEN, false);
                }
            }
            else if (button == btnLEDBlue)
            {
                if (button.BackColor == Color.White)
                {
                    btnLEDRed.BackColor = Color.White;
                    btnLEDGreen.BackColor = Color.White;
                    btnLEDBlue.BackColor = Color.Blue;
                    main?.deviceControl.SetLED(Utilities.DeviceControl.Mode2Type.LED_BLUE, true);
                }
                else
                {
                    btnLEDBlue.BackColor = Color.White;
                    main?.deviceControl.SetLED(Utilities.DeviceControl.Mode2Type.LED_BLUE, false);
                }
            }
        }

        private void btnR_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button == btnRNOT)
            {
                if (button.BackColor == Color.White)
                {
                    btnRNOT.BackColor = Color.Red;
                    btnRPVM.BackColor = Color.White;
                    btnROff.BackColor = Color.White;
                    main?.deviceControl.SetRelay(Utilities.DeviceControl.Mode2Type.RELAY_6V_NOT_PWM, true);
                }
                else
                {
                    btnRNOT.BackColor = Color.White;
                    main?.deviceControl.SetRelay(Utilities.DeviceControl.Mode2Type.RELAY_6V_NOT_PWM, false);
                }
            }
            else if (button == btnRPVM)
            {
                if (button.BackColor == Color.White)
                {
                    btnRNOT.BackColor = Color.White;
                    btnRPVM.BackColor = Color.Red;
                    btnROff.BackColor = Color.White;
                    main?.deviceControl.SetRelay(Utilities.DeviceControl.Mode2Type.RELAY_4V6_PWM, true);
                }
                else
                {
                    btnRPVM.BackColor = Color.White;
                    main?.deviceControl.SetRelay(Utilities.DeviceControl.Mode2Type.RELAY_4V6_PWM, false);
                }
            }
            else if (button == btnROff)
            {
                btnRNOT.BackColor = Color.White;
                btnRPVM.BackColor = Color.White;
                btnROff.BackColor = Color.White;
                main?.deviceControl.TurnOffAllRelays();
            }
            
        }

        private void IOSimulate_Load(object sender, EventArgs e)
        {
            btnRNOT.BackColor = Color.White;
            btnRPVM.BackColor = Color.White;
            btnROff.BackColor = Color.White;

            btnLEDRed.BackColor = Color.White;
            btnLEDGreen.BackColor = Color.White;
            btnLEDBlue.BackColor = Color.White;
        }
    }
}
