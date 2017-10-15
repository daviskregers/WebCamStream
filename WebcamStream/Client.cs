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

        TcpClient tcpclnt;
        string address;
        string port;
        TextBox output;
        Form1 form;
        bool continueThreads;
        Stream stm;

        public Client(string address, string port, TextBox output, Form1 form)
        {

            this.address = address;
            this.port = port;
            this.output = output;
            this.form = form;

            this.continueThreads = true;

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

                ObjectDelegate del = new ObjectDelegate(UpdateTextBox);
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

        private void UpdateTextBox(object obj)
        {
            // do we need to switch threads?
            if (form.InvokeRequired)
            {
                // slightly different now, as we dont need params
                // we can just use MethodInvoker
                ObjectDelegate method = new ObjectDelegate(UpdateTextBox);
                form.Invoke(method, obj);
                return;
            }

            string text = (string)obj;

            output.AppendText("Server: " + text + "\n");

        }


        private void UpdateImageBox(object image)
        {
            if (form.InvokeRequired)
            {
                // slightly different now, as we dont need params
                // we can just use MethodInvoker
                ObjectDelegate method = new ObjectDelegate(UpdateImageBox);
                form.Invoke(method, image);
                return;
            }

            form.updateImage((Bitmap)image);

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
                    Bitmap image = this.ReceiveImage();

                    img.Invoke(image);

                }
            }
            catch (Exception e)
            {
                //output.AppendText("unexpected client error while receiving data \n" + e.Message + "\n");
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

            stm.Write(userDataLen, 0, userDataLen.Length);
            stm.Write(bytes, 0, bytes.Length);

        }

        private Bitmap ReceiveImage()
        {

            Bitmap image;

            byte[] readMsgLen = new byte[4];
            stm.Read(readMsgLen, 0, 4);
            

            int dataLen = BitConverter.ToInt32( readMsgLen, 0);

            byte[] readMsgData = new byte[dataLen];
            stm.Read(readMsgData, 0, dataLen);

            using (var ms = new MemoryStream(readMsgData))
            {
                image = new Bitmap(ms);
            }

            return image;
        }

        public void SendText(string text)
        {

            if (text == "") return;

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] bytes = asen.GetBytes(text);
            byte[] userDataLen = BitConverter.GetBytes((Int32)bytes.Length);
            byte[] userDataType = BitConverter.GetBytes( (Int32) 0 );

            stm.Write(userDataLen, 0, userDataLen.Length);
            stm.Write(bytes, 0, bytes.Length);
            
            
            
            output.AppendText("Client: " + text + " \n");

        }

        ~Client()
        {
            tcpclnt.Dispose();
            continueThreads = false;
        }


    }
}
