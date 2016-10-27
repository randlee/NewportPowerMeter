namespace Visyn.Newport
{
    partial class MainForm
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
            this.rtbResponse = new System.Windows.Forms.RichTextBox();
            this.btnRead = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblResponse = new System.Windows.Forms.Label();
            this.lblCmd = new System.Windows.Forms.Label();
            this.txtCmd = new System.Windows.Forms.TextBox();
            this.btnConnectUsb = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.buttonConnectSerial = new System.Windows.Forms.Button();
            this.buttonQuery = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbResponse
            // 
            this.rtbResponse.BackColor = System.Drawing.SystemColors.Control;
            this.rtbResponse.Location = new System.Drawing.Point(85, 81);
            this.rtbResponse.Name = "rtbResponse";
            this.rtbResponse.ReadOnly = true;
            this.rtbResponse.Size = new System.Drawing.Size(283, 153);
            this.rtbResponse.TabIndex = 7;
            this.rtbResponse.TabStop = false;
            this.rtbResponse.Text = "";
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(379, 107);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(90, 23);
            this.btnRead.TabIndex = 5;
            this.btnRead.Text = "&Read";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.OnRead);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(379, 52);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(90, 23);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "&Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.OnSend);
            // 
            // lblResponse
            // 
            this.lblResponse.AutoSize = true;
            this.lblResponse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResponse.Location = new System.Drawing.Point(12, 80);
            this.lblResponse.Name = "lblResponse";
            this.lblResponse.Size = new System.Drawing.Size(67, 13);
            this.lblResponse.TabIndex = 6;
            this.lblResponse.Text = "Response:";
            // 
            // lblCmd
            // 
            this.lblCmd.AutoSize = true;
            this.lblCmd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCmd.Location = new System.Drawing.Point(12, 55);
            this.lblCmd.Name = "lblCmd";
            this.lblCmd.Size = new System.Drawing.Size(65, 13);
            this.lblCmd.TabIndex = 2;
            this.lblCmd.Text = "&Command:";
            // 
            // txtCmd
            // 
            this.txtCmd.Location = new System.Drawing.Point(85, 52);
            this.txtCmd.MaxLength = 63;
            this.txtCmd.Name = "txtCmd";
            this.txtCmd.Size = new System.Drawing.Size(283, 20);
            this.txtCmd.TabIndex = 3;
            // 
            // btnConnectUsb
            // 
            this.btnConnectUsb.Location = new System.Drawing.Point(12, 12);
            this.btnConnectUsb.Name = "btnConnectUsb";
            this.btnConnectUsb.Size = new System.Drawing.Size(90, 23);
            this.btnConnectUsb.TabIndex = 0;
            this.btnConnectUsb.Text = "&Connect USB";
            this.btnConnectUsb.UseVisualStyleBackColor = true;
            this.btnConnectUsb.Click += new System.EventHandler(this.OnConnect);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(204, 12);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(90, 23);
            this.btnDisconnect.TabIndex = 1;
            this.btnDisconnect.Text = "&Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.OnDisconnect);
            // 
            // buttonConnectSerial
            // 
            this.buttonConnectSerial.Location = new System.Drawing.Point(108, 12);
            this.buttonConnectSerial.Name = "buttonConnectSerial";
            this.buttonConnectSerial.Size = new System.Drawing.Size(90, 23);
            this.buttonConnectSerial.TabIndex = 8;
            this.buttonConnectSerial.Text = "&Connect Serial";
            this.buttonConnectSerial.UseVisualStyleBackColor = true;
            this.buttonConnectSerial.Click += new System.EventHandler(this.buttonConnectSerial_Click);
            // 
            // buttonQuery
            // 
            this.buttonQuery.Location = new System.Drawing.Point(379, 79);
            this.buttonQuery.Name = "buttonQuery";
            this.buttonQuery.Size = new System.Drawing.Size(90, 23);
            this.buttonQuery.TabIndex = 9;
            this.buttonQuery.Text = "&Query";
            this.buttonQuery.UseVisualStyleBackColor = true;
            this.buttonQuery.Click += new System.EventHandler(this.OnQuery);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 246);
            this.Controls.Add(this.buttonQuery);
            this.Controls.Add(this.buttonConnectSerial);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnConnectUsb);
            this.Controls.Add(this.rtbResponse);
            this.Controls.Add(this.btnRead);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.lblResponse);
            this.Controls.Add(this.lblCmd);
            this.Controls.Add(this.txtCmd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CSharp Sample Application";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbResponse;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label lblResponse;
        private System.Windows.Forms.Label lblCmd;
        private System.Windows.Forms.TextBox txtCmd;
        private System.Windows.Forms.Button btnConnectUsb;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button buttonConnectSerial;
        private System.Windows.Forms.Button buttonQuery;
    }
}

