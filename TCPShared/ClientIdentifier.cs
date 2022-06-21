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
            Id = new Guid().ToString();
        }
        public ClientIdentifier(String clientIdentifier)
        {
            Id = clientIdentifier;

        }
    }
}
