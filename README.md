# SoM.Ipc
An Net API style Ipc abstraction independent of used framework ( WatsonTcp, SignalR, other )

A library that allows quick setup of Ipc ( [ Inter | Remote ]-Process-Communication) between
.Net Core/Core or Core/Framework apps with minimal boilerplate. Abstracts IPC in a way that
allows trivial changes of underling communication framework ( SignalR, WatsonTcp, RabbitMq, etc) without changes to methods that actually handle the incoming events/requests. .
We use it to migrate existing services from Net Framework to .Net with WatsonTcp being a commonly supported step framework along the way.
Uploaded to NuGet for convenience of use.

## Features:
* Supports Events ( one way send ) ot Requests ( needs a response ) between clients/server ( both ways )
* Follows ASP.Net Api style ( Use attributes to mark a class as an IPC controller and a method as end an endpoint handler )
* Monitors connection with periodic pings and Auto-reconnects
* Serializes dto's to json with short type discriminators
  
 


### Server usage 

Register with Dependency Injection in _program.cs_:

```
    
    var builder = Host.CreateApplicationBuilder(args);
     …

    builder.Services.AddKeyedSoMIpcWatsonTcpService("MyIpc1",
        (options) =>
        {
            options.Uri = "127.0.0.1:50543";
        });
     …
```

or use _appsettings.json_

```
  "MyIpc1": {
    "Uri": "10.0.0.100:50543"
  },
```
with

```
    # builder.Services.AddKeyedSoMIpcWatsonTcpService("MyIpc1")
```

Server

```
    IpcServices _ipc;  // DI injected
    var srv = _ipc.Get("MyIpc1");  // <- get server instance
   
   …
   
   /* send event to all connected clients */
    srv.SendEventAsync( new MyIpc1.DemoIpcA() { Endpoint = "foo.evtA") });

```



BoilerPlate that IS required 

```
/* server side */

 [SoMEndpointController]  // <-- mark class as containing endpoint handlers ( speeds up finding handler methods on startup )
 class Controller {
    … 
    /* receive a one-way event from the client */
    [SoMEndpoint("foo.evtA")]  // <- client specified foo.evtA as endpoint for the 
    public void OnAReceived(DemoIpcA msg, IpcMetadata src)
    {
      Console.WriteLine($"Got A = {msg.PropA}");
    }
    … 
  }
  ```

  ```

 /* Dto objects ( inherited from SoMIpcMessage )

    /// <summary>
    /// Create a Message Dto ( DataTransferObject) type to send to and from server. 
    /// Mark it as Dto and serializable by adding SoMIpc attribute with unique string discriminator  
    /// </summary>
    [SoMIpc("nov.A")]    
    public class DemoIpcA : SoMIpcMessage
    {
        public string PropA { get; set; }
    }

```


### client usage 

```
 /* client side call to server */


  IpcClient<WatsonTcpClientConnector> _ipc = new IpcClient<FrWatsonTcp>( optionalILogger);

  _ipc.Connect(new HubConnectionOptions() {
            Server = "127.0.0.1",
            Port = 50543,
  });

   … 
   /* invoke server method marked as eventhandler for "foo.evtA" , passing it our Dto  */
   await _ipc.SendEventAsync(new MyIpc1.DemoIpcA() { Endpoint = "foo.evtA", PropA = "FooFeeFiFoFum" }, _cts.Token);

```

## Examples

There are minimal sample apps: Server ( .Net/Console ), Client ( Winforms/.Net Framework ) 
and common class library with Dto's in the *Examples* folder


### Project Git Page

https://github.com/jmarczewski/SoM.Ipc.git