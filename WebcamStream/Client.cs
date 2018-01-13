using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Net.Sockets;

namespace WebcamStream
{
    class Client
    {

        UdpClient udp;
        Thread t1;
        Thread t2;
        bool continue_threads = false;
        bool connected = false;

        string address;
        int port;
        TextBox output;
        Form1 form;
        Stream stm;
        TcpClient tcpclnt;

        public bool getConnected()
        {
            return connected;
        }

        public Client(string address, int port, TextBox output, Form1 form)
        {

            try
            {

                Disconnect();

                this.address = address;
                this.port = port;
                this.output = output;
                this.form = form;

                // UDP Client
                output.AppendText("Inititializing UDP connection on port " + port.ToString() + "... \n");
                udp = new UdpClient();
                continue_threads = true;

                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(address), port); // endpoint where server is listening
                udp.Connect(ep);

                // TCP Client

                tcpclnt = new TcpClient();
                output.AppendText("\nConnecting to " + address + "\n");
                tcpclnt.Connect(address, port);

                stm = tcpclnt.GetStream();

                connected = true;

                // UDP THREAD
                t1 = new Thread(() =>
                {

                    while (continue_threads)
                    {
                        byte[] bytes;
                        Bitmap image = form.getImage();

                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, ImageFormat.Jpeg);
                            bytes = ms.ToArray();
                        }

                        udp.Send(bytes, bytes.Length); // reply back
                        
                        var receivedData = udp.Receive(ref ep);
                        
                        using (var ms = new MemoryStream(receivedData))
                        {
                            image = new Bitmap(ms);
                        }

                        form.getPictureBox().Image = form.ResizeBitmap(image, 450, 253);

                        //output.AppendText("receive data from " + ep.ToString() + "\n");
                    }

                    if (!continue_threads)
                    {
                        output.AppendText("UDP processing thread stopped \n");
                    }

                });
                t1.Start();

                // TCP THREAD

                t2 = new Thread(() =>
                {

                    while (continue_threads)
                    {

                        byte[] text = new byte[form.getPacketSize()];
                        stm.Read(text, 0, form.getPacketSize());

                        String rcv = "";
                        for (int i = 0; i < text.Length; i++)
                            rcv += Convert.ToChar(text[i]);

                        output.AppendText("Server: " + rcv);
                        output.AppendText("\n");
                        
                    }

                    if (!continue_threads)
                    {
                        output.AppendText("UDP processing thread stopped \n");
                    }

                });
                t2.Start();

            }
            catch (Exception e)
            {
                output.AppendText("Error while initializing connection: " + e.Message + "\n");
                connected = false;
            }

        }

        public void sendText(string text)
        {

            if (text == "") return;
            ASCIIEncoding asen = new ASCIIEncoding();

            int packetSize = form.getPacketSize();
            byte[] bytes = asen.GetBytes(text);
            System.Array.Resize(ref bytes, packetSize);

            stm.Write(bytes, 0, packetSize);

        }

        public void Disconnect()
        {
            
            continue_threads = false;
            if (t1 != null) t1.Join();
            if (t2 != null) t2.Join();

            connected = false;

            if (stm != null) stm.Dispose();
            if (tcpclnt != null) tcpclnt.Dispose();
            if (udp != null) udp.Dispose();

            connected = false;

        }

        ~Client()
        {
            Disconnect();
        }

    }
}
