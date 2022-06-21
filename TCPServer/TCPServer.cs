using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpShared;

namespace TCPServer
{

    public class TcpServerHandler
    {

       

        private ManualResetEvent wManualResetEvent = new ManualResetEvent(false);

        public TcpServerHandler(string hostName1, int portNum1)
        {
            HostName = hostName1;
            PortNum = portNum1;
        }

        public string HostName { get; set; }
        public int PortNum { get; set; }

        public bool Run()
        {
            
            bool result = true;
            try
            {
                 
               
                if (String.IsNullOrWhiteSpace(HostName))
                {
                    HostName = Dns.GetHostName();
                }

                if (PortNum == 0)
                {
                    PortNum = 11000;
                }
               
                var listener = new TcpListener(IPAddress.Any, PortNum);

                listener.Start();

                while (true)
                {
                    try
                    {
                        Console.Write("Waiting for connection...");

                        TcpClient client = listener.AcceptTcpClient();

                        ClientState cs = new ClientState(client);

                        Console.WriteLine("Connection accepted.");

                        Task.Run(() => cs.ReceiveData());

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine($"Run: Error: {ex.ToString()}");
            }
            return result;
        }
        
    }

}
