using SoM.Ipc;
using System;
using System.Collections.Generic;
using System.Text;
using MyIpc1;

namespace SrvConsole
{

    /* Example class that contains Endpoint Handlers for processing Ip */

    [SoMEndpointController]  // <-- mark class as containing endpoint handlers ( speeds up finding handler methods on startup )
    public class Controller
    {

        public void NormalMethods() { }


        /* receive a one-way event from the client */
        [SoMEndpoint("foo.evtA")]  // <- client specified foo.evtA as endpoint for the 
        public void OnAReceived(DemoIpcA msg, IpcMetadata src)
        {
            Console.WriteLine($"Got A = {msg.PropA}");
        }


        /* Example of a request from a client that needs a response */ 
        [SoMEndpoint("anything-unique-allowed")]
        public DemoIpcB Pong(DemoIpcA msg, IpcMetadata src)
        {
            Console.WriteLine($"Got A = {msg.PropA}");
            return new DemoIpcB() { PropB = $"Pong! {msg.PropA}" };
        }


        /* Async requests are wrapped in Task<SoMIpcMsg> */
        [SoMEndpoint("foo.pong")]
        public async Task<DemoIpcB> PongAsync(DemoIpcA msg, IpcMetadata src)
        {
            Console.WriteLine($"Got A = {msg.PropA}");
            return new DemoIpcB() { PropB = $"Pong! {msg.PropA}" };
        }
    }
}
