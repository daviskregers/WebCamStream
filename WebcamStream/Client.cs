using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

namespace WebcamStream
{
    class Client
    {

        class Response
        {
            public Bitmap image;
            public string text;
            public int type;
        }

        TcpClient tcpclnt;
        

        string address;
        string port;
        TextBox output;
        Form1 form;
        Stream stm;
        const int packetSize = 10000;

        public Client(string address, string port, TextBox output, Form1 form)
        {

            this.address = address;
            this.port = port;
            this.output = output;
            this.form = form;
                        
            tcpclnt = new TcpClient();

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

                output.AppendText("\nConnecting to " + address + "\n");
                tcpclnt.Connect(address, Int32.Parse(port));

                stm = tcpclnt.GetStream();
                                
                ObjectDelegate img = new ObjectDelegate(UpdateImageBox);

                Thread th = new Thread(new ParameterizedThreadStart(WorkThread));
                //th.Start(del);
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
                output.AppendText("\n Server: " + res.text + " \n");
            }

            return;

        }

        private void WorkThread(object obj)
        {
            
            try
            {

                // ObjectDelegate del = (ObjectDelegate)obj;
                ObjectDelegate img = (ObjectDelegate)obj;

                while (true)
                {
                    if (stm == null) continue;

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
                Console.WriteLine("unexpected client error while receiving data \n" + e.Message + "\n");

                //byte[] err = BitConverter.GetBytes(-1);
                //stm.Write(err, 0, err.Length);
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

            byte[] check = BitConverter.GetBytes((Int32)13371337);
            byte[] bytes;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                bytes = ms.ToArray();
            }


            byte[] userDataLen = BitConverter.GetBytes((Int32)bytes.Length);
            byte[] userDatType = BitConverter.GetBytes((Int32) 1 );

            Console.WriteLine("WEBCAM LENGTH " + bytes.Length);

            System.Array.Resize(ref check, packetSize);
            System.Array.Resize(ref bytes, packetSize);
            System.Array.Resize(ref userDataLen, packetSize);
            System.Array.Resize(ref userDatType, packetSize);

            stm.Write( check, 0, packetSize);
            stm.Write(userDataLen, 0, packetSize);
            stm.Write(userDatType, 0, packetSize);
            stm.Write(bytes, 0, packetSize);

        }

        public void Disconnect()
        {
            if(tcpclnt.Connected) { 
                stm.Close();
                stm.Dispose();
                tcpclnt.Close();
                tcpclnt.Dispose();
            }
        }

        private Response ReceiveImage()
        {

            Response response = new Response();

            byte[] check = new byte[packetSize];
            System.Array.Resize(ref check, packetSize);
            stm.Read(check, 0, packetSize);

            Console.WriteLine("STARTCHECK" + BitConverter.ToInt32(check, 0));
            if (BitConverter.ToInt32(check, 0) != 13371337) return null;

            byte[] readMsgLen = new byte[packetSize];
            stm.Read(readMsgLen, 0, packetSize);

            Int32 msgLen = BitConverter.ToInt32(readMsgLen, 0);

            Console.WriteLine("MSGLEN" + BitConverter.ToInt32(readMsgLen, 0));
            if (BitConverter.ToInt32(readMsgLen, 0) == -1) return null;

            byte[] type = new byte[packetSize];
            stm.Read(type, 0, packetSize);

            Console.WriteLine("type" + BitConverter.ToInt32(type, 0));
            if (BitConverter.ToInt32(type, 0) == -1) return null;
            
            if (BitConverter.ToInt32(type, 0) == 1)
            {
                Console.WriteLine("IS Image");
                Bitmap image;

                int dataLen = BitConverter.ToInt32(readMsgLen, 0);

                byte[] readMsgData = new byte[packetSize];
                stm.Read(readMsgData, 0, packetSize);

                Console.WriteLine("MsgData" + BitConverter.ToInt32(readMsgLen, 0));
                if (BitConverter.ToInt32(readMsgData, 0) == -1 ) return null;

                Array.Resize(ref readMsgData, dataLen);

                using (var ms = new MemoryStream(readMsgData))
                {
                    image = new Bitmap(ms);
                }

                response.image = image;
                response.type = 1;

            }
            else if( BitConverter.ToInt32(type, 0) == 0 )
            {
                Console.WriteLine("IS Text");
                int dataLen = BitConverter.ToInt32(readMsgLen, 0);

                byte[] text = new byte[packetSize];
                stm.Read(text, 0, packetSize);

                Array.Resize(ref text, dataLen);

                Console.WriteLine("text " + BitConverter.ToInt32(text, 0));
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
            byte[] userDataLen = BitConverter.GetBytes((Int32)bytes.Length);
            System.Array.Resize(ref userDataLen, packetSize);
            byte[] userDataType = BitConverter.GetBytes( (Int32) 0 );
            System.Array.Resize(ref userDataType, packetSize);

            byte[] check = BitConverter.GetBytes((Int32)13371337);
            System.Array.Resize(ref check, packetSize);

            stm.Write(check, 0, packetSize);
            stm.Write(userDataLen, 0, packetSize);
            stm.Write(userDataType, 0, packetSize);
            stm.Write(bytes, 0, packetSize);

            output.AppendText("\n Client: " + text + " \n");

        }

        ~Client()
        {
            tcpclnt.Dispose();
        }


    }
}
