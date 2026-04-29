using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SoM.Ipc
{
    public class WatsonTcpConfiguration : SomIpcServiceOptions
    {

        public string TestWatsonSpecific { get; set; }

        public string Ip { get; set; } = null;

        public int Port { get; set; } = -1; 

        public IDictionary<string, string> Extras { get; set; }
    }
}
