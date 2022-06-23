using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpShared
{
    public interface IMessage
    {
        bool TryParse(String message);
        String ClientId { get; set; }
        String PayLoad { get; set; }
        bool HasClientId { get; }
        bool HasPayLoad { get; }
    }
    public class Message : IMessage
    {
        public const String MessageTemplate =
            $"{MessageStart}{ClientIdentifierStart}{ClientIdentifierEnd}{PayLoadStart}{PayLoadEnd}{MessageEnd}";

        public const String ClientIdentifierStart = "<ClientIdentifier>";
        public const String ClientIdentifierEnd = "</ClientIdentifier>";
        public const String MessageStart = "<ClientMessage>";
        public const String MessageEnd = "</ClientMessage>";
        public const String PayLoadStart = "<Payload>";
        public const String PayLoadEnd = "</Payload>";

        public Message()
        {
        }
        public Message(String clientId)
        {
            ClientId = clientId;
        }
        public String ClientId { get; set; }
        public String PayLoad { get; set; }
        public byte[] Create(String message)
        {
            byte[] buffer = new byte[message.Length + MessageTemplate.Length];

            PayLoad = message;
            String fullMessage = $"{MessageStart}{ClientIdentifierStart}{ClientId}{ClientIdentifierEnd}{PayLoadStart}{message}{PayLoadEnd}{MessageEnd}";
            return System.Text.Encoding.UTF8.GetBytes(fullMessage);
        }
        public bool TryParse(String message)
        {
           
            int startMessageIdx = -1;
            int endMessageIdx = -1;
            try
            {
                
                startMessageIdx = message.IndexOf(MessageStart);
                
                if (startMessageIdx > -1)
                {
                    endMessageIdx = message.IndexOf(MessageEnd, startMessageIdx);

                    int clientIdStartIdx = startMessageIdx + MessageStart.Length + ClientIdentifierStart.Length;
                    int clientIdEndIdx = message.IndexOf(ClientIdentifierEnd, clientIdStartIdx);

                    ClientId = message.Substring(clientIdStartIdx, clientIdEndIdx - clientIdStartIdx);

                    int payLoadStartIdx = clientIdEndIdx + ClientIdentifierEnd.Length + PayLoadStart.Length;
                    int payLoadEndIdx = message.IndexOf(PayLoadEnd, payLoadStartIdx);

                    PayLoad = message.Substring(payLoadStartIdx, payLoadEndIdx - payLoadStartIdx);
                }
              
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return ClientId != null && PayLoad != null;
        }
        public bool HasClientId => String.IsNullOrWhiteSpace(ClientId) == false;
        public bool HasPayLoad => String.IsNullOrWhiteSpace(PayLoad) == false;

        public override string ToString()
        {
            return $"ClientMessage: ClientId: {ClientId}, PayLoad={PayLoad}";
        }
    }

}
