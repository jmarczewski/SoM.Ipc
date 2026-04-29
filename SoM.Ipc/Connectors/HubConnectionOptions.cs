using System;
using System.Collections.Generic;
using System.Text;

namespace SoM.Ipc
{
    /// <summary>
    /// Subclass this class to specify connection options based on the framework used ie.
    /// WatsonTcp, SignalR. etc.
    /// </summary>
    public class HubConnectionOptions
    {
        public string Server { get; set; }

        public int Port { get; set; }

        public string Uri { get; set; } 

    }
}
