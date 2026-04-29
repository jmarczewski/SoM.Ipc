using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoM.Ipc
{
    /// <summary>
    /// Interface to be implemented by any remoting framework used for comms 
    /// ie. WatsonTcp, SignalR, SocketsIo, etc.
    /// Client Side connection that connects to the IServerConnector 
    /// </summary>
    public interface IClientConnector 
    {

        /// <summary>
        /// Connector raises this event when a framework client connects
        /// </summary>
        event EventHandler<IpcMetadata> Connected;

        /// <summary>
        /// Connector raises this event when a framework client disconnects
        /// </summary>
        event EventHandler<IpcMetadata> Disconnected;

        /// <summary>
        /// Friendly name ie. SignalR, WatsonTcp, etc. 
        /// </summary>
        string Name { get; }

        HubConnectionOptions ConnectionOptions { get; }

        IpcMetadata Metadata { get; }

        bool IsConnected { get; }

        bool IsConnecting { get; }

        Task ConnectAsync(HubConnectionOptions options, CancellationToken cancel = default);

        void Connect(HubConnectionOptions options);


        void Disconnect();

        /// <summary>
        /// Register method to invoke when server sends an event ( a one-way SoMIpcMessage with need for reply )
        /// </summary>
        /// <param name="r"></param>
        void RegisterIpcEventHandler(Action<SoMIpcMessage, IpcMetadata> r);

        /// <summary>
        /// Send an SoMIpcMeesage to the server and don't expect a reply
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        Task SendEventAsync(SoMIpcMessage message, CancellationToken cancel);

        /// <summary>
        /// Send a SomIpcMessage request to the server and fetch reply
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="timeout"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        Task<SoMIpcMessage> SendRequestAsync(SoMIpcMessage msg, TimeSpan timeout, CancellationToken cancel);
    
        
    
    
    }
}
