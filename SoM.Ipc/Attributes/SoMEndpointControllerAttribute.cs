

using System;

namespace SoM.Ipc
{

    /// <summary>
    /// For SoM IPC. Mark methods as handlers for incoming message of a given type and name
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class SoMEndpointControllerAttribute : System.Attribute
    {

        /// <summary>
        /// The class contains Ipc Endpoints 
        /// </summary>

        public SoMEndpointControllerAttribute()
        {

        }
    }
}

