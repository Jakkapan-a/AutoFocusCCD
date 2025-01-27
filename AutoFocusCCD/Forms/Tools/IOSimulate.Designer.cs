namespace AutoFocusCCD.Forms.Tools
{
    partial class IOSimulate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IOSimulate));
            this.txtTextData = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.btnSend = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnLEDOff = new System.Windows.Forms.Button();
            this.btnLEDBlue = new System.Windows.Forms.Button();
            this.btnLEDGreen = new System.Windows.Forms.Button();
            this.btnLEDRed = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnROff = new System.Windows.Forms.Button();
            this.btnRPVM = new System.Windows.Forms.Button();
            this.btnRNOT = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtTextData
            // 
            this.txtTextData.Location = new System.Drawing.Point(12, 103);
            this.txtTextData.MaxLength = 256;
            this.txtTextData.Name = "txtTextData";
            this.txtTextData.Size = new System.Drawing.Size(339, 20);
            this.txtTextData.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 352);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(438, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(357, 103);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Text:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label2.Location = new System.Drawing.Point(121, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 56);
            this.label2.TabIndex = 5;
            this.label2.Text = "I/O Simulate";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnLEDOff);
            this.groupBox1.Controls.Add(this.btnLEDBlue);
            this.groupBox1.Controls.Add(this.btnLEDGreen);
            this.groupBox1.Controls.Add(this.btnLEDRed);
            this.groupBox1.Location = new System.Drawing.Point(15, 138);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(411, 86);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "LED";
            // 
            // btnLEDOff
            // 
            this.btnLEDOff.Location = new System.Drawing.Point(261, 30);
            this.btnLEDOff.Name = "btnLEDOff";
            this.btnLEDOff.Size = new System.Drawing.Size(75, 36);
            this.btnLEDOff.TabIndex = 0;
            this.btnLEDOff.Text = "OFF";
            this.btnLEDOff.UseVisualStyleBackColor = true;
            this.btnLEDOff.Click += new System.EventHandler(this.btnLED_Click);
            // 
            // btnLEDBlue
            // 
            this.btnLEDBlue.Location = new System.Drawing.Point(181, 30);
            this.btnLEDBlue.Name = "btnLEDBlue";
            this.btnLEDBlue.Size = new System.Drawing.Size(75, 36);
            this.btnLEDBlue.TabIndex = 0;
            this.btnLEDBlue.Text = "BLUE";
            this.btnLEDBlue.UseVisualStyleBackColor = true;
            this.btnLEDBlue.Click += new System.EventHandler(this.btnLED_Click);
            // 
            // btnLEDGreen
            // 
            this.btnLEDGreen.Location = new System.Drawing.Point(100, 30);
            this.btnLEDGreen.Name = "btnLEDGreen";
            this.btnLEDGreen.Size = new System.Drawing.Size(75, 36);
            this.btnLEDGreen.TabIndex = 0;
            this.btnLEDGreen.Text = "GREEN";
            this.btnLEDGreen.UseVisualStyleBackColor = true;
            this.btnLEDGreen.Click += new System.EventHandler(this.btnLED_Click);
            // 
            // btnLEDRed
            // 
            this.btnLEDRed.Location = new System.Drawing.Point(19, 30);
            this.btnLEDRed.Name = "btnLEDRed";
            this.btnLEDRed.Size = new System.Drawing.Size(75, 36);
            this.btnLEDRed.TabIndex = 0;
            this.btnLEDRed.Text = "RED";
            this.btnLEDRed.UseVisualStyleBackColor = true;
            this.btnLEDRed.Click += new System.EventHandler(this.btnLED_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnROff);
            this.groupBox2.Controls.Add(this.btnRPVM);
            this.groupBox2.Controls.Add(this.btnRNOT);
            this.groupBox2.Location = new System.Drawing.Point(12, 230);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(411, 100);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "RELAY PVM";
            // 
            // btnROff
            // 
            this.btnROff.Location = new System.Drawing.Point(184, 40);
            this.btnROff.Name = "btnROff";
            this.btnROff.Size = new System.Drawing.Size(75, 36);
            this.btnROff.TabIndex = 0;
            this.btnROff.Text = "OFF";
            this.btnROff.UseVisualStyleBackColor = true;
            this.btnROff.Click += new System.EventHandler(this.btnR_Click);
            // 
            // btnRPVM
            // 
            this.btnRPVM.Location = new System.Drawing.Point(22, 40);
            this.btnRPVM.Name = "btnRPVM";
            this.btnRPVM.Size = new System.Drawing.Size(75, 36);
            this.btnRPVM.TabIndex = 0;
            this.btnRPVM.Text = "4.6V PVM";
            this.btnRPVM.UseVisualStyleBackColor = true;
            this.btnRPVM.Click += new System.EventHandler(this.btnR_Click);
            // 
            // btnRNOT
            // 
            this.btnRNOT.Location = new System.Drawing.Point(103, 40);
            this.btnRNOT.Name = "btnRNOT";
            this.btnRNOT.Size = new System.Drawing.Size(75, 36);
            this.btnRNOT.TabIndex = 0;
            this.btnRNOT.Text = "6V";
            this.btnRNOT.UseVisualStyleBackColor = true;
            this.btnRNOT.Click += new System.EventHandler(this.btnR_Click);
            // 
            // IOSimulate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 374);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txtTextData);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "IOSimulate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IOSimulate";
            this.Load += new System.EventHandler(this.IOSimulate_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTextData;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnLEDOff;
        private System.Windows.Forms.Button btnLEDBlue;
        private System.Windows.Forms.Button btnLEDGreen;
        private System.Windows.Forms.Button btnLEDRed;
        private System.Windows.Forms.Button btnROff;
        private System.Windows.Forms.Button btnRPVM;
        private System.Windows.Forms.Button btnRNOT;
    }
}