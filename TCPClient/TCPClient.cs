using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using TcpShared;

namespace TCPClient
{

    public class TcpClientHandler
    {

        public IDictionary<String, ClientInformation> Connections =
            new Dictionary<String, ClientInformation>();

        private ManualResetEvent wConnectManualResetEvent = new ManualResetEvent(false);
        private ManualResetEvent wSendManualResetEvent = new ManualResetEvent(false);
        private ManualResetEvent wReceiveManualResetEvent = new ManualResetEvent(false);

        public TcpClientHandler(string hostName1, int portNum1)
        {
            HostName = hostName1;
            PortNum = portNum1;
        }

        public string HostName { get; set; }
        public int PortNum { get; set; }

        public bool Run()
        {

            bool result = true;
            
            if (String.IsNullOrWhiteSpace(HostName))
            {
                HostName = Dns.GetHostName();
            }

            if (PortNum == 0)
            {
                PortNum = 11000;
            }

            try 
            {
                var client = new TcpClient(HostName, PortNum);

                ClientState cs = new ClientState(client);

                byte[] buffer = new byte[ClientState.BufferSize];

                ClientMessage sm = new ClientMessage();

                buffer = sm.Create("This is my message to you.");

                for (int messageIdx = 0; messageIdx < 1000000; messageIdx++)
                {
                    cs.SendData(buffer);
                    ClientMessage cm = null;
                    do
                    {
                        cm = cs.ReceiveData();
                        if (cm != null)
                        {
                            Console.WriteLine(cm.PayLoad);
                        }

                        Thread.Sleep(1000);

                    } while (cm != null);


                }
                
                
            }
            catch (ArgumentNullException ae)
            {
                Console.WriteLine("ArgumentNullException : {0}", ae.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

            return result;
        }


    }


}
