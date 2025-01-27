using AutoFocusCCD.Config;
using AutoFocusCCD.Forms.Setting;
using AutoFocusCCD.Forms.Tools;
using AutoFocusCCD.SQLite;
using AutoFocusCCD.Utilities;
using GitHub.secile.Video;
using NLog;
using NLog.Config;
using NLog.Targets;
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
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Main()
        {
            InitializeComponent();
            InitializeSerial();
            InitializeCapture();
            InitializeProcess();
            ShowAppVersion();

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Assembly.GetExecutingAssembly().GetName().Name, "NLog.config");
            if (File.Exists(path))
            {
                // Load NLog configuration from file
                LogManager.Configuration = new XmlLoggingConfiguration(path);
                Logger.Info("Load configuration from file: " + path);
            }
            else
            {
                SetDefaultNLogConfiguration(path);
                Logger.Error("Not found configuration file, set default configuration");
            }

            timerDateTime.Start();
        }

        public string[] baudList = { "9600", "19200", "38400", "57600", "115200" };


        public static string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Assembly.GetExecutingAssembly().GetName().Name);
        public static string PreferencesPath() => Path.Combine(path, Properties.Resources.Preferences);

        public static PreferencesConfig Preferences() => PreferencesConfigLoader.Load(PreferencesPath());

        private void Main_Load(object sender, EventArgs e)
        {

            using (SQLite.Product db = new SQLite.Product())
            {
#if DEBUG
                db.SyncTable();
#else
                db.CreateTable();
#endif
            }

            using (var db = new Boxes())
            {
#if DEBUG
                db.SyncTable();
#else
                db.CreateTable();
#endif

            }

            using (var db = new SQLite.History())
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
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Assembly.GetExecutingAssembly().GetName().Name);
            //preferences = PreferencesConfigLoader.Load(Path.Combine(path, "preferences.json"));
            Logger.Info("Application started");
            ClearFileServer();

            string pathSys = Preferences().FileSystem.Path;
            string pathDefult = PreferencesConfigLoader.LoadDefault().FileSystem.Path;
            if (pathDefult == pathSys)
            {
                path = Path.Combine(pathSys, "system", "images");
            }
            else
            {
                path = Path.Combine(pathSys, "images");
            }


            // chack and remove old file history
            /**
             * path: image/model/--folder--(delete) 
             *      
             */

            RemoveOldFiles();
        }

        private void RemoveOldFiles()
        {
            try
            {
                var deleteFileAfterDays= Preferences().FileSystem.DeleteFileAfterDays;
                var path = Preferences().FileSystem.Path;

                string pathDefult = PreferencesConfigLoader.LoadDefault().FileSystem.Path;
                if (pathDefult == path)
                {
                    path = Path.Combine(path, "system", "images");
                }
                else
                {
                    path = Path.Combine(path, "images");
                }

                /* -----------------
                |   productName -> datefolder(delete it )
                |
                */
                if (System.IO.Directory.Exists(path))
                {
                    Task.Run(() => 
                    {
                            //
                            var productDirectories = Directory.GetDirectories(path);
                            foreach (var pDir in productDirectories)
                            {
                                Logger.Info("************************************");

                                var productName = Path.GetFileName(pDir);
                                var dateDirectories = Directory.GetDirectories(pDir);
                                Logger.Info("Product name: " + productName);
                                foreach (var dDir in dateDirectories)
                                {
                                    var date = Path.GetFileName(dDir);
                                    var info = new DirectoryInfo(dDir);
                                    var createFolder = info.CreationTime;
                                    Console.WriteLine("Create folder: " + createFolder);
                                    Console.WriteLine("Total : " + DateTime.Now.Subtract(createFolder).TotalDays);
                                    Logger.Info("Date folder: " + date + ", " + DateTime.Now.Subtract(createFolder).TotalDays);
                                    if (DateTime.Now.Subtract(createFolder).TotalDays > deleteFileAfterDays)
                                    {
                                        try
                                        {
                                            Directory.Delete(dDir, true);
                                            Logger.Info("Remove old files: " + dDir);
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Error("Error remove old files: " + ex.Message);
                                        }
                                    }
                                }
                            }
                    });
                  
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error remove old files: " + ex.Message);
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadDevices();
        }
        private void CheckCreateFolder()
        {
            //if (preferences == null) return;
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if(!Directory.Exists(Path.Combine(path, Properties.Resources.PATH_IMAGE_CAPTURE)))
            {
                Directory.CreateDirectory(Path.Combine(path, Properties.Resources.PATH_IMAGE_CAPTURE));
            }
        }

        private void SetDefaultNLogConfiguration(string path)
        {
            // Step 1. Create configuration object
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration
            var fileTarget = new FileTarget("logfile")
            {
                FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Assembly.GetExecutingAssembly().GetName().Name, "logs", "${shortdate}.log"),
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 10,
                Layout = "${longdate} | ${level:uppercase=true} | ${message}",
            };

            // Step 3. Define rules
            config.AddTarget(fileTarget);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
            // Step 4. Activate the configuration
            LogManager.Configuration = config;
            LogManager.Setup();

            try
            {
                // Step 5. Save the configuration to a file
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                string xmlContent = $@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<nlog xmlns=""http://www.nlog-project.org/schemas/NLog.xsd""
xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
    <targets>
        <target xsi:type=""File"" name=""logfile"" fileName=""{fileTarget.FileName}""
        archiveEvery=""Day"" maxArchiveFiles=""10""
        layout=""${{longdate}} | ${{level:uppercase=true}} | ${{message}}"" />
    </targets>
    <rules>
        <logger name=""*"" minlevel=""Info"" writeTo=""logfile"" />
    </rules>
</nlog>";

                File.WriteAllText(path, xmlContent);

                Logger.Info("Save default NLog configuration to file: " + path);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Can't save default NLog configuration to file: " + path);
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

                // get model and send controls relay
                processValidate();



            }
        }

        private SQLite.Product _product = null;
        private void processValidate()
        {           

            if(txtQr.Text.Length < 7)
            {
                MessageBox.Show("QR code is invalid.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string qr = txtQr.Text.Substring(0, 7);
            
            this.lbTitle.Text = "Loading...";
            this.lbTitle.BackColor = Color.Yellow;
            this.lbTitle.ForeColor = Color.Black;
            _product = SQLite.Product.Get(qr);

            if (_product == null)
            {
                this.lbTitle.Text = "Model not found";
                this.lbTitle.BackColor = Color.Red;
                this.lbTitle.ForeColor = Color.White;
                return;
            }

            this.lbTitle.Text = "Waiting for start...";
            this.lbTitle.BackColor = Color.Yellow;
            this.lbTitle.ForeColor = Color.Black;
            if(_product.Type == 0)
            {
                this.deviceControl.SetRelay(Utilities.DeviceControl.Mode2Type.RELAY_6V_NOT_PWM, true);
            }
            else
            {
                this.deviceControl.SetRelay(Utilities.DeviceControl.Mode2Type.RELAY_4V6_PWM, true);
            }
        }

        private void modelsCCDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isConnect = false;
            if(btnConnect.Text == "Disconnect")
            {
                isConnect = true;
                btnConnect.PerformClick();
            }

            using (var product = new Forms.Setting.Product())
            {
                product.ShowDialog();
            }

            if (btnConnect.Text == "Connect" && isConnect == true)
            {
                btnConnect.PerformClick();
            }

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
            this.enhancedPacketHandler?.Close();
            this.camera?.Release();
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            try
            {
                if (btn.Text == "Connect")
                {
                    if (cmbDevices.SelectedIndex < 0)
                    {
                        throw new Exception("Please select a device first.");
                    }

                    if (cmbFormats.SelectedIndex < 0)
                    {
                        throw new Exception("Please select a format first.");
                    }

                    if (cmbCOMPort.SelectedIndex < 0)
                    {
                        throw new Exception("Please select COM port.");
                    }

                    if (cmbBaud.SelectedIndex < 0)
                    {
                        throw new Exception("Please select baud rate.");
                    }
                    
                    btn.Text = "Conneing...";
                    btn.Enabled = false;
                    await Task.Delay(100);
     
                    await Task.Run(() =>
                    {
                        if(InvokeRequired)
                        {
                            Invoke(new Action(() =>
                            {
                                string port = cmbCOMPort.SelectedItem.ToString();
                                int baud = int.Parse(cmbBaud.SelectedItem.ToString());

                                this.ConnectCamera();
                                this.enhancedPacketHandler?.Begin(port, baud);
                            }));
                        }

                    });

                    timerOutSerial.Start();
                    btn.Text = "Disconnect";
                    btn.Enabled = true;
                }
                else
                {
                    btn.Enabled = false;
                    btn.Text = "Disconn...";
                    await Task.Run(() =>
                    {
                        this.Stop();
                        this.enhancedPacketHandler.Close();
                    });

                    btn.Text = "Connect";
                    btn.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                this.Stop();
                Logger.Error(ex, "Error connecting camera: " + ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btn.Text = "Connect";
            }
            finally
            {
                Console.WriteLine("open and close ... :");
                this.lbVoltage.Text = "0.00V";
                this.lbCurrent.Text = "0.00mA";
                btn.Enabled = true;
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new Forms.Setting.Preferences();

            form.ShowDialog();
            // Reload preferences
            // preferences = PreferencesConfigLoader.Load(path);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string version = "";
            string company = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            string description = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
            string product = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;
            string title = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
            string copyright = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;

#if DEBUG
            version = Assembly.GetExecutingAssembly().GetName().Version.ToString() + " (Debug)";
#else
            try
            {
             version = ApplicationDeployment.CurrentDeployment.CurrentVersion == null ? Assembly.GetExecutingAssembly().GetName().Version.ToString() : ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();

            }
            catch
            {
                version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            // version = ApplicationDeployment.CurrentDeployment.CurrentVersion;
#endif

            var taskDialog = new TaskDialog
            {
                WindowTitle = $"{title} - About",
                MainInstruction = $"{product}",
                Content =
                $"Version: {version}\n" +
                $"Company: {company}" +
                $"\n {description}" +
                $"\n© " +
                $"{DateTime.Now.Year}" +
                $" All rights reserved.",
                MainIcon = TaskDialogIcon.Information,
            };

            taskDialog.AllowDialogCancellation = true;
            taskDialog.Buttons.Add(new TaskDialogButton("Close"));

            taskDialog.ShowDialog();
        }

        private void manageModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileManagement fileManagement = new FileManagement();

            fileManagement.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        IOSimulate io = null;
        private void iToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (io == null || io.IsDisposed)
            {
                io = new IOSimulate(this);
                io.Show();
            }
            else
            {
                io.BringToFront();
            }
        }

        private void timerOutSerial_Tick(object sender, EventArgs e)
        {
            timerOutSerial.Stop();
            this.deviceControl?.Command(0x01,0x06, Utilities.CommandType.Request, new byte[] { 0x00 });
        }

        private void toolStripStatusLabelSensor_TextChanged(object sender, EventArgs e)
        {
            // Check io simulate is open to return
            if(io != null && !io.IsDisposed)
            {
                return;
            }

        }

        public int countStart = 0;
        //public const int MAX_START = 3;
        private void timerOnStartProcess_Tick(object sender, EventArgs e)
        {
            if(countStart > 0)
            {
                this.lbTitle.Text = $"Start process in {this.countStart} seconds";
                this.lbTitle.BackColor = Color.Orange;
                this.lbTitle.ForeColor = Color.Black;
                countStart--;
                return;
            }

            timerOnStartProcess.Stop();
            // Start process

            this.StartProcess();

        }

        private void ClearFileServer()
        {
            string url = Preferences().Network.URL + "/api/v1/filemanager/clear-file";

            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    // post method
                    var res = client.PostAsync(url, null).Result;

                    if (res.IsSuccessStatusCode)
                    {
                        Logger.Info("Clear file server successful");
                    }
                    else
                    {
                        Logger.Error("Clear file server failed");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error clear file server: " + ex.Message);
            }
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(var history = new Forms.Setting.Historys())
            {
                history.ShowDialog();
            }
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = PreferencesConfigLoader.LoadDefault().FileSystem.Path;
            try
            {
                // Open windows explorer
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error opening path: " + ex.Message);
            }
        }

        private void workspecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Preferences().FileSystem.Path;
            try
            {
                // Open windows explorer
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error opening path: " + ex.Message);
            }
        }

        private async void btnClearMes_Click(object sender, EventArgs e)
        {
            try
            {
                btnClearMes.Enabled = false;
                this.SendText(Preferences().ClearMes.Message1);
                await Task.Delay(Preferences().ClearMes.Delay);
                this.SendText(txtEmp.Text);
            }
            catch (Exception ex) {
                Logger.Error("Clear MES :"+ ex.Message);
            }
            finally
            {
                btnClearMes.Enabled = true;
                this.txtQr.Focus();
            }
        }

        private void btnConfirmOK_Click(object sender, EventArgs e)
        {
            using(var db = SQLite.History.GetLast())
            {
                if (db != null)
                {
                    db.re_judgment = "OK";
                    db.UpdatedAt = SQliteDataAccess.GetDateTimeNow();
                    db.Update();
                }
            }
        }

        private void btnConfirmNG_Click(object sender, EventArgs e)
        {
            using (var db = SQLite.History.GetLast())
            {
                if (db != null)
                {
                    db.re_judgment = "NG";
                    db.UpdatedAt = SQliteDataAccess.GetDateTimeNow();
                    db.Update();
                }
            }
        }

        private void timerDateTime_Tick(object sender, EventArgs e)
        {
            lbDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}
