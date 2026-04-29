using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoM.Ipc.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;


namespace SoM.Ipc
{
    public class IpcEndpointHandler { /* leave empty. Needed for fast finding subclasses via reflection */ }
 

    public class IpcServer  
    {

        IServerConnector _server;
        ILogger<IpcServer> _logger;
        SomIpcServiceOptions _options;
        IServiceProvider _serviceProvider;
        SoMIpcServices _somRepository;
        IpcHandlerMap _handlerMap;

        List<IpcMetadata> _clients = new List<IpcMetadata>();

        static SoMIpcMessage NotFoundSoMIpcResult = new SoMIpcMessage()
        {
            IpcError = new SoMIpcError()
            {
                ErrorType = "not found",
                Description = "endpoint not found"
            }
         };

        //public IpcServer([ServiceKey] object serviceKey, ILogger<IpcServer> logger, IConfiguration configuration, , IpcHandlerMap handlerMap)

        public IpcServer([ServiceKey] string serviceKey, ILogger<IpcServer> logger,  SoMIpcServices soMRepo, 
            IpcHandlerMap handlerMap, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _handlerMap = handlerMap;
            _serviceProvider = serviceProvider;
            _somRepository = soMRepo;

            /* Configure the Selected IServerConnector */
            object builder = soMRepo.GetConfigurationBuilder(serviceKey);
            IConfigurationSection section = soMRepo.GetSection(serviceKey);

            _server = soMRepo.GetConnector(serviceKey);

            _server.RegisterIpcEventHandler(OnEventReceived);
            _server.RegisterIpcRequestHandler(OnRequestReceived);
            _server.RegisterIpcAsyncRequestHandler(OnAsyncRequestReceived);

            _options = _server.Configure(section, builder, logger);
            _server.Start();

        }

        public IpcMetadata[] ConnectedClients => _server.ConnectedClients;

        void OnEventReceived(SoMIpcMessage msg, IpcMetadata meta)
        {
            try
            {
                if ( msg is null)
                {
                    _logger.LogWarning("[IPC-EVT] Null message from Ipc client ( {Client} )",  meta);
                    return;
                }
                if ( msg.Endpoint is null)
                {
                    _logger.LogWarning("[IPC-EVT] Event endpoint is null. ( call from {Client} )", msg.Endpoint, meta.ClientIpPort);
                    return;
                }
                MethodInfo mi = _handlerMap.GetEndpointInfo(msg.Endpoint);
                if (mi == null)
                {
                    _logger.LogWarning("[IPC-EVT] No handler for event `{Name}` found!. ( call from {Client} )", msg.Endpoint, meta.ClientIpPort);
                    return;
                }

                var invokeOn = _serviceProvider.GetService(mi.DeclaringType);
                mi.Invoke(invokeOn, new object[] { msg, meta });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Type} processing event from client {Args}", ex.GetType(), meta.ClientIpPort);
            }
        }

        async Task<SoMIpcMessage> OnAsyncRequestReceived(SoMIpcMessage msg, IpcMetadata meta)
        {
            try
            {
                if (msg is null)
                {
                    _logger.LogWarning("[IPC-CMD] Null message from Ipc client ( {Client} )", meta);
                    return NotFoundSoMIpcResult;
                }
                if (msg.Endpoint is null)
                {
                    /* Ping is the only message allowed without an enpoint */
                    if (msg.GetType() == typeof(SoM.Ipc.Ping))
                    {
                        return new SoM.Ipc.Ping()
                        {
                            Data = $"{DateTime.Now}",
                        };
                    }
                    _logger.LogWarning("[IPC-CMD] Event endpoint is null. ( call from {Client} )", msg.Endpoint, meta.ClientIpPort);
                    return NotFoundSoMIpcResult;
                }
                MethodInfo mi = _handlerMap.GetEndpointInfo(msg.Endpoint);
                if (mi == null)
                {
                    _logger.LogWarning("[IPC-CMD] No handler for command {Name} found!. ( call from {Client} )", msg.Endpoint, meta.ClientIpPort);
                    return NotFoundSoMIpcResult;
                }

                var invokeOn = _serviceProvider.GetService(mi.DeclaringType);
                var isAwaitable = mi.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
                object reply;
                if (isAwaitable)
                    reply = await (dynamic)mi.Invoke(invokeOn, new object[] { msg, meta });
                else
                    reply = mi.Invoke(invokeOn, new object[] { msg, meta });
                var result = reply as SoMIpcMessage;

                if (result == null)
                    return NotFoundSoMIpcResult;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Type} processing request from client {Args}", ex.GetType(), meta.ClientIpPort);
                return new SoMIpcMessage()
                {
                    IpcError = new SoMIpcError()
                    {
                        ErrorType = $"server internal {ex.GetType()} occurred",
                        Description = ex.Message
                    }
                };
            }
        }

        SoMIpcMessage OnRequestReceived(SoMIpcMessage msg, IpcMetadata meta)
        {
            Task<SoMIpcMessage> t = OnAsyncRequestReceived(msg, meta);
            t.Start();
            t.GetAwaiter().GetResult();
            return t.Result;
        }

        public int SendEvent(SoMIpcMessage message)
        {
            return _server.SendEvent(message);
        }

        public Task<int> SendEventAsync(SoMIpcMessage message)
        {
            return _server.SendEventAsync(message);
        }

        public void SendEvent(SoMIpcMessage message, IpcMetadata metadata)
        {
            _server.SendEvent(message, metadata);
        }

        public Task SendEventAsync(SoMIpcMessage message, IpcMetadata metadata)
        {
            return _server.SendEventAsync(message, metadata);
        }

        public Task<SoMIpcMessage> SendRequest(SoMIpcMessage message, IpcMetadata client, TimeSpan timeout = default, CancellationToken cancel = default)
        { 
             return _server.SendRequest(message, client, timeout, cancel);   
        }

    }
}
