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
        //OPCServer objOPCServer;
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

        // Convert data to ASCII
        recText = System.Text.Encoding.ASCII.GetString(receivedBytes);
        if (recText[0] != '\0')
        {
            while ((i < 500) && (receivedBytes[i] != ';'))
                i++;
            char[] path = new char[i];
            recText.CopyTo(0, path, 0, i);
            string FileName = new string(path);
            if (recText[i] == ';')
                i++;
            j = i;
            while ((i < 500) && (receivedBytes[i] != '\0'))
                i++;
            char[] arguments = new char[i - j];
            recText.CopyTo(j, arguments, 0, i - j);
            string Arguments = new string(arguments);

            Process m_Proccess = new Process();
            m_Proccess.StartInfo.FileName = FileName;
            m_Proccess.StartInfo.Arguments = Arguments;
            try
            {
                using (Process execute = Process.Start(m_Proccess.StartInfo))
                {
                    execute.WaitForExit();
                    Console.Write("app is opened in path: " + FileName + Environment.NewLine + "Arguments: " + Arguments + Environment.NewLine);
                    Console.WriteLine("-------------------------------\n");
                }
            }
            catch (Exception e)
            {
                Console.Write("cannot open path: " + FileName + Environment.NewLine + "Arguments: " + Arguments + "Error: " +Environment.NewLine + e.ToString() + Environment.NewLine);
                Console.WriteLine("-------------------------------\n");
            }
        }
        else Console.Write("bad string" + Environment.NewLine);
        // Restart listening for udp data packages
        c.BeginReceive(DataReceived, ar.AsyncState);
    }
}