﻿namespace WebcamStream
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
            this.webCameraControl1 = new WebEye.Controls.WinForms.WebCameraControl.WebCameraControl();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.address = new System.Windows.Forms.TextBox();
            this.connect = new System.Windows.Forms.Button();
            this.port_tcp = new System.Windows.Forms.TextBox();
            this.isServer = new System.Windows.Forms.CheckBox();
            this.output = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.port_udp = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // webCameraControl1
            // 
            this.webCameraControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webCameraControl1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.webCameraControl1.Location = new System.Drawing.Point(570, 37);
            this.webCameraControl1.Margin = new System.Windows.Forms.Padding(2);
            this.webCameraControl1.MaximumSize = new System.Drawing.Size(450, 253);
            this.webCameraControl1.MinimumSize = new System.Drawing.Size(450, 253);
            this.webCameraControl1.Name = "webCameraControl1";
            this.webCameraControl1.Size = new System.Drawing.Size(450, 253);
            this.webCameraControl1.TabIndex = 23;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(899, 9);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 24;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(824, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Use webcam";
            // 
            // address
            // 
            this.address.Location = new System.Drawing.Point(8, 9);
            this.address.Name = "address";
            this.address.Size = new System.Drawing.Size(76, 20);
            this.address.TabIndex = 28;
            this.address.Text = "127.0.0.1";
            // 
            // connect
            // 
            this.connect.Location = new System.Drawing.Point(349, 8);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(75, 23);
            this.connect.TabIndex = 27;
            this.connect.Text = "Start";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.connect_Click);
            // 
            // port_tcp
            // 
            this.port_tcp.Location = new System.Drawing.Point(98, 9);
            this.port_tcp.Name = "port_tcp";
            this.port_tcp.Size = new System.Drawing.Size(76, 20);
            this.port_tcp.TabIndex = 26;
            this.port_tcp.Text = "8080";
            // 
            // isServer
            // 
            this.isServer.AutoSize = true;
            this.isServer.Location = new System.Drawing.Point(278, 11);
            this.isServer.Name = "isServer";
            this.isServer.Size = new System.Drawing.Size(57, 17);
            this.isServer.TabIndex = 30;
            this.isServer.Text = "Server";
            this.isServer.UseVisualStyleBackColor = true;
            // 
            // output
            // 
            this.output.Location = new System.Drawing.Point(8, 37);
            this.output.Multiline = true;
            this.output.Name = "output";
            this.output.ReadOnly = true;
            this.output.Size = new System.Drawing.Size(557, 643);
            this.output.TabIndex = 31;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(8, 687);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(476, 20);
            this.textBox1.TabIndex = 32;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(490, 685);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 33;
            this.button1.Text = "Send";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pictureBox1.Location = new System.Drawing.Point(570, 295);
            this.pictureBox1.MaximumSize = new System.Drawing.Size(450, 253);
            this.pictureBox1.MinimumSize = new System.Drawing.Size(450, 253);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(450, 253);
            this.pictureBox1.TabIndex = 34;
            this.pictureBox1.TabStop = false;
            // 
            // port_udp
            // 
            this.port_udp.Location = new System.Drawing.Point(188, 9);
            this.port_udp.Name = "port_udp";
            this.port_udp.Size = new System.Drawing.Size(76, 20);
            this.port_udp.TabIndex = 35;
            this.port_udp.Text = "8081";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1027, 718);
            this.Controls.Add(this.port_udp);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.output);
            this.Controls.Add(this.isServer);
            this.Controls.Add(this.address);
            this.Controls.Add(this.connect);
            this.Controls.Add(this.port_tcp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.webCameraControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public WebEye.Controls.WinForms.WebCameraControl.WebCameraControl webCameraControl1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox address;
        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.TextBox port_tcp;
        private System.Windows.Forms.CheckBox isServer;
        private System.Windows.Forms.TextBox output;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox port_udp;
    }
}

