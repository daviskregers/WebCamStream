﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebcamStream
{
    public partial class Form1 : Form
    {
        public Webcam webcam;
        Client client;
        Server server;
        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            webcam.changeSource();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if(isServer.Checked)
            {
                server = new Server(address.Text, port.Text, output, this);

            }
            else
            {
                client = new Client(address.Text, port.Text, output, this);
            }

        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webcam = new Webcam(webCameraControl1, comboBox1); 
            webcam.loadSources();
        }
        
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            webCameraControl1.Dispose();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.SendChat();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {

            if ((e.KeyCode & Keys.Enter) == Keys.Enter)
            {
                this.SendChat();
            }

        }

        private void SendChat()
        {

            if(isServer.Checked)
            {
                server.SendText(textBox1.Text);
                textBox1.Text = "";
            }
            else
            {
                client.SendText(textBox1.Text);
                textBox1.Text = "";
            }
        }

    }
}
