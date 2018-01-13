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
    class Server
    {

        UdpClient udpServer;
        Thread t1;
        Thread t2;
        bool continue_threads = false;
        bool connected = false;

        IPAddress ipAd;
        TcpListener myList;
        Socket s;

        string address;
        int port;
        TextBox output;
        Form1 form;

        public bool getConnected()
        {
            return connected;
        }

        public Server(string address, int port, TextBox output, Form1 form)
        {

            try
            {
                Disconnect();

                this.address = address;
                this.port = port;
                this.output = output;
                this.form = form;

                // UDP
                output.AppendText("Inititializing UDP connection on port " + port.ToString() + "... \n");
                udpServer = new UdpClient(port);

                continue_threads = true;

                // TCP

                ipAd = IPAddress.Parse(address);
                myList = new TcpListener(ipAd, port);
                myList.Start();

                output.AppendText("\nThe server is running at port " + port + "... \n");
                output.AppendText("\nThe local End point is: " + myList.LocalEndpoint + "\n");
                output.AppendText("\nWaiting for a connection..... \n ");

                s = myList.AcceptSocket();

                output.AppendText("\nConnection accepted from " + s.RemoteEndPoint + "\n");

                connected = true;

                // UDP thread
                t1 = new Thread(() =>
                {

                    while(continue_threads)
                    {

                        var remoteEP = new IPEndPoint(IPAddress.Any, 11000);
                        var data = udpServer.Receive(ref remoteEP); // listen on port 11000
                        Bitmap image;

                        using (var ms = new MemoryStream(data))
                        {
                            image = new Bitmap(ms);
                        }

                        form.getPictureBox().Image = form.ResizeBitmap( image, 450, 253);

                        //output.AppendText("receive data from " + remoteEP.ToString() + "\n");

                        byte[] bytes;
                        image = form.getImage();

                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, ImageFormat.Jpeg);
                            bytes = ms.ToArray();
                        }

                        udpServer.Send(bytes, bytes.Length, remoteEP); // reply back

                    }

                    if(!continue_threads)
                    {
                        output.AppendText("UDP processing thread stopped \n");
                    }

                });
                t1.Start();

                // TCP thread
                t2 = new Thread(() =>
                {

                    while (continue_threads)
                    {

                        byte[] text = new byte[form.getPacketSize()];
                        s.Receive(text);

                        String rcv = "";
                        for (int i = 0; i < text.Length; i++)
                            rcv += Convert.ToChar(text[i]);

                        output.AppendText("Client: " + rcv);
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
            }

        }

        public void sendText(string text)
        {

            if (text == "") return;
            ASCIIEncoding asen = new ASCIIEncoding();

            int packetSize = form.getPacketSize();
            byte[] bytes = asen.GetBytes(text);
            System.Array.Resize(ref bytes, packetSize);

            s.Send(bytes, packetSize, SocketFlags.None);

        }

        public void Disconnect()
        {

            continue_threads = false;

            if(t1 != null) t1.Join();
            if(t2 != null) t2.Join();

            if (s != null)
            {
                s.Disconnect(false);
                s.Dispose();
            }

            if(myList != null)
            {
                myList.Stop();
            }
            
            if( udpServer != null )
            {
                udpServer.Dispose();
            }
            
            connected = false;

        }

        ~Server()
        {
            Disconnect();
        }

    }
}
