using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;

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

            output.AppendText("Client: " + text + "\n");

        }

        private void WorkThread(object obj)
        {

            ObjectDelegate del = (ObjectDelegate)obj;

            try
            {

                while (true)
                {

                    if (!s.Connected) continue;

                    string rcv = "";
                    del.Invoke(rcv);

                }

            }
            catch
            {
                output.AppendText("Error encountered in server workthread");
            }


        }

        ~Server()
        {
            myList.Stop();
        }

    }
}
