namespace UiPathWinFormExample.net40
{
    partial class Form1
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
            this.cbRobot = new System.Windows.Forms.ComboBox();
            this.cbProcess = new System.Windows.Forms.ComboBox();
            this.bStartJob = new System.Windows.Forms.Button();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.tbSmthType = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cbRobot
            // 
            this.cbRobot.FormattingEnabled = true;
            this.cbRobot.Location = new System.Drawing.Point(12, 12);
            this.cbRobot.Name = "cbRobot";
            this.cbRobot.Size = new System.Drawing.Size(188, 21);
            this.cbRobot.TabIndex = 0;
            // 
            // cbProcess
            // 
            this.cbProcess.FormattingEnabled = true;
            this.cbProcess.Location = new System.Drawing.Point(12, 39);
            this.cbProcess.Name = "cbProcess";
            this.cbProcess.Size = new System.Drawing.Size(188, 21);
            this.cbProcess.TabIndex = 1;
            // 
            // bStartJob
            // 
            this.bStartJob.Location = new System.Drawing.Point(206, 39);
            this.bStartJob.Name = "bStartJob";
            this.bStartJob.Size = new System.Drawing.Size(75, 23);
            this.bStartJob.TabIndex = 2;
            this.bStartJob.Text = "Start job";
            this.bStartJob.UseVisualStyleBackColor = true;
            this.bStartJob.Click += new System.EventHandler(this.bStartJob_Click);
            // 
            // tbOutput
            // 
            this.tbOutput.Location = new System.Drawing.Point(287, 12);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutput.Size = new System.Drawing.Size(501, 48);
            this.tbOutput.TabIndex = 3;
            // 
            // tbSmthType
            // 
            this.tbSmthType.Location = new System.Drawing.Point(12, 68);
            this.tbSmthType.Multiline = true;
            this.tbSmthType.Name = "tbSmthType";
            this.tbSmthType.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbSmthType.Size = new System.Drawing.Size(776, 370);
            this.tbSmthType.TabIndex = 4;
            this.tbSmthType.Text = "Something types while proccess running...";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tbSmthType);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.bStartJob);
            this.Controls.Add(this.cbProcess);
            this.Controls.Add(this.cbRobot);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbRobot;
        private System.Windows.Forms.ComboBox cbProcess;
        private System.Windows.Forms.Button bStartJob;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.TextBox tbSmthType;
    }
}

