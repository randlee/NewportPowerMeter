namespace DataStoreSample
{
    partial class DataStoreForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose ();
            }
            base.Dispose (disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
        {
            this.lblSampleSize = new System.Windows.Forms.Label();
            this.txtSampleSize = new System.Windows.Forms.TextBox();
            this.btnGet = new System.Windows.Forms.Button();
            this.lblResponse = new System.Windows.Forms.Label();
            this.rtbResponse = new System.Windows.Forms.RichTextBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.buttonContinuous = new System.Windows.Forms.Button();
            this.buttonAbort = new System.Windows.Forms.Button();
            this.buttonTriggeredSoftkey = new System.Windows.Forms.Button();
            this.buttonTriggerTtlDuration = new System.Windows.Forms.Button();
            this.buttonTriggerCommand = new System.Windows.Forms.Button();
            this.buttonSendScpiTrigger = new System.Windows.Forms.Button();
            this.buttonTtlTriggerEdge2Edge = new System.Windows.Forms.Button();
            this.textBoxInterval = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSaveLog = new System.Windows.Forms.Button();
            this.buttonConnectSerial = new System.Windows.Forms.Button();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.buttonConnectUsb = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblSampleSize
            // 
            this.lblSampleSize.AutoSize = true;
            this.lblSampleSize.Location = new System.Drawing.Point(12, 52);
            this.lblSampleSize.Name = "lblSampleSize";
            this.lblSampleSize.Size = new System.Drawing.Size(68, 13);
            this.lblSampleSize.TabIndex = 0;
            this.lblSampleSize.Text = "Sample Size:";
            // 
            // txtSampleSize
            // 
            this.txtSampleSize.Location = new System.Drawing.Point(96, 49);
            this.txtSampleSize.MaxLength = 6;
            this.txtSampleSize.Name = "txtSampleSize";
            this.txtSampleSize.Size = new System.Drawing.Size(82, 20);
            this.txtSampleSize.TabIndex = 1;
            this.txtSampleSize.Text = "1000";
            this.txtSampleSize.TextChanged += new System.EventHandler(this.txtSampleSize_TextChanged);
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(332, 47);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(75, 23);
            this.btnGet.TabIndex = 2;
            this.btnGet.Text = "DS:Get?";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.ButtonGet_OnClick);
            // 
            // lblResponse
            // 
            this.lblResponse.AutoSize = true;
            this.lblResponse.Location = new System.Drawing.Point(12, 104);
            this.lblResponse.Name = "lblResponse";
            this.lblResponse.Size = new System.Drawing.Size(58, 13);
            this.lblResponse.TabIndex = 3;
            this.lblResponse.Text = "Response:";
            // 
            // rtbResponse
            // 
            this.rtbResponse.BackColor = System.Drawing.SystemColors.Control;
            this.rtbResponse.Location = new System.Drawing.Point(86, 104);
            this.rtbResponse.Name = "rtbResponse";
            this.rtbResponse.ReadOnly = true;
            this.rtbResponse.Size = new System.Drawing.Size(334, 273);
            this.rtbResponse.TabIndex = 8;
            this.rtbResponse.TabStop = false;
            this.rtbResponse.Text = "";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(7, 398);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(33, 13);
            this.lblTime.TabIndex = 9;
            this.lblTime.Text = "Time:";
            // 
            // txtTime
            // 
            this.txtTime.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtTime.Location = new System.Drawing.Point(86, 397);
            this.txtTime.Name = "txtTime";
            this.txtTime.ReadOnly = true;
            this.txtTime.Size = new System.Drawing.Size(334, 20);
            this.txtTime.TabIndex = 16;
            this.txtTime.TabStop = false;
            // 
            // buttonContinuous
            // 
            this.buttonContinuous.Location = new System.Drawing.Point(434, 12);
            this.buttonContinuous.Name = "buttonContinuous";
            this.buttonContinuous.Size = new System.Drawing.Size(75, 23);
            this.buttonContinuous.TabIndex = 17;
            this.buttonContinuous.Text = "Continuous DS";
            this.buttonContinuous.UseVisualStyleBackColor = true;
            this.buttonContinuous.Click += new System.EventHandler(this.buttonContinuous_Click);
            // 
            // buttonAbort
            // 
            this.buttonAbort.Enabled = false;
            this.buttonAbort.Location = new System.Drawing.Point(434, 268);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(75, 23);
            this.buttonAbort.TabIndex = 18;
            this.buttonAbort.Text = "Abort";
            this.buttonAbort.UseVisualStyleBackColor = true;
            this.buttonAbort.Click += new System.EventHandler(this.buttonAbort_Click);
            // 
            // buttonTriggeredSoftkey
            // 
            this.buttonTriggeredSoftkey.Location = new System.Drawing.Point(434, 44);
            this.buttonTriggeredSoftkey.Name = "buttonTriggeredSoftkey";
            this.buttonTriggeredSoftkey.Size = new System.Drawing.Size(75, 37);
            this.buttonTriggeredSoftkey.TabIndex = 19;
            this.buttonTriggeredSoftkey.Text = "Softkey Trigger";
            this.buttonTriggeredSoftkey.UseVisualStyleBackColor = true;
            this.buttonTriggeredSoftkey.Click += new System.EventHandler(this.buttonTriggeredSoftkey_Click);
            // 
            // buttonTriggerTtlDuration
            // 
            this.buttonTriggerTtlDuration.Location = new System.Drawing.Point(434, 131);
            this.buttonTriggerTtlDuration.Name = "buttonTriggerTtlDuration";
            this.buttonTriggerTtlDuration.Size = new System.Drawing.Size(75, 37);
            this.buttonTriggerTtlDuration.TabIndex = 20;
            this.buttonTriggerTtlDuration.Text = "TTL Trigger (duration)";
            this.buttonTriggerTtlDuration.UseVisualStyleBackColor = true;
            this.buttonTriggerTtlDuration.Click += new System.EventHandler(this.buttonTriggerTtlDuration_Click);
            // 
            // buttonTriggerCommand
            // 
            this.buttonTriggerCommand.Location = new System.Drawing.Point(434, 87);
            this.buttonTriggerCommand.Name = "buttonTriggerCommand";
            this.buttonTriggerCommand.Size = new System.Drawing.Size(75, 37);
            this.buttonTriggerCommand.TabIndex = 21;
            this.buttonTriggerCommand.Text = "SCPI Trigger";
            this.buttonTriggerCommand.UseVisualStyleBackColor = true;
            this.buttonTriggerCommand.Click += new System.EventHandler(this.buttonSCPITriggerCommand_Click);
            // 
            // buttonSendScpiTrigger
            // 
            this.buttonSendScpiTrigger.Enabled = false;
            this.buttonSendScpiTrigger.Location = new System.Drawing.Point(434, 220);
            this.buttonSendScpiTrigger.Name = "buttonSendScpiTrigger";
            this.buttonSendScpiTrigger.Size = new System.Drawing.Size(75, 37);
            this.buttonSendScpiTrigger.TabIndex = 22;
            this.buttonSendScpiTrigger.Text = "Send Scpi Trigger";
            this.buttonSendScpiTrigger.UseVisualStyleBackColor = true;
            this.buttonSendScpiTrigger.Click += new System.EventHandler(this.buttonSendScpiTrigger_Click);
            // 
            // buttonTtlTriggerEdge2Edge
            // 
            this.buttonTtlTriggerEdge2Edge.Location = new System.Drawing.Point(434, 174);
            this.buttonTtlTriggerEdge2Edge.Name = "buttonTtlTriggerEdge2Edge";
            this.buttonTtlTriggerEdge2Edge.Size = new System.Drawing.Size(75, 37);
            this.buttonTtlTriggerEdge2Edge.TabIndex = 23;
            this.buttonTtlTriggerEdge2Edge.Text = "TTL Trigger (edge-edge)";
            this.buttonTtlTriggerEdge2Edge.UseVisualStyleBackColor = true;
            this.buttonTtlTriggerEdge2Edge.Click += new System.EventHandler(this.buttonTtlTriggerEdge2Edge_Click);
            // 
            // textBoxInterval
            // 
            this.textBoxInterval.Location = new System.Drawing.Point(96, 71);
            this.textBoxInterval.MaxLength = 6;
            this.textBoxInterval.Name = "textBoxInterval";
            this.textBoxInterval.Size = new System.Drawing.Size(82, 20);
            this.textBoxInterval.TabIndex = 25;
            this.textBoxInterval.Text = "10";
            this.textBoxInterval.TextChanged += new System.EventHandler(this.textBoxInterval_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Interval:";
            // 
            // buttonSaveLog
            // 
            this.buttonSaveLog.Enabled = false;
            this.buttonSaveLog.Location = new System.Drawing.Point(443, 354);
            this.buttonSaveLog.Name = "buttonSaveLog";
            this.buttonSaveLog.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveLog.TabIndex = 26;
            this.buttonSaveLog.Text = "Save Log";
            this.buttonSaveLog.UseVisualStyleBackColor = true;
            this.buttonSaveLog.Click += new System.EventHandler(this.buttonSaveLog_Click);
            // 
            // buttonConnectSerial
            // 
            this.buttonConnectSerial.Location = new System.Drawing.Point(115, 12);
            this.buttonConnectSerial.Name = "buttonConnectSerial";
            this.buttonConnectSerial.Size = new System.Drawing.Size(90, 23);
            this.buttonConnectSerial.TabIndex = 29;
            this.buttonConnectSerial.Text = "&Connect Serial";
            this.buttonConnectSerial.UseVisualStyleBackColor = true;
            this.buttonConnectSerial.Click += new System.EventHandler(this.buttonConnectSerial_Click);
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Location = new System.Drawing.Point(211, 12);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(90, 23);
            this.buttonDisconnect.TabIndex = 28;
            this.buttonDisconnect.Text = "&Disconnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // buttonConnectUsb
            // 
            this.buttonConnectUsb.Location = new System.Drawing.Point(19, 12);
            this.buttonConnectUsb.Name = "buttonConnectUsb";
            this.buttonConnectUsb.Size = new System.Drawing.Size(90, 23);
            this.buttonConnectUsb.TabIndex = 27;
            this.buttonConnectUsb.Text = "&Connect USB";
            this.buttonConnectUsb.UseVisualStyleBackColor = true;
            this.buttonConnectUsb.Click += new System.EventHandler(this.buttonConnectUsb_Click);
            // 
            // DataStoreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 421);
            this.Controls.Add(this.buttonConnectSerial);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.buttonConnectUsb);
            this.Controls.Add(this.buttonSaveLog);
            this.Controls.Add(this.textBoxInterval);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonTtlTriggerEdge2Edge);
            this.Controls.Add(this.buttonSendScpiTrigger);
            this.Controls.Add(this.buttonTriggerCommand);
            this.Controls.Add(this.buttonTriggerTtlDuration);
            this.Controls.Add(this.buttonTriggeredSoftkey);
            this.Controls.Add(this.buttonAbort);
            this.Controls.Add(this.buttonContinuous);
            this.Controls.Add(this.txtTime);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.rtbResponse);
            this.Controls.Add(this.lblResponse);
            this.Controls.Add(this.btnGet);
            this.Controls.Add(this.txtSampleSize);
            this.Controls.Add(this.lblSampleSize);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "DataStoreForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Store Sample Application";
            this.Load += new System.EventHandler(this.DataStoreForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSampleSize;
        private System.Windows.Forms.TextBox txtSampleSize;
        private System.Windows.Forms.Button btnGet;
        private System.Windows.Forms.Label lblResponse;
        private System.Windows.Forms.RichTextBox rtbResponse;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.Button buttonContinuous;
        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.Button buttonTriggeredSoftkey;
        private System.Windows.Forms.Button buttonTriggerTtlDuration;
        private System.Windows.Forms.Button buttonTriggerCommand;
        private System.Windows.Forms.Button buttonSendScpiTrigger;
        private System.Windows.Forms.Button buttonTtlTriggerEdge2Edge;
        private System.Windows.Forms.TextBox textBoxInterval;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSaveLog;
        private System.Windows.Forms.Button buttonConnectSerial;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.Button buttonConnectUsb;
    }
}

