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
                output.AppendText("Please provide the server port \n");
                return;
            }

            if (address == "")
            {
                output.AppendText("Please provide the server IP \n");
                return;
            }

            try
            {

                ipAd = IPAddress.Parse(address);
                myList = new TcpListener(ipAd, Int32.Parse(port));
                myList.Start();

                output.AppendText("The server is running at port " + port + "... \n");
                output.AppendText("The local End point is: " + myList.LocalEndpoint + "\n");
                output.AppendText("Waiting for a connection..... \n ");

                s = myList.AcceptSocket();
                output.AppendText("Connection accepted from " + s.RemoteEndPoint + "\n");
                
                ObjectDelegate img = new ObjectDelegate(UpdateImageBox);

                Thread th = new Thread(new ParameterizedThreadStart(WorkThread));
                
                th.Start(img);


            }
            catch (Exception err)
            {
                output.AppendText("An error occured: " + err.ToString() + "\n");
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

            if(res.type == 1)
            {
                form.updateImage((Bitmap) res.image);
            }
            else if(res.type == 0) {
                output.AppendText("Client: " + res.text + "\n");
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

                    img.Invoke(image);

                }

            }
            catch (Exception e)
            {
                output.AppendText("Error encountered in server workthread " + e.Message + "\n");

                byte[] err = BitConverter.GetBytes(-1);
                s.Send(err);
                Thread.Sleep(1000);
               
                this.WorkThread(obj);
            }


        }

        private void SendImage()
        {

            Bitmap image = form.getImage();
            byte[] bytes;

            using ( MemoryStream ms = new MemoryStream() )
            {
                image.Save(ms, ImageFormat.Jpeg );
                bytes = ms.ToArray();
            }

            byte[] type = BitConverter.GetBytes((Int32) 1);
            byte[] userDataLen = BitConverter.GetBytes((Int32)bytes.Length);

            s.Send(userDataLen);
            s.Send(type);
            s.Send(bytes);

        }

        public void Disconnect()
        {
            if (s.Connected) { 
                s.Disconnect(true);
            }
        }

        private Response ReceiveImage()
        {

            Response response = new Response();

            byte[] b = new byte[4];
            s.Receive(b);

            if (BitConverter.ToInt32(b, 0) == -1) return null;
            
            byte[] type = new byte[4];
            s.Receive(type);

            if (BitConverter.ToInt32(type, 0) == -1) return null;

            //output.AppendText("got response with type " + BitConverter.ToInt32(type, 0).ToString() + "\n");

            if (BitConverter.ToInt32(type, 0) == 1) {

                Bitmap image;

                int dataLen = BitConverter.ToInt32(b, 0);

                byte[] img = new byte[dataLen];
                s.Receive(img);

                if (BitConverter.ToInt32(img, 0) == -1) return null;

                using (var ms = new MemoryStream(img))
                {
                    image = new Bitmap(ms);
                }

                response.image = image;
                response.type = 1;

            }
            else
            {
                int dataLen = BitConverter.ToInt32(b, 0);

                byte[] text = new byte[dataLen];
                s.Receive(text);

                if (BitConverter.ToInt32(text, 0) == -1) return null;

                String rcv = "";
                for (int i = 0; i < text.Length; i++)
                    rcv += Convert.ToChar(text[i]);

                response.text = rcv;
                response.type = 0;
            }

            return response;

        }

        public void SendText(string text)
        {

            if (text == "") return;
            ASCIIEncoding asen = new ASCIIEncoding();
            
            byte[] bytes = asen.GetBytes(text);
            byte[] type = BitConverter.GetBytes((Int32) 0);
            byte[] userDataLen = BitConverter.GetBytes((Int32)bytes.Length);

            s.Send(userDataLen);
            s.Send(type);
            s.Send(bytes);

            output.AppendText("Server: " + text + " \n");

        }

        ~Server()
        {
            myList.Stop();
        }

    }
}
