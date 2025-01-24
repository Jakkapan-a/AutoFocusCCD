using AutoFocusCCD.Config;
using AutoFocusCCD.Forms.Setting;
using AutoFocusCCD.Forms.Tools;
using AutoFocusCCD.SQLite;
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
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Assembly.GetExecutingAssembly().GetName().Name);
            //preferences = PreferencesConfigLoader.Load(Path.Combine(path, "preferences.json"));
            Logger.Info("Application started");
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
            this.RefreshComboBoxWithList(cmbCOMPort, SerialPort.GetPortNames(), false);
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
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            try
            {
                if(btn.Text == "Connect")
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

                    this.ConnectCamera();

                    string port = cmbCOMPort.SelectedItem.ToString();
                    int baud = int.Parse(cmbBaud.SelectedItem.ToString());

                    this.enhancedPacketHandler?.Begin(port, baud);
                    timerOutSerial.Start();

                    btn.Text = "Disconnect";
                }
                else
                {
                    this.Stop();
                    this.enhancedPacketHandler.Close();
                    btn.Text = "Connect";
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
             version = ApplicationDeployment.CurrentDeployment.CurrentVersion == null ? Assembly.GetExecutingAssembly().GetName().Version.ToString() : ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            // version = ApplicationDeployment.CurrentDeployment.CurrentVersion;
            // version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
        public const int MAX_START = 3;
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

       
    }
}
