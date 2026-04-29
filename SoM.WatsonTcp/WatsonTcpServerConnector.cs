using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WatsonTcp;

namespace SoM.Ipc
{

    /// <summary>
    /// WatsonTcp communication wrapper used by IpcServer
    /// </summary>
    public class WatsonTcpServerConnector : IServerConnector
    {

        const int DEFAULT_PORT = 55315;

        int Port;
        string IpAddress;
        WatsonTcpServer _server;
        ILogger _logger;

        Func<SoMIpcMessage, IpcMetadata, SoMIpcMessage> _ipcRequestHandler = null;
        Func<SoMIpcMessage, IpcMetadata, Task<SoMIpcMessage>> _ipcAsyncRequestHandler = null;
        Action<SoMIpcMessage, IpcMetadata> _ipcEventHandler = null;
        WatsonTcpConfiguration _options;

        public string Name => "WatsonTcp";

        public event EventHandler<IpcMetadata> ClientConnected;
        public event EventHandler<IpcMetadata> ClientDisconnected;


        public SomIpcServiceOptions Configure(IConfigurationSection section, object builder, object iLogger)
        {


            WatsonTcpConfiguration cfg = new WatsonTcpConfiguration();
            /* manually assign values to avoid dependency on configuration.extensions */
            if (section.GetSection("Uri").Exists())
                cfg.Uri = section.GetSection("Uri").Value;

            if (builder != null)
            {
                Action<WatsonTcpConfiguration> del = (Action<WatsonTcpConfiguration>)builder;
                del.Invoke(cfg);
            }


            _logger = iLogger as ILogger;
            try
            {
                string ipPort = cfg.Uri;
                var tokens = ipPort?.Split(':');
                IpAddress = tokens[0];
                Port = int.Parse(tokens[1]);
            }
            catch (Exception ex)
            {
                _logger.LogError("{Type} parsing WatsonTcpServer ip and port from appsettings.json 'Uri' entry: {Ip}{Port}. IP:PORT format required. WatsonService will not start",
                    ex.GetType(), IpAddress, Port);
                throw ex;
            }
            _options = cfg;

            /* create WatsonTcp server instance */
            _server = new WatsonTcpServer(IpAddress, Port);
            _server.Events.ClientConnected += OnClientConnected;
            _server.Events.ClientDisconnected += OnClientDisconnected;
            _server.Events.MessageReceived += MessageReceived;
            _server.Callbacks.SyncRequestReceivedAsync = SyncRequestReceived;
           
            return _options;
        }

        public IpcMetadata[] ConnectedClients => _server.ListClients().Select(p => p.AsIpcMetaData()).ToArray();


        public void Start()
        {
            _server.Start();
            if ( _logger != null)
            _logger.LogInformation("Rpc server started (TCP/IP watson) on {Ip} port {Port}", IpAddress, Port);
        }

        public void Close()
        {
            try
            {
                _server.DisconnectClientsAsync();
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogWarning("{Type} disconnecting all WatsonTcp clients", ex.GetType());
            }
            _server.Stop();
        }

        public void RegisterIpcAsyncRequestHandler(Func<SoMIpcMessage, IpcMetadata, Task<SoMIpcMessage>> r)
        {
            _ipcAsyncRequestHandler = r;
        }
        public void RegisterIpcRequestHandler(Func<SoMIpcMessage, IpcMetadata, SoMIpcMessage> r)
        {
            _ipcRequestHandler = r;
        }

        public void RegisterIpcEventHandler(Action<SoMIpcMessage, IpcMetadata> r)
        {
            _ipcEventHandler = r;
        }

        public void SendEvent(SoMIpcMessage message, IpcMetadata client)
        {
            Task t = _server.SendAsync(client.ClientGuid, message.Serialize());
            t.Start();
            t.GetAwaiter().GetResult();
        }

        public async Task SendEventAsync(SoMIpcMessage message, IpcMetadata client)
        {
            await _server.SendAsync(client.ClientGuid, message.Serialize());
        }

        public int SendEvent(SoMIpcMessage message)
        {
            var t = Task<int>.Run(async () => { return await SendEventAsync(message); });
            t.Start();
            t.GetAwaiter().GetResult();
            return t.Result;
        }
   

        public async Task<int> SendEventAsync(SoMIpcMessage message)
        {
            async Task<bool> _sendAsync(Guid guid, string payload)
            {
                try
                {
                    return await _server.SendAsync(guid, payload);
                }
                catch
                {
                    return false;
                }
            }
            string data = message.Serialize();
            List<Task<bool>> tasks = new List<Task<bool>>();
            foreach (var meta in _server.ListClients().ToArray())
            {
                Task<bool> t = _sendAsync(meta.Guid, data);
            }
            return (await Task.WhenAll(tasks)).Where(p => p).Count();
        }


        public async Task<SoMIpcMessage> SendRequest(SoMIpcMessage message, IpcMetadata client, TimeSpan timeout = default, CancellationToken cancel = default)
        {
            int timeOutMs = timeout.TotalMilliseconds > 0 ? (int)timeout.TotalMilliseconds : 60;
            try
            {
                var reply = await _server.SendAndWaitAsync(timeOutMs, client.ClientGuid, message.Serialize(), null, 0, cancel);
                return SoMIpcMessage.Deserialize(reply.Data);
            } catch 
            {
                throw;
            }
        }



        #region Events and requests incoming from client
        private async Task<SyncResponse> SyncRequestReceived(SyncRequest request)
        {
            SoMIpcMessage msg;
            var metadata = request.Client.AsIpcMetaData();
            try
            {
                string json = Encoding.UTF8.GetString(request.Data);
                msg = SoMIpcMessage.Deserialize(json);
                if (msg == null)
                {
                    if (_logger != null)
                        _logger.LogWarning("Received null json rpc message from {Client}", metadata.ClientIpPort);
                    return new SyncResponse(request, new SoMIpcMessage()
                    {
                        Endpoint = "ipc-error",
                        IpcError = SoMIpcError.Create("json error", null)
                    }.Serialize()); ;
                }
            }
            catch (JsonException ex)
            {
                return new SyncResponse(request, new SoMIpcMessage()
                {
                    Endpoint = "icp-error",
                    IpcError = SoMIpcError.Create("json error", ex)
                }.Serialize());
            }
            try
            {

                /* send reply to client wrapped in SynResponce */

                SoMIpcMessage response = await _ipcAsyncRequestHandler.Invoke(msg, metadata);
                return new SyncResponse(request, response.Serialize());
            }
            catch (Exception ex)
            {
                return new SyncResponse(request, new SoMIpcMessage()
                {
                    Endpoint = "server-error",
                    IpcError = SoMIpcError.Create("error executing request", ex)
                }.Serialize());
            }
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            try
            {
                string json = Encoding.UTF8.GetString(args.Data);

                SoMIpcMessage msg = SoMIpcMessage.Deserialize(json);
                if (msg == null)
                {
                    if (_logger != null)
                        _logger.LogWarning("Received null json rpc message from {Client}", args.Client.IpPort);
                    return;
                }
                _ipcEventHandler.Invoke(msg, args.Client.AsIpcMetaData());
            }
            catch (JsonException ex)
            {
                if (_logger != null)
                    _logger.LogWarning("{Type} deserializing json message - {Msg} from {Client}",
                   ex.GetType(), args.Data != null ? Encoding.UTF8.GetString(args.Data) : "N.A",
                   args.Client);
            }

        }

        #endregion;

        void OnClientConnected(object sender, ConnectionEventArgs args)
        {
            try
            {
                if (_logger != null)
                    _logger.LogDebug("Client connected: {Metadata} ", args.Client.IpPort);

                if (ClientConnected != null)
                    ClientConnected(this, args.Client.AsIpcMetaData());

            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError("{Type} connecting client : {Metadata}", ex.GetType(), args.Client.IpPort);
            }
        }

        void OnClientDisconnected(object sender, DisconnectionEventArgs args)
        {
            try
            {
                if (_logger != null)
                    _logger.LogDebug("Client disconnected: {Metadata} : {Reason} ",
             args.Client.IpPort, args.Reason.ToString());
                if (ClientDisconnected != null)
                    ClientDisconnected(this, args.Client.AsIpcMetaData());
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogError("{Type} on client disconnect : {Metadata}", ex.GetType(), args.Client.IpPort);
            }
        }


    }
}
