using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SoM.Ipc
{
    /// <summary>
    /// Interface to be implemented by any remoting framework used for comms 
    /// ie. WatsonTcp, SignalR, SocketsIo, etc.
    /// Used by IpcService to run comms through the framework
    /// Server side implementation that accepts connections from IClientConnector clients.
    /// </summary>
    public interface IServerConnector
    {
        /// <summary>
        /// Connector raises this event when a framework client connects
        /// </summary>
        event EventHandler<IpcMetadata> ClientConnected;

        /// <summary>
        /// Connector raises this event when a framework client disconnects
        /// </summary>
        event EventHandler<IpcMetadata> ClientDisconnected;

        /// <summary>
        /// Friendly name ie. SignalR, WatsonTcp, etc. 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Will be onvoked by the IpcServer on the main app.
        /// .Net Core standard ( Microsoft.Extensions ) IConfiguration and ILogger
        /// objects will be always provided
        /// </summary>
        /// <param name="_IConfiguration"></param>
        /// <param name="_ILogger"></param>
        //void Configure(SomIpcServiceOptions options, object ILogger);

        SomIpcServiceOptions Configure(IConfigurationSection  section, object builder, object iLogger);

        IpcMetadata[] ConnectedClients { get; }

        void Start();

        void Close();

        void RegisterIpcEventHandler(Action<SoMIpcMessage, IpcMetadata> r);

        void RegisterIpcRequestHandler(Func<SoMIpcMessage, IpcMetadata, SoMIpcMessage> r);

        void RegisterIpcAsyncRequestHandler(Func<SoMIpcMessage, IpcMetadata, Task<SoMIpcMessage>> r);

        void SendEvent(SoMIpcMessage message, IpcMetadata client);

        Task SendEventAsync(SoMIpcMessage message, IpcMetadata client);

        int SendEvent(SoMIpcMessage message);

        Task<int> SendEventAsync(SoMIpcMessage message);

        Task<SoMIpcMessage> SendRequest(SoMIpcMessage message, IpcMetadata client, TimeSpan timeout = default, CancellationToken cancel = default);
    }
}
