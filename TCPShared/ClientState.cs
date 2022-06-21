using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpShared
{
    public class ClientState
    {
        public const int BufferSize = 1024;
        public static IDictionary<String, ClientInformation> Connections =
            new ConcurrentDictionary<string, ClientInformation>();
        public ClientState(TcpClient client)
        {
            WorkingBuffer = new StringBuilder();
            Client = client;
        }
        public ClientMessage ReceiveData()
        {
            byte[] buffer = new byte[ClientState.BufferSize];
            NetworkStream ns = Client.GetStream();
            ClientMessage cm = null;
            int offset = 0;
            int read = 0;
            do
            { 
                read = ns.Read(buffer, offset, BufferSize);
                if (read > 0)
                {
                    offset += read;
                    WorkingBuffer.Append(Encoding.UTF8.GetString(buffer, offset, read));
                }

            } while (read>0);

            if (read > 0)
            {
                cm = new ClientMessage();
                ProcessReceivedMessage(this, ref cm);
            }

            return cm;
        }
        public String Message => WorkingBuffer.ToString();
        public bool HasData => WorkingBuffer.Length > 0;
        private StringBuilder WorkingBuffer { get; }
        public TcpClient Client { get; set; }
        public void SendData(byte[] data)
        {
            NetworkStream ns = Client.GetStream();
            ns.Write(data, 0, data.Length);
        }
        public static bool ProcessReceivedMessage(ClientState message, ref ClientMessage cm)
        {
            bool result = true;
            try
            {
                Console.WriteLine("Processing Parsed Connection ClientMessage.");

                if (cm.Parse(message))
                {
                    if (cm.HasClientId && Connections.ContainsKey(cm.ClientId))
                    {
                        ClientInformation ci = Connections[cm.ClientId];
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
