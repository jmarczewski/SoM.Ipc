using SoM;
using SoM.Ipc;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoM.Ipc
{ 
    
    [SoMIpc("_ping")] /* Mark for serialization with type discriminator */
    public class Ping : SoMIpcMessage
    {
        public string Data { get; set; }

    }
}
