using System;
using System.Collections.Generic;
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

        public IDictionary<String, ClientInformation> Connections =
            new Dictionary<String, ClientInformation>();
        public TcpServerHandler(string hostName1, int portNum1)
        {
            HostName = hostName1;
            PortNum = portNum1;
        }

        public string HostName { get; }
        public int PortNum { get; }

        public bool Run()
        {

            bool result = true;
            try
            {
                IPHostEntry ipHostInfo = null;
               
                if (String.IsNullOrWhiteSpace(HostName))
                {
                    ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    
                }
                else
                {
                    ipHostInfo = Dns.GetHostEntry(HostName);
                }

                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = 
                    new IPEndPoint(ipAddress, PortNum>0 ? PortNum : 11000);

                Socket listener = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(localEndPoint);

                Console.WriteLine("TcpServer Listening...");

                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

            }
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine($"Run: Error: {ex.ToString()}");
            }
            return result;
        }
        public void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                ClientState clientState = new ClientState(handler);

                byte[] buffer = new byte[ClientState.BufferSize];
                handler.BeginReceive(buffer, 0, ClientState.BufferSize, 0,
                    new AsyncCallback(ReadCallback), clientState);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AcceptCallback: Error: {ex.ToString()}");
            }
        }
        public void ReadCallback(IAsyncResult ar)
        {

            try
            {
                ClientState clientState = (ClientState)ar.AsyncState;
                Socket handler = clientState.WorkingSocket;

                // Read data from the client socket.  
                int read = handler.EndReceive(ar);

                // Data was read from the client socket.  
                if (read > 0)
                {
                    byte[] buffer = new byte[ClientState.BufferSize];
                    handler.BeginReceive(buffer, 0, ClientState.BufferSize, 0,
                        new AsyncCallback(ReadCallback), clientState);
                }
                else
                {
                    if (clientState.HasData)
                    {
                        // All the data has been read from the client;  
                        // display it on the console.  
                        Console.WriteLine($"Read {clientState.Message.Length} bytes from socket.\n Data : {clientState.Message}");

                        ProcessReceivedMessage(clientState);
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ProcessReceivedMessage: Error: {ex.ToString()}");
            }

        }
        public bool ProcessReceivedMessage(ClientState message)
        {
            bool result = true;
            try
            {
                Console.WriteLine("Processing Parsed Connection ClientMessage.");

                IMessage messageHandler = new ClientMessage();

                if (messageHandler.Parse(message))
                {

                    if (messageHandler.HasClientId && Connections.ContainsKey(messageHandler.ClientId))
                    {
                        ClientInformation ci = Connections[messageHandler.ClientId];
                        ci.Reply();
                    }
                    else
                    {
                        ClientIdentifier clientIdentifier = new ClientIdentifier();
                        ClientInformation ci = new ClientInformation(clientIdentifier, message);
                        Connections.Add(clientIdentifier.Id, ci);
                        ci.Reply();
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine($"ProcessReceivedMessage: Error: {ex.ToString()}");
            }
            return result;
        }

    }

}
