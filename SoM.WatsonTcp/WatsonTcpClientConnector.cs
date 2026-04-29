using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WatsonTcp;

namespace SoM.Ipc
{
    /// <summary>
    /// Net Framework version of Connector ( No Dependency Injection required )
    /// </summary>
    public class WatsonTcpClientConnector : SoM.Ipc.IClientConnector 
    {
        ILogger _logger;
        WatsonTcpClient _client;
        bool _shouldConnect = true;
        ClientMetadata _metaData = null;
        HubConnectionOptions _options;

        CancellationTokenSource _cts = new CancellationTokenSource();
        Action<SoMIpcMessage, IpcMetadata> _eventDelegate;

        public HubConnectionOptions ConnectionOptions => _options;

        public string Name => "WatsonTcp";
        public bool IsConnected => _client != null && _client.Connected;

        public bool IsConnecting => _client != null && ! _client.Connected && _shouldConnect;


        public event EventHandler<IpcMetadata> Connected;
        public event EventHandler<IpcMetadata> Disconnected;


        public IpcMetadata Metadata => _client.AsIpcMetaData();

        public WatsonTcpClientConnector(ILogger logger)
        {
            _logger = logger;
        }

        public async Task ConnectAsync(HubConnectionOptions options, CancellationToken cancel = default)
        {
            if (IsConnected)
                throw new InvalidProgramException("SoM Hub Client already connected");
            Connect(options);
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.Elapsed < TimeSpan.FromSeconds(5))
            {
                await Task.Delay(100);
                if (IsConnected)
                    return;
            }
        }

        public void Connect(HubConnectionOptions options)
        {
            if (IsConnected)
                throw new InvalidProgramException("SoM Hub Client already connected");
            if (options == null)
                throw new ArgumentNullException("HubConnectionOptions cannot be null");
            _options = options;
            _cts = new CancellationTokenSource();
            BuildClient();
            StartMonitoring();
            _shouldConnect = true;
        }



        public void Disconnect()
        {
            _shouldConnect = false;
            _cts.Cancel();
        }

        public void RegisterIpcEventHandler(Action<SoMIpcMessage, IpcMetadata> r)
        {
            _eventDelegate = r;
        }

        public async Task SendEventAsync(SoMIpcMessage message, CancellationToken cancel)
        {
            
            if (message == null)
                throw new ArgumentNullException("You are trying to send an null event to a SoM LPR Hub");
            byte[] payload = SerializePayload(message);
             /* fire event and let the client deal with errors */
             await _client.SendAsync(payload, null, 0, cancel);

        }

        public async Task<SoMIpcMessage> SendRequestAsync(SoMIpcMessage message, TimeSpan timeout, CancellationToken cancel)
        {
            if (message == null)
                throw new ArgumentNullException("You are trying to send an null event to a SoM LPR Hub");
            byte[] payload = SerializePayload(message);
            /* fire event and let the client deal with errors */
            SyncResponse rsp = await _client.SendAndWaitAsync(
                  (int)timeout.TotalMilliseconds, payload, null, 0, cancel);
            try
            {
                SoMIpcMessage reply = SoMIpcMessage.Deserialize(rsp.Data);
                return reply;
            } catch ( JsonException ex)
            {
                throw new JsonException("Deserializing SoMIpcMessage from server failed.", ex);
            }
        }

        void BuildClient()
        {
            _client = new WatsonTcpClient(_options.Server, _options.Port);
            _client.Events.ExceptionEncountered += (s, e) =>
            {
                if ( _logger != null)
                _logger.LogWarning("Remote SoM server threw {Type}", e.Exception.GetType());
                //OnExceptionOccured(s, e);
            };
            _client.Callbacks.SyncRequestReceivedAsync = OnSyncRequestAsync;
            _client.Events.MessageReceived += OnMessageReceived;
            _client.Events.ServerConnected += (s, e) =>
            {
                _metaData = e.Client;
                if (Connected != null)
                    Connected(this, e.Client.AsIpcMetaData());
            };
            _client.Events.ServerDisconnected += (s, e) =>
            {
                _metaData = null;
                if (Disconnected != null)
                    Disconnected(this, e.Client.AsIpcMetaData());
            };
        }

        protected void StartMonitoring()
        {
            CancellationToken cancel = _cts.Token;
            Task t = Task.Run(async () =>
            {
                System.Diagnostics.Debug.WriteLine("Started monitoring");
                while (!cancel.IsCancellationRequested)
                {
                    try
                    {
                        if (!IsConnected && this._shouldConnect)
                        {
                            _client.Connect();
                        }
                        if (IsConnected && !this._shouldConnect)
                        {
                            _client.Disconnect();
                        }
                        if (!IsConnected && !this._shouldConnect)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1), cancel);
                            continue;
                        }
                        if (IsConnected && _shouldConnect)
                        {
                            /* periodic monitoring */
                            await Task.Delay(TimeSpan.FromSeconds(30), cancel);
                            if (cancel.IsCancellationRequested)
                                break;
                            try
                            {
                                var json = new Ping()
                                {
                                    Data = $"ping"
                                }.Serialize();
                                var rx = await _client.SendAndWaitAsync(2000, json);
                                if (rx.Data == null)
                                {
                                    try { _client.Disconnect(); } catch { /* ignore */ }
                                }

                            }
                            catch 
                            {
                                try { _client.Disconnect(); } catch { /* ignore */ }
                            }
                        }

                    }
                    catch ( TaskCanceledException)
                    {
                        _client.Disconnect();
                        return;
                    }
                    catch (Exception ex)
                    {
                        if ( _logger != null)
                        _logger.LogWarning("Errors monitoring SoM Hub WatsonTcp connection ({Type})", ex.GetType());
                        await Task.Delay(TimeSpan.FromSeconds(3), cancel);
                        /* rebuild the client */
                        BuildClient();
                    }
                }
            });
        }   


        private async Task<SyncResponse> OnSyncRequestAsync(SyncRequest request)
        {
            throw new NotImplementedException();
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            SoMIpcMessage msg = null;
            try
            {
                string json = Encoding.UTF8.GetString(e.Data);
                msg = SoMIpcMessage.Deserialize(json);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.LogDebug("{Type} recieving event from SoM Server", ex.GetType());
                return;
            }


            if (_eventDelegate != null)
                _eventDelegate.Invoke(msg, e.Client.AsIpcMetaData());
        }


        byte[] SerializePayload(SoMIpcMessage message)
        {
            try
            {
                return UTF8Encoding.UTF8.GetBytes(message.Serialize());
            }
            catch (JsonException ex)
            {
                throw new JsonException("Serializing SoMIpcMessage to json failed.", ex);
            }
        }

      
    }


} 
