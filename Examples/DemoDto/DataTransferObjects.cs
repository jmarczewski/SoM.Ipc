using SoM.Ipc;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace MyIpc1
{
    /// <summary>
    /// Create a Message Dto ( DataTransferObject) type to send to and from server. 
    /// Mark it as Dto and serializable by adding SoMIpc attribute with unique string discriminator  
    /// </summary>
    [SoMIpc("nov.A")]    
    public class DemoIpcA : SoMIpcMessage
    {
        public string PropA { get; set; }
    }


    /// <summary>
    /// Same as A
    /// </summary>
    [SoMIpc("nov.B")]

    public class DemoIpcB : SoMIpcMessage
    {
        public string PropB { get; set; }
    }

}
