using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace WebcamStream
{
    public partial class Form1 : Form
    {
        public Webcam webcam;
        Client client;
        Server server;
        const int packetSize = 10000;

        ~Form1()
        {
            webcam.Dispose();
            client.Disconnect();
            server.Disconnect();
        }

        public PictureBox getPictureBox()
        {
            return pictureBox1;
        }
        
        public int getPacketSize()
        {
            return packetSize;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            webcam.changeSource();
        }
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                webcam = new Webcam(webCameraControl1, comboBox1, output);
                webcam.loadSources();
            }
            catch( Exception ex )
            {
                output.AppendText("Error loading webcam service " + ex.Message + "\n");
            }

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {

                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress my_ip = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                address.Text = my_ip.ToString();

            }

        }
        
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            webCameraControl1.Dispose();
        }

        private void connect_Click(object sender, EventArgs e)
        {

            String port_udp_text = port_udp.Text;
            int port_udp_int;

            try
            {

                port_udp_int = Int32.Parse(port_udp_text);
                
                if (isServer.Checked)
                {

                    server = new Server(address.Text, port_udp_int, output, this);

                }
                else
                {

                    client = new Client(address.Text, port_udp_int, output, this);

                }

            }
            catch {
                output.AppendText("Error while parsing connection configuration \n");
                return;
            }
            
        }

        public Bitmap getImage()
        {
            return webcam.GetCurrentImage();
        }

        public Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(sourceBMP, 0, 0, width, height);
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {


            sendChat();   

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

            if((e.KeyCode & Keys.Enter) == Keys.Enter)
            {
                sendChat();
            }

        }

        private void sendChat()
        {
            if (isServer.Checked && server != null && server.getConnected())
            {
                server.sendText(textBox1.Text);
                output.AppendText("Server: " + textBox1.Text);
                output.AppendText("\n");
                textBox1.Text = "";
            }
            else if (!isServer.Checked && client != null && client.getConnected())
            {
                client.sendText(textBox1.Text);
                output.AppendText("Server: " + textBox1.Text);
                output.AppendText("\n");
                textBox1.Text = "";
            }
            else
            {
                output.AppendText("Before send check failed: most likely not connected");
            }
        }
    }
}
