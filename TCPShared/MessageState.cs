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
    public class MessageState
    {
        public const int BufferSize = 1024;
       
        public static IDictionary<String, ClientInformation> Connections =
            new ConcurrentDictionary<string, ClientInformation>();

        public static int ReceivedMessageCount = 0;

        public MessageState(TcpClient client)
        {
            WorkingBuffer = new StringBuilder();
            Client = client;
        }
        public Message ClientReceiveData()
        {
            byte[] buffer = new byte[MessageState.BufferSize];
            NetworkStream ns = Client.GetStream();
            Message cm = null;
            int offset = 0;
            int read = 0;
            do
            {
               
                read = ns.Read(buffer, offset, BufferSize);
                if (read > 0)
                {
                   
                    String bufferToString = Encoding.UTF8.GetString(buffer, offset, read);
                    WorkingBuffer.Append(bufferToString);
                    offset += read;
                    if (read > 0 && read < BufferSize)
                    {
                        offset = 0;
                        cm = new Message();
                        ProcessReceivedMessage(this, ref cm);
                        break;
                    }
                    
                }

            } while (read>0);
            return cm;
        }
        public Message ServerReceiveData()
        {
            byte[] buffer = new byte[MessageState.BufferSize];
            NetworkStream ns = Client.GetStream();
            Message cm = null;
            int offset = 0;
            int read = 0;
            do
            {

                read = ns.Read(buffer, offset, BufferSize);
                if (read > 0)
                {

                    String bufferToString = Encoding.UTF8.GetString(buffer, offset, read);
                    WorkingBuffer.Append(bufferToString);
                    offset += read;
                    if (read > 0 && read < BufferSize)
                    {
                        offset = 0;
                        cm = new Message();
                        Console.WriteLine($"Received message # {ReceivedMessageCount++}.");
                        ProcessReceivedMessage(this, ref cm);
                        WorkingBuffer.Clear();
                    }

                }

            } while (read > 0);
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
        public static bool ProcessReceivedMessage(MessageState message, ref Message cm)
        {
            bool result = true;
            try
            {

                if (cm.Parse(message))
                {

                    if (cm.HasClientId && Connections.ContainsKey(cm.ClientId))
                    {
                        ClientInformation ci = Connections[cm.ClientId];
                        ci.Reply(cm);
                    }
                    else
                    {
                        ClientIdentifier clientIdentifier = new ClientIdentifier(cm.ClientId);
                        ClientInformation ci = new ClientInformation(clientIdentifier, message);
                        Connections.Add(clientIdentifier.Id, ci);
                        ci.Reply(cm);
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
