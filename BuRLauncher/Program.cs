using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
//using OPCAutomation;

class Program
{
    static void Main(string[] args)
    {

        // Create UDP client
        int receiverPort = 11000;
        int batPort = 11001;
        try
        {
            UdpClient receiver = new UdpClient(receiverPort);
            UdpClient bat = new UdpClient(batPort);
            receiver.BeginReceive(DataReceived, receiver);
            bat.BeginReceive(DataReceived, bat);
            // Display some information
            Console.WriteLine("Starting Upd receiving on port: " + receiverPort + Environment.NewLine);
            Console.WriteLine("Starting Upd receiving on port: " + batPort + Environment.NewLine);
            Console.WriteLine("Press any key to quit.");
            Console.WriteLine("-------------------------------\n");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error:" + e.ToString() + "\n");
        }
        Console.ReadKey();
        //try
        // {
        //    OPCServer m_OPCServer = new OPCServer();
        // }

    }

    private static void DataReceived(IAsyncResult ar)
    {
        UdpClient c = (UdpClient)ar.AsyncState;
        IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        Byte[] receivedBytes = c.EndReceive(ar, ref receivedIpEndPoint);
        int i = 0, j = 0;
        string recText;
        bool isChromeOpen = false;

        recText = System.Text.Encoding.ASCII.GetString(receivedBytes);      //Convert Byte Array to ASCII String
        if (recText[0] != '\0')
        {                                            //Try empty message
            while ((i < (receivedBytes.Length)) && (receivedBytes[i] != ';'))                  //wait for ';'
                i++;
            char[] path = new char[i];                                      //Alloc storage of char array for path
            recText.CopyTo(0, path, 0, i);                                  //Copy char array
            string FileName = new string(path);                             //Convert char array to string
            if (recText[i] == ';')                                          //try for empty arguments text
                i++;
            j = i;                                                          //Save start position index of arguments string
            while ((i < (receivedBytes.Length)) && (receivedBytes[i] != '\0'))                 //wait for end of string
                i++;
            char[] arguments = new char[i - j];                             //Alloc storage of char array for arguments
            recText.CopyTo(j, arguments, 0, i - j);
            string Arguments = new string(arguments);

            try
            {
                if (FileName.CompareTo("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe") == 0)
                {
                    Process[] findChrome = Process.GetProcessesByName("chrome");
                    if (findChrome.Length != 0)
                        isChromeOpen = true;
                }

                if (!isChromeOpen)
                {
                    Process m_Proccess = new Process();
                    m_Proccess.StartInfo.FileName = FileName;
                    m_Proccess.StartInfo.Arguments = Arguments;
                    Process.Start(m_Proccess.StartInfo);

                    Console.Write("app is opened in path: " + FileName + Environment.NewLine + "Arguments: " + Arguments + Environment.NewLine);
                    Console.WriteLine("-------------------------------\n");
                }
                else
                {
                    Console.Write("Browser is already opened!" + Environment.NewLine);
                    Console.WriteLine("-------------------------------\n");
                }
            }
            catch (Exception e)
            {
                Console.Write("cannot open path: " + FileName + Environment.NewLine + "Arguments: " + Arguments + "Error: " + Environment.NewLine + e.ToString() + Environment.NewLine);
                Console.WriteLine("-------------------------------\n");
            }
        }
        else Console.Write("bad string" + Environment.NewLine);
        // Restart listening for udp data packages
        c.BeginReceive(DataReceived, ar.AsyncState);
    }
}
