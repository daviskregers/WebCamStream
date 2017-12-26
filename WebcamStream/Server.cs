using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace WebcamStream
{
    class Server
    {

        IPAddress ipAd;
        TcpListener myList;
        Socket s;

        string address;
        string port;
        TextBox output;
        Form1 form;
        const int packetSize = 10000;

        class Response
        {
            public Bitmap image;
            public string text;
            public int type;
        }

        public Server(string address, string port, TextBox output, Form1 form)
        {

            this.address = address;
            this.port = port;
            this.output = output;
            this.form = form;

            if (port == "")
            {
                output.AppendText("\nPlease provide the server port \n");
                return;
            }

            if (address == "")
            {
                output.AppendText("\nPlease provide the server IP \n");
                return;
            }

            try
            {

                ipAd = IPAddress.Parse(address);
                myList = new TcpListener(ipAd, Int32.Parse(port));
                myList.Start();

                output.AppendText("\nThe server is running at port " + port + "... \n");
                output.AppendText("\nThe local End point is: " + myList.LocalEndpoint + "\n");
                output.AppendText("\nWaiting for a connection..... \n ");

                s = myList.AcceptSocket();

                s.ReceiveBufferSize = packetSize;
                s.SendBufferSize = packetSize;

                output.AppendText("\nConnection accepted from " + s.RemoteEndPoint + "\n");

                byte[] check = BitConverter.GetBytes((Int32)13371337);
                System.Array.Resize(ref check, packetSize);
                s.Send(check, packetSize, SocketFlags.None);

                ObjectDelegate img = new ObjectDelegate(UpdateImageBox);

                Thread th = new Thread(new ParameterizedThreadStart(WorkThread));

                th.Start(img);


            }
            catch (Exception err)
            {
                output.AppendText("\nAn error occured: " + err.ToString() + "\n");
            }

        }

        private delegate void ObjectDelegate(object obj);

        private void UpdateImageBox(object response)
        {
            if (form.InvokeRequired)
            {
                // slightly different now, as we dont need params
                // we can just use MethodInvoker
                ObjectDelegate method = new ObjectDelegate(UpdateImageBox);
                form.Invoke(method, response);
                return;
            }

            Response res = (Response)response;

            if (res == null) return;

            if (res.type == 1)
            {
                form.updateImage((Bitmap)res.image);
            }
            else if (res.type == 0)
            {
                output.AppendText("\n Client: " + res.text + " \n");
            }

        }

        private void WorkThread(object obj)
        {

            try
            {

                ObjectDelegate img = (ObjectDelegate)obj;

                while (true)
                {

                    if (!s.Connected) continue;

                    this.SendImage();
                    Response image = this.ReceiveImage();

                    if (image != null)
                    {
                        Console.WriteLine("Image not null");
                        img.Invoke(image);
                    }
                    else
                    {
                        Console.WriteLine("Image null");
                    }

                    Thread.Sleep(500);

                }

            }
            catch (Exception e)
            {
                //output.AppendText("Error encountered in server workthread " + e.Message + "\n");

                //byte[] err = BitConverter.GetBytes(-1);
                //s.Send(err);
                Thread.Sleep(500);
                this.WorkThread(null);
            }


        }

        private static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(sourceBMP, 0, 0, width, height);
            return result;
        }

        private void SendImage()
        {

            Bitmap image = form.getImage();
            image = ResizeBitmap(image, 320, 240);
            byte[] bytes;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                bytes = ms.ToArray();
            }

            byte[] type = BitConverter.GetBytes((Int32)1);
            byte[] userDataLen = BitConverter.GetBytes((Int32)bytes.Length);

            Console.WriteLine("WEBCAM LENGTH " + bytes.Length);

            byte[] check = BitConverter.GetBytes((Int32)13371337);

            System.Array.Resize(ref check, packetSize);
            System.Array.Resize(ref bytes, packetSize);
            System.Array.Resize(ref type, packetSize);
            System.Array.Resize(ref userDataLen, packetSize);

            s.Send(check, packetSize, SocketFlags.None);
            s.Send(userDataLen, packetSize, SocketFlags.None);
            s.Send(type, packetSize, SocketFlags.None);
            s.Send(bytes, packetSize, SocketFlags.None);


        }

        public void Disconnect()
        {
            if (s.Connected)
            {
                s.Disconnect(true);
            }
        }

        private Response ReceiveImage()
        {

            Response response = new Response();

            byte[] check = new byte[packetSize];
            s.Receive(check);


            Console.WriteLine("STARTCHECK" + BitConverter.ToInt32(check, 0));
            if (BitConverter.ToInt32(check, 0) != 13371337) return null;

            byte[] b = new byte[packetSize];
            s.Receive(b);

            Console.WriteLine("MSGLEN" + BitConverter.ToInt32(b, 0));
            if (BitConverter.ToInt32(b, 0) == -1) return null;

            byte[] type = new byte[packetSize];
            s.Receive(type);

            Console.WriteLine("TYPE" + BitConverter.ToInt32(b, 0));
            if (BitConverter.ToInt32(type, 0) == -1) return null;

            if (BitConverter.ToInt32(type, 0) == 1)
            {

                Console.WriteLine("IS IMAGE");

                Bitmap image;

                int dataLen = BitConverter.ToInt32(b, 0);

                byte[] img = new byte[packetSize];
                s.Receive(img);

                Console.WriteLine("IMGDATA" + BitConverter.ToInt32(img, 0));
                if (BitConverter.ToInt32(img, 0) == -1) return null;

                using (var ms = new MemoryStream(img))
                {
                    image = new Bitmap(ms);
                }

                response.image = image;
                response.type = 1;

            }
            else if (BitConverter.ToInt32(type, 0) == 0)
            {
                Console.WriteLine("IS TEXT");
                int dataLen = BitConverter.ToInt32(b, 0);

                byte[] text = new byte[packetSize];
                s.Receive(text);

                Console.WriteLine("TEXT" + BitConverter.ToInt32(text, 0));
                if (BitConverter.ToInt32(text, 0) == -1) return null;

                String rcv = "";
                for (int i = 0; i < text.Length; i++)
                    rcv += Convert.ToChar(text[i]);

                response.text = rcv;
                response.type = 0;
            }
            else
            {
                Console.WriteLine("IS ERROR");
                return null;
            }

            return response;

        }

        public void SendText(string text)
        {

            if (text == "") return;
            ASCIIEncoding asen = new ASCIIEncoding();

            byte[] bytes = asen.GetBytes(text);
            System.Array.Resize(ref bytes, packetSize);
            byte[] type = BitConverter.GetBytes((Int32)0);
            System.Array.Resize(ref type, packetSize);
            byte[] userDataLen = BitConverter.GetBytes((Int32)bytes.Length);
            System.Array.Resize(ref userDataLen, packetSize);

            byte[] check = BitConverter.GetBytes((Int32)13371337);

            System.Array.Resize(ref check, packetSize);

            s.Send(check, packetSize, SocketFlags.None);
            s.Send(userDataLen, packetSize, SocketFlags.None);
            s.Send(type, packetSize, SocketFlags.None);
            s.Send(bytes, packetSize, SocketFlags.None);

            output.AppendText("\n Server: " + text + " \n");

        }

        ~Server()
        {
            myList.Stop();
        }

    }
}
