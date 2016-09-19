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
            this.buttonTriggerTtl = new System.Windows.Forms.Button();
            this.buttonTriggerCommand = new System.Windows.Forms.Button();
            this.buttonSendScpiTrigger = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblSampleSize
            // 
            this.lblSampleSize.AutoSize = true;
            this.lblSampleSize.Location = new System.Drawing.Point(24, 29);
            this.lblSampleSize.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblSampleSize.Name = "lblSampleSize";
            this.lblSampleSize.Size = new System.Drawing.Size(138, 25);
            this.lblSampleSize.TabIndex = 0;
            this.lblSampleSize.Text = "Sample Size:";
            // 
            // txtSampleSize
            // 
            this.txtSampleSize.Location = new System.Drawing.Point(172, 23);
            this.txtSampleSize.Margin = new System.Windows.Forms.Padding(6);
            this.txtSampleSize.MaxLength = 6;
            this.txtSampleSize.Name = "txtSampleSize";
            this.txtSampleSize.Size = new System.Drawing.Size(160, 31);
            this.txtSampleSize.TabIndex = 1;
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(410, 23);
            this.btnGet.Margin = new System.Windows.Forms.Padding(6);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(150, 44);
            this.btnGet.TabIndex = 2;
            this.btnGet.Text = "DS:Get?";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.ButtonGet_OnClick);
            // 
            // lblResponse
            // 
            this.lblResponse.AutoSize = true;
            this.lblResponse.Location = new System.Drawing.Point(24, 83);
            this.lblResponse.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblResponse.Name = "lblResponse";
            this.lblResponse.Size = new System.Drawing.Size(115, 25);
            this.lblResponse.TabIndex = 3;
            this.lblResponse.Text = "Response:";
            // 
            // rtbResponse
            // 
            this.rtbResponse.BackColor = System.Drawing.SystemColors.Control;
            this.rtbResponse.Location = new System.Drawing.Point(172, 83);
            this.rtbResponse.Margin = new System.Windows.Forms.Padding(6);
            this.rtbResponse.Name = "rtbResponse";
            this.rtbResponse.ReadOnly = true;
            this.rtbResponse.Size = new System.Drawing.Size(664, 355);
            this.rtbResponse.TabIndex = 8;
            this.rtbResponse.TabStop = false;
            this.rtbResponse.Text = "";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(24, 452);
            this.lblTime.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(65, 25);
            this.lblTime.TabIndex = 9;
            this.lblTime.Text = "Time:";
            // 
            // txtTime
            // 
            this.txtTime.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtTime.Location = new System.Drawing.Point(172, 450);
            this.txtTime.Margin = new System.Windows.Forms.Padding(6);
            this.txtTime.Name = "txtTime";
            this.txtTime.ReadOnly = true;
            this.txtTime.Size = new System.Drawing.Size(664, 31);
            this.txtTime.TabIndex = 16;
            this.txtTime.TabStop = false;
            // 
            // buttonContinuous
            // 
            this.buttonContinuous.Location = new System.Drawing.Point(867, 23);
            this.buttonContinuous.Margin = new System.Windows.Forms.Padding(6);
            this.buttonContinuous.Name = "buttonContinuous";
            this.buttonContinuous.Size = new System.Drawing.Size(150, 44);
            this.buttonContinuous.TabIndex = 17;
            this.buttonContinuous.Text = "Continuous DS";
            this.buttonContinuous.UseVisualStyleBackColor = true;
            this.buttonContinuous.Click += new System.EventHandler(this.buttonContinuous_Click);
            // 
            // buttonAbort
            // 
            this.buttonAbort.Enabled = false;
            this.buttonAbort.Location = new System.Drawing.Point(867, 441);
            this.buttonAbort.Margin = new System.Windows.Forms.Padding(6);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(150, 44);
            this.buttonAbort.TabIndex = 18;
            this.buttonAbort.Text = "Abort";
            this.buttonAbort.UseVisualStyleBackColor = true;
            this.buttonAbort.Click += new System.EventHandler(this.buttonAbort_Click);
            // 
            // buttonTriggeredSoftkey
            // 
            this.buttonTriggeredSoftkey.Location = new System.Drawing.Point(867, 85);
            this.buttonTriggeredSoftkey.Margin = new System.Windows.Forms.Padding(6);
            this.buttonTriggeredSoftkey.Name = "buttonTriggeredSoftkey";
            this.buttonTriggeredSoftkey.Size = new System.Drawing.Size(150, 71);
            this.buttonTriggeredSoftkey.TabIndex = 19;
            this.buttonTriggeredSoftkey.Text = "Softkey Trigger";
            this.buttonTriggeredSoftkey.UseVisualStyleBackColor = true;
            this.buttonTriggeredSoftkey.Click += new System.EventHandler(this.buttonTriggeredSoftkey_Click);
            // 
            // buttonTriggerTtl
            // 
            this.buttonTriggerTtl.Location = new System.Drawing.Point(867, 174);
            this.buttonTriggerTtl.Margin = new System.Windows.Forms.Padding(6);
            this.buttonTriggerTtl.Name = "buttonTriggerTtl";
            this.buttonTriggerTtl.Size = new System.Drawing.Size(150, 71);
            this.buttonTriggerTtl.TabIndex = 20;
            this.buttonTriggerTtl.Text = "TTL Trigger";
            this.buttonTriggerTtl.UseVisualStyleBackColor = true;
            this.buttonTriggerTtl.Click += new System.EventHandler(this.buttonTriggerTtl_Click);
            // 
            // buttonTriggerCommand
            // 
            this.buttonTriggerCommand.Location = new System.Drawing.Point(867, 263);
            this.buttonTriggerCommand.Margin = new System.Windows.Forms.Padding(6);
            this.buttonTriggerCommand.Name = "buttonTriggerCommand";
            this.buttonTriggerCommand.Size = new System.Drawing.Size(150, 71);
            this.buttonTriggerCommand.TabIndex = 21;
            this.buttonTriggerCommand.Text = "SCPI Trigger";
            this.buttonTriggerCommand.UseVisualStyleBackColor = true;
            this.buttonTriggerCommand.Click += new System.EventHandler(this.buttonTriggerCommand_Click);
            // 
            // buttonSendScpiTrigger
            // 
            this.buttonSendScpiTrigger.Enabled = false;
            this.buttonSendScpiTrigger.Location = new System.Drawing.Point(867, 352);
            this.buttonSendScpiTrigger.Margin = new System.Windows.Forms.Padding(6);
            this.buttonSendScpiTrigger.Name = "buttonSendScpiTrigger";
            this.buttonSendScpiTrigger.Size = new System.Drawing.Size(150, 71);
            this.buttonSendScpiTrigger.TabIndex = 22;
            this.buttonSendScpiTrigger.Text = "Send Scpi Trigger";
            this.buttonSendScpiTrigger.UseVisualStyleBackColor = true;
            this.buttonSendScpiTrigger.Click += new System.EventHandler(this.buttonSendScpiTrigger_Click);
            // 
            // DataStoreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 496);
            this.Controls.Add(this.buttonSendScpiTrigger);
            this.Controls.Add(this.buttonTriggerCommand);
            this.Controls.Add(this.buttonTriggerTtl);
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
            this.Margin = new System.Windows.Forms.Padding(6);
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
        private System.Windows.Forms.Button buttonTriggerTtl;
        private System.Windows.Forms.Button buttonTriggerCommand;
        private System.Windows.Forms.Button buttonSendScpiTrigger;
    }
}

