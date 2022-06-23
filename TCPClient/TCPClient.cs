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
        private ManualResetEvent wReceiveManualResetEvent = new ManualResetEvent(false);
        private ManualResetEvent wSendManualResetEvent = new ManualResetEvent(false);

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

            Thread.Sleep(3000);

            try 
            {

                var client = new TcpClient(HostName, PortNum);

                using (client)
                {
                    int messageIdx = 0;
                    while (true)
                    {
                        String message = $"This is message # {++messageIdx}.";

                        MessageState cs = new MessageState(client);

                        byte[] buffer = new byte[MessageState.BufferSize];

                        Message sm = new Message();

                        buffer = sm.Create(message);

                        cs.SendData(buffer);

                        Message cm = null;
                        do
                        {
                            cm = cs.ClientReceiveData();
                            if (cm != null)
                            {
                                break;
                            }

                        } while (cm != null);
                        Thread.Sleep(5);
                    }

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

            Console.WriteLine($"Successfully sent {int.MaxValue} messages.");
            return result;
        }


    }


}
