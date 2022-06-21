using System;
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
        public ClientState(Socket workingSocket)
        {
            WorkingBuffer = new StringBuilder();
            WorkingSocket = workingSocket;
        }
        public void ReceiveData(byte[] buffer, int bytesReceived)
        {
            WorkingBuffer.Append(Encoding.UTF8.GetString(buffer, 0, bytesReceived));
        }
        public String Message => WorkingBuffer.ToString();
        public bool HasData => WorkingBuffer.Length > 0;
        private StringBuilder WorkingBuffer { get; }
        public Socket WorkingSocket { get; set; }
        public void SendData(String data)
        {
            byte[] msg = System.Text.Encoding.UTF8.GetBytes(data);
            int bytesSent = WorkingSocket.Send(msg);
        }
    }
}
