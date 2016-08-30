namespace DataStoreSample
{
    partial class frmMain
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
            this.lblSampleSize = new System.Windows.Forms.Label ();
            this.txtSampleSize = new System.Windows.Forms.TextBox ();
            this.btnGet = new System.Windows.Forms.Button ();
            this.lblResponse = new System.Windows.Forms.Label ();
            this.rtbResponse = new System.Windows.Forms.RichTextBox ();
            this.lblTime = new System.Windows.Forms.Label ();
            this.txtTime = new System.Windows.Forms.TextBox ();
            this.SuspendLayout ();
            // 
            // lblSampleSize
            // 
            this.lblSampleSize.AutoSize = true;
            this.lblSampleSize.Location = new System.Drawing.Point (12, 15);
            this.lblSampleSize.Name = "lblSampleSize";
            this.lblSampleSize.Size = new System.Drawing.Size (68, 13);
            this.lblSampleSize.TabIndex = 0;
            this.lblSampleSize.Text = "Sample Size:";
            // 
            // txtSampleSize
            // 
            this.txtSampleSize.Location = new System.Drawing.Point (86, 12);
            this.txtSampleSize.MaxLength = 6;
            this.txtSampleSize.Name = "txtSampleSize";
            this.txtSampleSize.Size = new System.Drawing.Size (82, 20);
            this.txtSampleSize.TabIndex = 1;
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point (205, 12);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size (75, 23);
            this.btnGet.TabIndex = 2;
            this.btnGet.Text = "DS:Get?";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler (this.OnClick_btnGet);
            // 
            // lblResponse
            // 
            this.lblResponse.AutoSize = true;
            this.lblResponse.Location = new System.Drawing.Point (12, 43);
            this.lblResponse.Name = "lblResponse";
            this.lblResponse.Size = new System.Drawing.Size (58, 13);
            this.lblResponse.TabIndex = 3;
            this.lblResponse.Text = "Response:";
            // 
            // rtbResponse
            // 
            this.rtbResponse.BackColor = System.Drawing.SystemColors.Control;
            this.rtbResponse.Location = new System.Drawing.Point (86, 43);
            this.rtbResponse.Name = "rtbResponse";
            this.rtbResponse.ReadOnly = true;
            this.rtbResponse.Size = new System.Drawing.Size (194, 40);
            this.rtbResponse.TabIndex = 8;
            this.rtbResponse.TabStop = false;
            this.rtbResponse.Text = "";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point (12, 90);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size (33, 13);
            this.lblTime.TabIndex = 9;
            this.lblTime.Text = "Time:";
            // 
            // txtTime
            // 
            this.txtTime.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtTime.Location = new System.Drawing.Point (86, 89);
            this.txtTime.Name = "txtTime";
            this.txtTime.ReadOnly = true;
            this.txtTime.Size = new System.Drawing.Size (194, 20);
            this.txtTime.TabIndex = 16;
            this.txtTime.TabStop = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size (292, 121);
            this.Controls.Add (this.txtTime);
            this.Controls.Add (this.lblTime);
            this.Controls.Add (this.rtbResponse);
            this.Controls.Add (this.lblResponse);
            this.Controls.Add (this.btnGet);
            this.Controls.Add (this.txtSampleSize);
            this.Controls.Add (this.lblSampleSize);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Store Sample Application";
            this.ResumeLayout (false);
            this.PerformLayout ();

        }

        #endregion

        private System.Windows.Forms.Label lblSampleSize;
        private System.Windows.Forms.TextBox txtSampleSize;
        private System.Windows.Forms.Button btnGet;
        private System.Windows.Forms.Label lblResponse;
        private System.Windows.Forms.RichTextBox rtbResponse;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.TextBox txtTime;
    }
}

