namespace pol
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            button1 = new Button();
            button2 = new Button();
            btnStartServer = new Button();
            txtMyIP = new TextBox();
            txtTargetIP = new TextBox();
            btnSync = new Button();
            btnShowKey = new Button();
            txtReceiverKey = new TextBox();
            txtSenderKey = new TextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(363, 271);
            button1.Name = "button1";
            button1.Size = new Size(139, 29);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(363, 338);
            button2.Name = "button2";
            button2.Size = new Size(139, 29);
            button2.TabIndex = 1;
            button2.Text = "button2 (read)";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // btnStartServer
            // 
            btnStartServer.Location = new Point(12, 287);
            btnStartServer.Name = "btnStartServer";
            btnStartServer.Size = new Size(94, 29);
            btnStartServer.TabIndex = 2;
            btnStartServer.Text = "Server";
            btnStartServer.UseVisualStyleBackColor = true;
            btnStartServer.Click += btnStartServer_Click;
            // 
            // txtMyIP
            // 
            txtMyIP.Location = new Point(12, 45);
            txtMyIP.Name = "txtMyIP";
            txtMyIP.Size = new Size(125, 27);
            txtMyIP.TabIndex = 3;
            txtMyIP.Text = "My IP";
            // 
            // txtTargetIP
            // 
            txtTargetIP.Location = new Point(12, 107);
            txtTargetIP.Name = "txtTargetIP";
            txtTargetIP.Size = new Size(125, 27);
            txtTargetIP.TabIndex = 4;
            txtTargetIP.Text = "Target";
            // 
            // btnSync
            // 
            btnSync.Location = new Point(139, 287);
            btnSync.Name = "btnSync";
            btnSync.Size = new Size(94, 29);
            btnSync.TabIndex = 5;
            btnSync.Text = "Sync";
            btnSync.UseVisualStyleBackColor = true;
            btnSync.Click += btnSync_Click;
            // 
            // btnShowKey
            // 
            btnShowKey.Location = new Point(618, 338);
            btnShowKey.Name = "btnShowKey";
            btnShowKey.Size = new Size(94, 29);
            btnShowKey.TabIndex = 6;
            btnShowKey.Text = "Show Key";
            btnShowKey.UseVisualStyleBackColor = true;
            btnShowKey.Click += btnShowKey_Click;
            // 
            // txtReceiverKey
            // 
            txtReceiverKey.Location = new Point(261, 45);
            txtReceiverKey.Name = "txtReceiverKey";
            txtReceiverKey.Size = new Size(599, 27);
            txtReceiverKey.TabIndex = 7;
            txtReceiverKey.Text = "txtReceiverKey";
            // 
            // txtSenderKey
            // 
            txtSenderKey.Location = new Point(261, 107);
            txtSenderKey.Name = "txtSenderKey";
            txtSenderKey.Size = new Size(599, 27);
            txtSenderKey.TabIndex = 8;
            txtSenderKey.Text = "txtSenderKey";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(970, 478);
            Controls.Add(txtSenderKey);
            Controls.Add(txtReceiverKey);
            Controls.Add(btnShowKey);
            Controls.Add(btnSync);
            Controls.Add(txtTargetIP);
            Controls.Add(txtMyIP);
            Controls.Add(btnStartServer);
            Controls.Add(button2);
            Controls.Add(button1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button btnStartServer;
        private TextBox txtMyIP;
        private TextBox txtTargetIP;
        private Button btnSync;
        private Button btnShowKey;
        private TextBox txtReceiverKey;
        private TextBox txtSenderKey;
    }
}
