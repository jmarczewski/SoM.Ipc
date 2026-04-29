using Microsoft.Extensions.Logging;
using SoM.Ipc.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoM.Ipc
{
    public class IpcClient<T> where T : SoM.Ipc.IClientConnector
    {
        ILogger _logger;
        IClientConnector _connector;
        HubConnectionOptions _options;
        //Action<SoMIpcMessage, IpcMetadata> _eventDelegate;

        public event EventHandler<IpcMetadata> Connected;
        public event EventHandler<IpcMetadata> Disconnected;


        public HubConnectionOptions ConnectionOptions => _options;


        public IpcClient(ILogger logger)
        {
            _logger = logger;
            CreateConnector();
        }


        public bool IsConnected => _connector.IsConnected;
        public bool IsConnecting => _connector.IsConnecting;

        public IpcMetadata Metadata => _connector != null ? _connector.Metadata : null;

        public void Connect(HubConnectionOptions options)
        {
            if (_connector == null)
                throw new ArgumentNullException("No connector selected");
            _options = options;
            _connector.Connect(options);
        }

        public void Disconnect()
        {
            if (_connector != null)
                _connector.Disconnect();
        }

        /// <summary>
        /// Register method to invoke when server sends an event ( a one-way SoMIpcMessage with need for reply )
        /// </summary>
        /// <param name="r"></param>
        public void RegisterIpcEventHandler(Action<SoMIpcMessage, IpcMetadata> r)
        {
            _connector.RegisterIpcEventHandler(r);
        }

        /// <summary>
        /// Send an SoMIpcMeesage to the server and don't expect a reply
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public Task SendEventAsync(SoMIpcMessage message, CancellationToken cancel)
        {
            return _connector.SendEventAsync(message, cancel);
        }

        /// <summary>
        /// Send a SomIpcMessage request to the server and fetch reply
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="timeout"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public Task<SoMIpcMessage> SendRequestAsync(SoMIpcMessage msg, TimeSpan timeout = default, CancellationToken cancel = default)
        {
            return _connector.SendRequestAsync(msg, timeout, cancel);
        }


        void CreateConnector()
        {
            if (SoM.Ipc.SoMIpcMessage.TypeResolver == null)
            {
                /* use reflection to find end register all Dto Objects for json Serialization */
                var ipcMessageTypes = PolymorphicTypeFinder.GetSoMIpcMessageDerivedTypes(_logger);
                SoM.Ipc.SoMIpcMessage.TypeResolver = new PolymorphicJsonTypeResolver(ipcMessageTypes);

            }

            _connector = (SoM.Ipc.IClientConnector)Activator.CreateInstance(typeof(T), _logger);
            _connector.Connected += (sender, e) => { if (Connected != null) Connected(this, e); };
            _connector.Disconnected += (sender, e) => { if (Disconnected != null) Disconnected(this, e); };
        }

    }
}
