using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SoM.Ipc
{
    public class IpcMetadata
    {
        /// <summary>
        /// Ip:Port of client making the call
        /// </summary>
        public string ClientIpPort { get; set; } 
        
        /// <summary>
        /// Optional client certificate if provided.
        /// </summary>
        public X509Certificate ClientCertificate { get; set; }
        
        /// <summary>
        /// Optional native context object dependent on Ipc framework used
        /// </summary>
        public object TransportFrameworkContextObj { get; set; }


        /// <summary>
        /// Optional Client Guid used by server to id client
        /// </summary>
        public Guid ClientGuid { get; set; } = Guid.NewGuid();


        public override string ToString()
        {
            return ClientIpPort;
        }

    }
}
