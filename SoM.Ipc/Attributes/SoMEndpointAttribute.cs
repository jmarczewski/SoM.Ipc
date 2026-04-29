

using System;

namespace SoM.Ipc
{

    /// <summary>
    /// For SoM IPC. Mark methods as handlers for incoming message of a given type and name
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class SoMEndpointAttribute : System.Attribute
    {


        public int Framework { get; set; }

        /// <summary>
        /// The Endpoint value declared on the incoming SoMIpcMessage 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The subtype of SomIpcMessage<T> that this enpoint handles
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="framework"></param>
        public SoMEndpointAttribute(string name, int framework = 0)
        {
            Name = name;
            Framework = framework;
        }
    }
}

