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

        public Client(string address, string port, TextBox output, Form1 form)
        {

            this.address = address;
            this.port = port;
            this.output = output;
            this.form = form;
            
            tcpclnt = new TcpClient();

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

                output.AppendText("Connecting to " + address + "\n");
                tcpclnt.Connect(address, Int32.Parse(port));

                stm = tcpclnt.GetStream();
                
                ObjectDelegate img = new ObjectDelegate(UpdateImageBox);

                Thread th = new Thread(new ParameterizedThreadStart(WorkThread));
                //th.Start(del);
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

            if (res.type == 1)
            {
                form.updateImage((Bitmap)res.image);
            }
            else if (res.type == 0)
            {
                output.AppendText("Server: " + res.text + "\n");
            }

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

                    img.Invoke(image);

                    Thread.Sleep(50);

                }
            }
            catch (Exception e)
            {
                output.AppendText("unexpected client error while receiving data \n" + e.Message + "\n");

                byte[] err = BitConverter.GetBytes(-1);
                stm.Write(err, 0, err.Length);
                Thread.Sleep(1000);

                this.WorkThread(obj);
            }

        }

        private void SendImage()
        {

            Bitmap image = form.getImage();
            byte[] bytes;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                bytes = ms.ToArray();
            }

            byte[] userDataLen = BitConverter.GetBytes((Int32)bytes.Length);
            byte[] userDatType = BitConverter.GetBytes((Int32) 1 );

            stm.Write(userDataLen, 0, userDataLen.Length);
            stm.Write(userDatType, 0, userDatType.Length);
            stm.Write(bytes, 0, bytes.Length);

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

            byte[] readMsgLen = new byte[4];
            stm.Read(readMsgLen, 0, 4);

            if (BitConverter.ToInt32(readMsgLen, 0) == -1) return null;

            byte[] type = new byte[4];
            stm.Read(type, 0, 4);

            if (BitConverter.ToInt32(type, 0) == -1) return null;

            //output.AppendText("got response with type " + BitConverter.ToInt32(type, 0).ToString() + "\n");

            if (BitConverter.ToInt32(type, 0) == 1)
            {

                Bitmap image;

                int dataLen = BitConverter.ToInt32(readMsgLen, 0);

                byte[] readMsgData = new byte[dataLen];
                stm.Read(readMsgData, 0, dataLen);

                if (BitConverter.ToInt32(readMsgData, 0) == -1) return null;

                using (var ms = new MemoryStream(readMsgData))
                {
                    image = new Bitmap(ms);
                }

                response.image = image;
                response.type = 1;

            }
            else
            {
                int dataLen = BitConverter.ToInt32(readMsgLen, 0);

                byte[] text = new byte[dataLen];
                stm.Read(text, 0, dataLen);

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
            byte[] userDataLen = BitConverter.GetBytes((Int32)bytes.Length);
            byte[] userDataType = BitConverter.GetBytes( (Int32) 0 );

            stm.Write(userDataLen, 0, userDataLen.Length);
            stm.Write(userDataType, 0, userDataType.Length);
            stm.Write(bytes, 0, bytes.Length);

            output.AppendText("Client: " + text + " \n");

        }

        ~Client()
        {
            tcpclnt.Dispose();
        }


    }
}
