namespace CltFramework
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
            this.components = new System.ComponentModel.Container();
            this.buttonEvntA = new System.Windows.Forms.Button();
            this.timer500ms = new System.Windows.Forms.Timer(this.components);
            this.labelConnectionStatus = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.labelPong = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonEvntA
            // 
            this.buttonEvntA.Location = new System.Drawing.Point(12, 76);
            this.buttonEvntA.Name = "buttonEvntA";
            this.buttonEvntA.Size = new System.Drawing.Size(124, 27);
            this.buttonEvntA.TabIndex = 0;
            this.buttonEvntA.Text = "Send Event A";
            this.buttonEvntA.UseVisualStyleBackColor = true;
            this.buttonEvntA.Click += new System.EventHandler(this.buttonEvntA_Click);
            // 
            // timer500ms
            // 
            this.timer500ms.Enabled = true;
            this.timer500ms.Interval = 500;
            this.timer500ms.Tick += new System.EventHandler(this.timer500ms_Tick);
            // 
            // labelConnectionStatus
            // 
            this.labelConnectionStatus.AutoSize = true;
            this.labelConnectionStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelConnectionStatus.Location = new System.Drawing.Point(190, 23);
            this.labelConnectionStatus.Name = "labelConnectionStatus";
            this.labelConnectionStatus.Size = new System.Drawing.Size(24, 20);
            this.labelConnectionStatus.TabIndex = 2;
            this.labelConnectionStatus.Text = "---";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(12, 20);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 23);
            this.buttonConnect.TabIndex = 3;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Location = new System.Drawing.Point(93, 20);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(75, 23);
            this.buttonDisconnect.TabIndex = 4;
            this.buttonDisconnect.Text = "Disconnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 122);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 27);
            this.button1.TabIndex = 5;
            this.button1.Text = "Send Pong Request";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonReq_Click);
            // 
            // labelPong
            // 
            this.labelPong.AutoSize = true;
            this.labelPong.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.labelPong.Location = new System.Drawing.Point(12, 152);
            this.labelPong.Name = "labelPong";
            this.labelPong.Size = new System.Drawing.Size(19, 15);
            this.labelPong.TabIndex = 6;
            this.labelPong.Text = "---";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 215);
            this.Controls.Add(this.labelPong);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.labelConnectionStatus);
            this.Controls.Add(this.buttonEvntA);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonEvntA;
        private System.Windows.Forms.Timer timer500ms;
        private System.Windows.Forms.Label labelConnectionStatus;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelPong;
    }
}

