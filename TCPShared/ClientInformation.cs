using System.Net.Sockets;

namespace TcpShared
{
    public class ClientInformation
    {
        public const String AckMessage = "<Ack>";
        public ClientIdentifier ClientIdentifier { get; set; }
        public ClientState ClientState { get; set; }
        public ClientInformation(ClientIdentifier client, ClientState clientState)
        {
            ClientIdentifier = client;
            ClientState = clientState;
        }
        public void Reply()
        {
            ClientMessage cm = new ClientMessage();
            
            ClientState.SendData(cm.Create(AckMessage));
        }
        public void Send(String message)
        {
            ClientMessage cm = new ClientMessage();

            ClientState.SendData(cm.Create(message));
        }
    }
}