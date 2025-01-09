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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preferences));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTestNetwork = new System.Windows.Forms.Button();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.nClearDelay = new System.Windows.Forms.NumericUpDown();
            this.btnMesTest = new System.Windows.Forms.Button();
            this.txtMesTest = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtClearMessage1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtKeySendDescription = new System.Windows.Forms.RichTextBox();
            this.cbIsAllowNG = new System.Windows.Forms.CheckBox();
            this.nKeySendDelay = new System.Windows.Forms.NumericUpDown();
            this.btnTestNG = new System.Windows.Forms.Button();
            this.txtMesMessagetest = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtKeySendNG = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.nThreshold = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.nDay = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.button5 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nClearDelay)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nKeySendDelay)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nThreshold)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nDay)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnTestNetwork);
            this.groupBox1.Controls.Add(this.txtURL);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(321, 92);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Network";
            // 
            // btnTestNetwork
            // 
            this.btnTestNetwork.Location = new System.Drawing.Point(240, 63);
            this.btnTestNetwork.Name = "btnTestNetwork";
            this.btnTestNetwork.Size = new System.Drawing.Size(75, 23);
            this.btnTestNetwork.TabIndex = 4;
            this.btnTestNetwork.Text = "Test";
            this.btnTestNetwork.UseVisualStyleBackColor = true;
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(42, 37);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(273, 20);
            this.txtURL.TabIndex = 2;
            this.txtURL.Text = "http://127.0.0.1:10010";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 40);
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
            this.groupBox2.Controls.Add(this.txtMesTest);
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
            // 
            // btnMesTest
            // 
            this.btnMesTest.Location = new System.Drawing.Point(240, 128);
            this.btnMesTest.Name = "btnMesTest";
            this.btnMesTest.Size = new System.Drawing.Size(75, 23);
            this.btnMesTest.TabIndex = 4;
            this.btnMesTest.Text = "Test";
            this.btnMesTest.UseVisualStyleBackColor = true;
            // 
            // txtMesTest
            // 
            this.txtMesTest.Location = new System.Drawing.Point(71, 93);
            this.txtMesTest.Name = "txtMesTest";
            this.txtMesTest.Size = new System.Drawing.Size(244, 20);
            this.txtMesTest.TabIndex = 2;
            this.txtMesTest.Text = "test";
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
            this.groupBox3.Controls.Add(this.cbIsAllowNG);
            this.groupBox3.Controls.Add(this.nKeySendDelay);
            this.groupBox3.Controls.Add(this.btnTestNG);
            this.groupBox3.Controls.Add(this.txtMesMessagetest);
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
            // 
            // cbIsAllowNG
            // 
            this.cbIsAllowNG.AutoSize = true;
            this.cbIsAllowNG.Checked = true;
            this.cbIsAllowNG.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIsAllowNG.ForeColor = System.Drawing.Color.Red;
            this.cbIsAllowNG.Location = new System.Drawing.Point(9, 20);
            this.cbIsAllowNG.Name = "cbIsAllowNG";
            this.cbIsAllowNG.Size = new System.Drawing.Size(96, 17);
            this.cbIsAllowNG.TabIndex = 6;
            this.cbIsAllowNG.Text = "Allow send NG";
            this.cbIsAllowNG.UseVisualStyleBackColor = true;
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
            // 
            // btnTestNG
            // 
            this.btnTestNG.Location = new System.Drawing.Point(240, 177);
            this.btnTestNG.Name = "btnTestNG";
            this.btnTestNG.Size = new System.Drawing.Size(75, 23);
            this.btnTestNG.TabIndex = 4;
            this.btnTestNG.Text = "Test";
            this.btnTestNG.UseVisualStyleBackColor = true;
            // 
            // txtMesMessagetest
            // 
            this.txtMesMessagetest.Location = new System.Drawing.Point(71, 93);
            this.txtMesMessagetest.Name = "txtMesMessagetest";
            this.txtMesMessagetest.Size = new System.Drawing.Size(244, 20);
            this.txtMesMessagetest.TabIndex = 2;
            this.txtMesMessagetest.Text = "test";
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
            this.groupBox4.Controls.Add(this.nThreshold);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Location = new System.Drawing.Point(349, 20);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(327, 78);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Processing";
            // 
            // nThreshold
            // 
            this.nThreshold.Location = new System.Drawing.Point(71, 31);
            this.nThreshold.Name = "nThreshold";
            this.nThreshold.Size = new System.Drawing.Size(244, 20);
            this.nThreshold.TabIndex = 5;
            this.nThreshold.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 33);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Threshold";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.nDay);
            this.groupBox5.Controls.Add(this.button5);
            this.groupBox5.Controls.Add(this.button2);
            this.groupBox5.Controls.Add(this.button1);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.textBox1);
            this.groupBox5.Location = new System.Drawing.Point(349, 124);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(327, 154);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "File System";
            // 
            // nDay
            // 
            this.nDay.Location = new System.Drawing.Point(75, 31);
            this.nDay.Name = "nDay";
            this.nDay.Size = new System.Drawing.Size(198, 20);
            this.nDay.TabIndex = 5;
            this.nDay.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
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
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(279, 35);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(26, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "Day";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(71, 81);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(244, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "./";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 84);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(51, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Path file :";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(246, 107);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(69, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(171, 107);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(69, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Open";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Location = new System.Drawing.Point(349, 294);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(327, 206);
            this.groupBox6.TabIndex = 10;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "----";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(71, 107);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(69, 23);
            this.button5.TabIndex = 4;
            this.button5.Text = "Reset";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(694, 545);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
            ((System.ComponentModel.ISupportInitialize)(this.nThreshold)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nDay)).EndInit();
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
        private System.Windows.Forms.TextBox txtMesTest;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtClearMessage1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox txtKeySendDescription;
        private System.Windows.Forms.CheckBox cbIsAllowNG;
        private System.Windows.Forms.NumericUpDown nKeySendDelay;
        private System.Windows.Forms.Button btnTestNG;
        private System.Windows.Forms.TextBox txtMesMessagetest;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtKeySendNG;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown nThreshold;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.NumericUpDown nDay;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.GroupBox groupBox6;
    }
}