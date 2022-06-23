using System.Net.Sockets;

namespace TcpShared
{
    public class ClientInformation
    {
        public const String AckMessage = "<Ack>";
        public ClientIdentifier ClientIdentifier { get; set; }
        public MessageState ClientState { get; set; }
        public ClientInformation(ClientIdentifier client, MessageState clientState)
        {
            ClientIdentifier = client;
            ClientState = clientState;
        }
        public void Reply(Message cm)
        {
            byte[] ackMsg = cm.Create(AckMessage);

            ClientState.SendData(ackMsg);
        }
        public void Send(String message)
        {
            Message cm = new Message(ClientIdentifier.Id);
            byte[] m = cm.Create(message);
            ClientState.SendData(m);
        }
    }
}