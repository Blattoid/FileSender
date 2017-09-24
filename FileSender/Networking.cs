using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace fileTransfer
{
    class TcpClients
    {
        //Define function Upload()
        public static void Upload(String server, Int32 Port, string Filename)
        {
            try
            {

                int bufferSize = 1024;
                byte[] buffer = null;
                byte[] header = null;


                FileStream fs = new FileStream(Filename, FileMode.Open);

                int bufferCount = Convert.ToInt32(Math.Ceiling((double)fs.Length / (double)bufferSize));



                TcpClient tcpClient = new TcpClient(server, Port);
                tcpClient.SendTimeout = 600000;
                tcpClient.ReceiveTimeout = 600000;

                string headerStr = "Content-length:" + fs.Length.ToString() + "\r\n";
                header = new byte[bufferSize];
                Array.Copy(Encoding.ASCII.GetBytes(headerStr), header, Encoding.ASCII.GetBytes(headerStr).Length);

                tcpClient.Client.Send(header);

                for (int i = 0; i < bufferCount; i++)
                {
                    buffer = new byte[bufferSize];
                    int size = fs.Read(buffer, 0, bufferSize);

                    tcpClient.Client.Send(buffer, size, SocketFlags.Partial);

                }

                tcpClient.Client.Close();

                fs.Close();
                Console.WriteLine("Send complete.");
            }


            catch (Exception e)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error sending file. " + e.Message);

            }
        }


        // Incoming data from the client.  
        public static void RecieveData(int port, string filename)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();


                Socket socket = listener.AcceptSocket();

                int bufferSize = 1024;
                byte[] buffer = null;
                byte[] header = null;
                string headerStr = "";
                int filesize = 5;


                header = new byte[bufferSize];

                socket.Receive(header);

                headerStr = Encoding.ASCII.GetString(header);


                string[] splitted = headerStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                Dictionary<string, string> headers = new Dictionary<string, string>();
                foreach (string s in splitted)
                {
                    if (s.Contains(":"))
                    {
                        headers.Add(s.Substring(0, s.IndexOf(":")), s.Substring(s.IndexOf(":") + 1));
                    }

                }
                //Get filesize from header
                filesize = Convert.ToInt32(headers["Content-length"]);

                int bufferCount = Convert.ToInt32(Math.Ceiling((double)filesize / (double)bufferSize));


                FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);

                Console.WriteLine("Recieving data..."); //At this point data is being sent. Let's inform the user of this :)
                while (filesize > 0)
                {
                    buffer = new byte[bufferSize];

                    //recieve data into buffer

                    int size = socket.Receive(buffer, SocketFlags.Partial);

                    fs.Write(buffer, 0, size);

                    filesize -= size;
                }


                fs.Close();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error recieving: " + e.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }

}