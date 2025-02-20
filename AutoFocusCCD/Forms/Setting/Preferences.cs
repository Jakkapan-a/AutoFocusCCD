using AutoFocusCCD.Config;
using Multi_Camera_MINI_AOI_V3.Utilities;
using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFocusCCD.Forms.Setting
{
    public partial class Preferences : Form
    {
        public Preferences()
        {
            InitializeComponent();
        }

        private PreferencesConfig preferencesConfig = null;
        private void Preferences_Load(object sender, EventArgs e)
        {
            loadPreferences();
        }

        private void loadPreferences()
        {
            this.preferencesConfig = Main.Preferences();
            if (preferencesConfig == null) return;
            
            txtURL.Text = preferencesConfig.Network.URL;
            txtURL2.Text = preferencesConfig.Network.URL;

            txtClearMessage1.Text = preferencesConfig.ClearMes.Message1;
            nClearDelay.Value = preferencesConfig.ClearMes.Delay;
            txtClearMessage2.Text = preferencesConfig.ClearMes.Message2;

            cbAllowNG.Checked = preferencesConfig.OptionNG.AllowSendNG;
            txtKeySendNG.Text = preferencesConfig.OptionNG.Key;
            nKeySendDelay.Value = preferencesConfig.OptionNG.Delay;
            txtMessageSendNG.Text = preferencesConfig.OptionNG.Message;
            txtKeySendDescription.Text = preferencesConfig.OptionNG.Description;

            nThreshold.Value = preferencesConfig.Processing.Threshold;
            cbType.SelectedIndex = preferencesConfig.Processing.Type;
            nTimeStart.Value = preferencesConfig.Processing.TimeStart;
            nIntervalStart.Value = preferencesConfig.Processing.Interval;

            nDeleteFileAfterDays.Value = preferencesConfig.FileSystem.DeleteFileAfterDays;
            txtPath.Text = preferencesConfig.FileSystem.Path;

            cbRectangle.Checked = preferencesConfig.Other.Rectangle;
            cbByPass.Checked = preferencesConfig.Other.ByPass;

        }

        private void updatePreferences()
        {
            if (preferencesConfig == null) return;

            preferencesConfig.Network.URL = txtURL.Text;
            preferencesConfig.Network.URL2 = txtURL2.Text;

            preferencesConfig.ClearMes.Message1 = txtClearMessage1.Text;
            preferencesConfig.ClearMes.Delay = (int)nClearDelay.Value;
            preferencesConfig.ClearMes.Message2 = txtClearMessage2.Text;
            preferencesConfig.OptionNG.AllowSendNG = cbAllowNG.Checked;
            preferencesConfig.OptionNG.Key = txtKeySendNG.Text;
            preferencesConfig.OptionNG.Delay = (int)nKeySendDelay.Value;
            preferencesConfig.OptionNG.Message = txtMessageSendNG.Text;
            preferencesConfig.OptionNG.Description = txtKeySendDescription.Text;
            preferencesConfig.Processing.Threshold = (int)nThreshold.Value;
            preferencesConfig.Processing.TimeStart = (int)nTimeStart.Value;
            preferencesConfig.Processing.Interval = (int)nIntervalStart.Value;
            preferencesConfig.Processing.Type = cbType.SelectedIndex;
            preferencesConfig.FileSystem.DeleteFileAfterDays = (int)nDeleteFileAfterDays.Value;
            preferencesConfig.FileSystem.Path = txtPath.Text;
            preferencesConfig.Other.Rectangle = cbRectangle.Checked;
            preferencesConfig.Other.ByPass = cbByPass.Checked;

            // Save and update 
            PreferencesConfigLoader.Save(Main.PreferencesPath(), preferencesConfig);
        }

        private void factoryResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            taskDialog1.AllowDialogCancellation = true;
            taskDialog1.MainInstruction = "Are you sure you want to reset to factory settings?";
            taskDialog1.Content = "This will reset all settings to default. This action cannot be undone.";
            taskDialog1.MainIcon = TaskDialogIcon.Warning;
            taskDialog1.WindowTitle = "Factory Reset";
            taskDialog1.CenterParent = true;

           if(taskDialog1.ShowDialog(this) == _taskDialogButtonOK)
            {
                PreferencesConfig config = PreferencesConfigLoader.LoadDefault();
                PreferencesConfigLoader.Save(Main.PreferencesPath(), config);
                loadPreferences();
            }
        }

        private async void btnTestNetwork_Click(object sender, EventArgs e)
        {

            bool pass = false;
            try{
                btnTestNetwork.Enabled = false;

                string txtURL1 = txtURL.Text;
                string txtURL2 = this.txtURL2.Text;
                bool txt2Visible = this.txtURL2.Visible;

                await Task.Run(() =>
                {
                    using (var client = new HttpClient())
                    {
                        // set time out client
                        //client.
                        client.Timeout  = new TimeSpan(0, 0, 2); // 3 seconds
                        var res = client.GetAsync(txtURL1).Result;
                        if (res.IsSuccessStatusCode)
                        {
                            Main.Logger.Info("Network test successful 1", "Test Network", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            pass = true;
                        }
                        else
                        {
                            throw new Exception("Connection failed 1");
                        }
                    }
                    /*
                    if (txt2Visible)
                    {
                        using (var client = new HttpClient())
                        {
                            client.Timeout = new TimeSpan(0, 0, 2); // 3 seconds
                            var res = client.GetAsync(txtURL1).Result;
                            if (res.IsSuccessStatusCode)
                            {
                                Main.Logger.Info("Network test successful 2", "Test Network", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                MessageBox.Show("Connection successful 2", "Test Network", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                throw new Exception("Connection failed 2");
                            }
                        }
                    }*/
                });
            }
            catch(Exception ex)
            {
                pass = false;
                Main.Logger.Error("Error testing network: " + ex.Message);
                MessageBox.Show("Connection failed", "Test Network", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (pass)
                {
                    MessageBox.Show("Connection successful", "Test Network", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                btnTestNetwork.Enabled = true;
            }
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox != null && textBox == txtURL)
            {
                //if(Extensions.IsValidURL(txtURL.Text))
                //{
                //    txtURL.ForeColor = Color.Black;
                //}
                //else
                //{
                //    txtURL.ForeColor = Color.Red;
                //    return;
                //}

                //if http?

                if (!txtURL.Text.StartsWith("http://") && !txtURL.Text.StartsWith("https://"))
                {
                    txtURL.Text = "http://" + txtURL.Text;
                }
            }

            if (textBox != null && textBox == txtURL2 && txtURL2.Visible)
            {
                if (Extensions.IsValidURL(txtURL2.Text))
                {
                    txtURL.ForeColor = Color.Black;
                }
                else
                {
                    txtURL.ForeColor = Color.Red;
                    return;
                }
            }

            timerUpdate.Stop();
            timerUpdate.Start();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            updatePreferences();
        }


        private void nInput_ValueChanged(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            timerUpdate.Start();
        }


        private void btnMesTest_Click(object sender, EventArgs e)
        {

        }

        private void btnTestNG_Click(object sender, EventArgs e)
        {

        }

        private void btnResetFilePath_Click(object sender, EventArgs e)
        {
            PreferencesConfig config = PreferencesConfigLoader.LoadDefault();
            txtPath.Text = config.FileSystem.Path;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.SelectedPath = txtPath.Text;
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = dialog.SelectedPath;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                // Open windows explorer
                System.Diagnostics.Process.Start("explorer.exe", txtPath.Text);
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error opening path: " + ex.Message);
            }
        }

        private void cbAllowNG_CheckedChanged(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            timerUpdate.Start();
        }

        private void txtKeySendDescription_TextChanged(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            timerUpdate.Start();
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            timerUpdate.Start();
        }

        private void cbRectangle_CheckedChanged(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            timerUpdate.Start();
        }
    }
}
