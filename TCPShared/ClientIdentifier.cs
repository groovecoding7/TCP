using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpShared
{
    public class ClientIdentifier
    {
        public string Id { get; set; }
        public ClientIdentifier()
        {
            Id = Guid.NewGuid().ToString();
        }
        public ClientIdentifier(String clientIdentifier)
        {
            Id = clientIdentifier;

        }
    }
}
