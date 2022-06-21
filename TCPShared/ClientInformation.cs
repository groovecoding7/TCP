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
           ClientState.SendData(AckMessage);
        }
        public void Send(String message)
        {
            ClientState.SendData(message);
        }
    }
}