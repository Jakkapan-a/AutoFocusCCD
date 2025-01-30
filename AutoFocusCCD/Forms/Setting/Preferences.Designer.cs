namespace AutoFocusCCD.Forms.Setting
{
    partial class Preferences
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preferences));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTestNetwork = new System.Windows.Forms.Button();
            this.txtURL2 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.nClearDelay = new System.Windows.Forms.NumericUpDown();
            this.btnMesTest = new System.Windows.Forms.Button();
            this.txtClearMessage2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtClearMessage1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtKeySendDescription = new System.Windows.Forms.RichTextBox();
            this.cbAllowNG = new System.Windows.Forms.CheckBox();
            this.nKeySendDelay = new System.Windows.Forms.NumericUpDown();
            this.btnTestNG = new System.Windows.Forms.Button();
            this.txtMessageSendNG = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtKeySendNG = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.nIntervalStart = new System.Windows.Forms.NumericUpDown();
            this.nTimeStart = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.nThreshold = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.nDeleteFileAfterDays = new System.Windows.Forms.NumericUpDown();
            this.btnResetFilePath = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cbByPass = new System.Windows.Forms.CheckBox();
            this.cbRectangle = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.factoryResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.taskDialog1 = new Ookii.Dialogs.WinForms.TaskDialog(this.components);
            this._taskDialogButtonOK = new Ookii.Dialogs.WinForms.TaskDialogButton(this.components);
            this._taskDialogButtonCancel = new Ookii.Dialogs.WinForms.TaskDialogButton(this.components);
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nClearDelay)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nKeySendDelay)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nIntervalStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nTimeStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nThreshold)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nDeleteFileAfterDays)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnTestNetwork);
            this.groupBox1.Controls.Add(this.txtURL2);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.txtURL);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(321, 106);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Network";
            // 
            // btnTestNetwork
            // 
            this.btnTestNetwork.Location = new System.Drawing.Point(240, 76);
            this.btnTestNetwork.Name = "btnTestNetwork";
            this.btnTestNetwork.Size = new System.Drawing.Size(75, 23);
            this.btnTestNetwork.TabIndex = 4;
            this.btnTestNetwork.Text = "Test";
            this.btnTestNetwork.UseVisualStyleBackColor = true;
            this.btnTestNetwork.Click += new System.EventHandler(this.btnTestNetwork_Click);
            // 
            // txtURL2
            // 
            this.txtURL2.Location = new System.Drawing.Point(42, 54);
            this.txtURL2.Name = "txtURL2";
            this.txtURL2.Size = new System.Drawing.Size(273, 20);
            this.txtURL2.TabIndex = 2;
            this.txtURL2.Text = "http://127.0.0.1:10010";
            this.txtURL2.Visible = false;
            this.txtURL2.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 57);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(35, 13);
            this.label15.TabIndex = 1;
            this.label15.Text = "URL2";
            this.label15.Visible = false;
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(42, 26);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(273, 20);
            this.txtURL.TabIndex = 2;
            this.txtURL.Text = "http://127.0.0.1:10010";
            this.txtURL.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "URL";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 523);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(694, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.nClearDelay);
            this.groupBox2.Controls.Add(this.btnMesTest);
            this.groupBox2.Controls.Add(this.txtClearMessage2);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtClearMessage1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 121);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(321, 157);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Option Clear MES";
            // 
            // nClearDelay
            // 
            this.nClearDelay.Location = new System.Drawing.Point(71, 67);
            this.nClearDelay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nClearDelay.Name = "nClearDelay";
            this.nClearDelay.Size = new System.Drawing.Size(244, 20);
            this.nClearDelay.TabIndex = 5;
            this.nClearDelay.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nClearDelay.ValueChanged += new System.EventHandler(this.nInput_ValueChanged);
            // 
            // btnMesTest
            // 
            this.btnMesTest.Location = new System.Drawing.Point(240, 128);
            this.btnMesTest.Name = "btnMesTest";
            this.btnMesTest.Size = new System.Drawing.Size(75, 23);
            this.btnMesTest.TabIndex = 4;
            this.btnMesTest.Text = "Test";
            this.btnMesTest.UseVisualStyleBackColor = true;
            this.btnMesTest.Click += new System.EventHandler(this.btnMesTest_Click);
            // 
            // txtClearMessage2
            // 
            this.txtClearMessage2.Location = new System.Drawing.Point(71, 93);
            this.txtClearMessage2.Name = "txtClearMessage2";
            this.txtClearMessage2.Size = new System.Drawing.Size(244, 20);
            this.txtClearMessage2.TabIndex = 2;
            this.txtClearMessage2.Text = "test";
            this.txtClearMessage2.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Delay";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Message 2";
            // 
            // txtClearMessage1
            // 
            this.txtClearMessage1.Location = new System.Drawing.Point(71, 41);
            this.txtClearMessage1.Name = "txtClearMessage1";
            this.txtClearMessage1.Size = new System.Drawing.Size(244, 20);
            this.txtClearMessage1.TabIndex = 2;
            this.txtClearMessage1.Text = "clear";
            this.txtClearMessage1.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Message 1";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtKeySendDescription);
            this.groupBox3.Controls.Add(this.cbAllowNG);
            this.groupBox3.Controls.Add(this.nKeySendDelay);
            this.groupBox3.Controls.Add(this.btnTestNG);
            this.groupBox3.Controls.Add(this.txtMessageSendNG);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.txtKeySendNG);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(12, 294);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(321, 206);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Option NG";
            // 
            // txtKeySendDescription
            // 
            this.txtKeySendDescription.Location = new System.Drawing.Point(71, 120);
            this.txtKeySendDescription.Name = "txtKeySendDescription";
            this.txtKeySendDescription.Size = new System.Drawing.Size(244, 51);
            this.txtKeySendDescription.TabIndex = 7;
            this.txtKeySendDescription.Text = "";
            this.txtKeySendDescription.TextChanged += new System.EventHandler(this.txtKeySendDescription_TextChanged);
            // 
            // cbAllowNG
            // 
            this.cbAllowNG.AutoSize = true;
            this.cbAllowNG.Checked = true;
            this.cbAllowNG.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAllowNG.ForeColor = System.Drawing.Color.Red;
            this.cbAllowNG.Location = new System.Drawing.Point(9, 20);
            this.cbAllowNG.Name = "cbAllowNG";
            this.cbAllowNG.Size = new System.Drawing.Size(96, 17);
            this.cbAllowNG.TabIndex = 6;
            this.cbAllowNG.Text = "Allow send NG";
            this.cbAllowNG.UseVisualStyleBackColor = true;
            this.cbAllowNG.CheckedChanged += new System.EventHandler(this.cbAllowNG_CheckedChanged);
            // 
            // nKeySendDelay
            // 
            this.nKeySendDelay.Location = new System.Drawing.Point(71, 67);
            this.nKeySendDelay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nKeySendDelay.Name = "nKeySendDelay";
            this.nKeySendDelay.Size = new System.Drawing.Size(244, 20);
            this.nKeySendDelay.TabIndex = 5;
            this.nKeySendDelay.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nKeySendDelay.ValueChanged += new System.EventHandler(this.nInput_ValueChanged);
            // 
            // btnTestNG
            // 
            this.btnTestNG.Location = new System.Drawing.Point(240, 177);
            this.btnTestNG.Name = "btnTestNG";
            this.btnTestNG.Size = new System.Drawing.Size(75, 23);
            this.btnTestNG.TabIndex = 4;
            this.btnTestNG.Text = "Test";
            this.btnTestNG.UseVisualStyleBackColor = true;
            this.btnTestNG.Click += new System.EventHandler(this.btnTestNG_Click);
            // 
            // txtMessageSendNG
            // 
            this.txtMessageSendNG.Location = new System.Drawing.Point(71, 93);
            this.txtMessageSendNG.Name = "txtMessageSendNG";
            this.txtMessageSendNG.Size = new System.Drawing.Size(244, 20);
            this.txtMessageSendNG.TabIndex = 2;
            this.txtMessageSendNG.Text = "test";
            this.txtMessageSendNG.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Delay";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 120);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Description";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 96);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Message";
            // 
            // txtKeySendNG
            // 
            this.txtKeySendNG.Location = new System.Drawing.Point(71, 41);
            this.txtKeySendNG.Name = "txtKeySendNG";
            this.txtKeySendNG.Size = new System.Drawing.Size(244, 20);
            this.txtKeySendNG.TabIndex = 2;
            this.txtKeySendNG.Text = "006D";
            this.txtKeySendNG.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Key";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbType);
            this.groupBox4.Controls.Add(this.nIntervalStart);
            this.groupBox4.Controls.Add(this.nTimeStart);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.nThreshold);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Location = new System.Drawing.Point(349, 20);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(327, 98);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Processing";
            // 
            // cbType
            // 
            this.cbType.FormattingEnabled = true;
            this.cbType.Items.AddRange(new object[] {
            "CLS",
            "DETECT"});
            this.cbType.Location = new System.Drawing.Point(71, 43);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(121, 21);
            this.cbType.TabIndex = 6;
            this.cbType.Visible = false;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // nIntervalStart
            // 
            this.nIntervalStart.Location = new System.Drawing.Point(206, 71);
            this.nIntervalStart.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nIntervalStart.Name = "nIntervalStart";
            this.nIntervalStart.Size = new System.Drawing.Size(109, 20);
            this.nIntervalStart.TabIndex = 5;
            this.nIntervalStart.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nIntervalStart.ValueChanged += new System.EventHandler(this.nInput_ValueChanged);
            // 
            // nTimeStart
            // 
            this.nTimeStart.Location = new System.Drawing.Point(71, 70);
            this.nTimeStart.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nTimeStart.Name = "nTimeStart";
            this.nTimeStart.Size = new System.Drawing.Size(121, 20);
            this.nTimeStart.TabIndex = 5;
            this.nTimeStart.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nTimeStart.ValueChanged += new System.EventHandler(this.nInput_ValueChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(32, 46);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(31, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "Type";
            this.label14.Visible = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 73);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(55, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Time Start";
            // 
            // nThreshold
            // 
            this.nThreshold.Location = new System.Drawing.Point(71, 19);
            this.nThreshold.Name = "nThreshold";
            this.nThreshold.Size = new System.Drawing.Size(244, 20);
            this.nThreshold.TabIndex = 5;
            this.nThreshold.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nThreshold.ValueChanged += new System.EventHandler(this.nInput_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Threshold";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.nDeleteFileAfterDays);
            this.groupBox5.Controls.Add(this.btnResetFilePath);
            this.groupBox5.Controls.Add(this.btnOpen);
            this.groupBox5.Controls.Add(this.btnBrowse);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.txtPath);
            this.groupBox5.Location = new System.Drawing.Point(349, 124);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(327, 154);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "File System";
            // 
            // nDeleteFileAfterDays
            // 
            this.nDeleteFileAfterDays.Location = new System.Drawing.Point(75, 31);
            this.nDeleteFileAfterDays.Name = "nDeleteFileAfterDays";
            this.nDeleteFileAfterDays.Size = new System.Drawing.Size(198, 20);
            this.nDeleteFileAfterDays.TabIndex = 5;
            this.nDeleteFileAfterDays.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nDeleteFileAfterDays.ValueChanged += new System.EventHandler(this.nInput_ValueChanged);
            // 
            // btnResetFilePath
            // 
            this.btnResetFilePath.Location = new System.Drawing.Point(71, 107);
            this.btnResetFilePath.Name = "btnResetFilePath";
            this.btnResetFilePath.Size = new System.Drawing.Size(69, 23);
            this.btnResetFilePath.TabIndex = 4;
            this.btnResetFilePath.Text = "Reset";
            this.btnResetFilePath.UseVisualStyleBackColor = true;
            this.btnResetFilePath.Click += new System.EventHandler(this.btnResetFilePath_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(171, 107);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(69, 23);
            this.btnOpen.TabIndex = 4;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(246, 107);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(69, 23);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(279, 35);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(26, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "Day";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 84);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "File test :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 33);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Delete file";
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(71, 81);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(244, 20);
            this.txtPath.TabIndex = 2;
            this.txtPath.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cbByPass);
            this.groupBox6.Controls.Add(this.cbRectangle);
            this.groupBox6.Location = new System.Drawing.Point(349, 294);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(327, 206);
            this.groupBox6.TabIndex = 10;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Other";
            // 
            // cbByPass
            // 
            this.cbByPass.AutoSize = true;
            this.cbByPass.ForeColor = System.Drawing.Color.Red;
            this.cbByPass.Location = new System.Drawing.Point(17, 177);
            this.cbByPass.Name = "cbByPass";
            this.cbByPass.Size = new System.Drawing.Size(71, 17);
            this.cbByPass.TabIndex = 0;
            this.cbByPass.Text = "BY PASS";
            this.cbByPass.UseVisualStyleBackColor = true;
            this.cbByPass.Visible = false;
            this.cbByPass.CheckedChanged += new System.EventHandler(this.cbRectangle_CheckedChanged);
            // 
            // cbRectangle
            // 
            this.cbRectangle.AutoSize = true;
            this.cbRectangle.Location = new System.Drawing.Point(18, 30);
            this.cbRectangle.Name = "cbRectangle";
            this.cbRectangle.Size = new System.Drawing.Size(105, 17);
            this.cbRectangle.TabIndex = 0;
            this.cbRectangle.Text = "Show Rectangle";
            this.cbRectangle.UseVisualStyleBackColor = true;
            this.cbRectangle.CheckedChanged += new System.EventHandler(this.cbRectangle_CheckedChanged);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.factoryResetToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(142, 26);
            // 
            // factoryResetToolStripMenuItem
            // 
            this.factoryResetToolStripMenuItem.Name = "factoryResetToolStripMenuItem";
            this.factoryResetToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.factoryResetToolStripMenuItem.Text = "Factory reset";
            this.factoryResetToolStripMenuItem.Click += new System.EventHandler(this.factoryResetToolStripMenuItem_Click);
            // 
            // taskDialog1
            // 
            this.taskDialog1.Buttons.Add(this._taskDialogButtonOK);
            this.taskDialog1.Buttons.Add(this._taskDialogButtonCancel);
            this.taskDialog1.MainInstruction = "taskDialog1";
            // 
            // _taskDialogButtonOK
            // 
            this._taskDialogButtonOK.ButtonType = Ookii.Dialogs.WinForms.ButtonType.Ok;
            this._taskDialogButtonOK.Text = "taskDialogButton1OK";
            // 
            // _taskDialogButtonCancel
            // 
            this._taskDialogButtonCancel.ButtonType = Ookii.Dialogs.WinForms.ButtonType.Cancel;
            this._taskDialogButtonCancel.Text = "taskDialogButton2";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 200;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(694, 545);
            this.ContextMenuStrip = this.contextMenuStrip;
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(710, 584);
            this.Name = "Preferences";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nClearDelay)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nKeySendDelay)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nIntervalStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nTimeStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nThreshold)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nDeleteFileAfterDays)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnTestNetwork;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown nClearDelay;
        private System.Windows.Forms.Button btnMesTest;
        private System.Windows.Forms.TextBox txtClearMessage2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtClearMessage1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox txtKeySendDescription;
        private System.Windows.Forms.CheckBox cbAllowNG;
        private System.Windows.Forms.NumericUpDown nKeySendDelay;
        private System.Windows.Forms.Button btnTestNG;
        private System.Windows.Forms.TextBox txtMessageSendNG;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtKeySendNG;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown nThreshold;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.NumericUpDown nDeleteFileAfterDays;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnResetFilePath;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem factoryResetToolStripMenuItem;
        private Ookii.Dialogs.WinForms.TaskDialog taskDialog1;
        private Ookii.Dialogs.WinForms.TaskDialogButton _taskDialogButtonOK;
        private Ookii.Dialogs.WinForms.TaskDialogButton _taskDialogButtonCancel;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.NumericUpDown nTimeStart;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.CheckBox cbRectangle;
        private System.Windows.Forms.NumericUpDown nIntervalStart;
        private System.Windows.Forms.CheckBox cbByPass;
        private System.Windows.Forms.TextBox txtURL2;
        private System.Windows.Forms.Label label15;
    }
}