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

namespace WebcamStream
{
    class Client
    {

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

                ObjectDelegate del = new ObjectDelegate(UpdateTextBox);

                Thread th = new Thread(new ParameterizedThreadStart(WorkThread));
                th.Start(del);

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

        private void WorkThread(object obj)
        {

            try
            {
                ObjectDelegate del = (ObjectDelegate)obj;



                while (true)
                {

                    if (stm == null) continue;

                    byte[] bb = new byte[100];
                    int k = stm.Read(bb, 0, 100);

                    String rcv = "";
                    for (int i = 0; i < k; i++)
                        rcv += Convert.ToChar(bb[i]);

                    del.Invoke(rcv);

                }
            }
            catch
            {
                output.AppendText("Unexpected client error while receiving data \n");
            }
            
        }

        public void SendText(string text)
        {

            if (text == "") return;

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(text);
            stm.Write(ba, 0, ba.Length);
            output.AppendText("Client: " + text + " \n");

        }

        ~Client()
        {
            tcpclnt.Dispose();
        }


    }
}
