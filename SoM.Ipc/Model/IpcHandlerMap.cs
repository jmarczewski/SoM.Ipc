using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SoM.Ipc
{
    public class IpcHandlerMap
    {
        public class IpcEndpointHandler { /* leave empty. Needed for fast finding subclasses via reflection */ }

        ILogger<IpcHandlerMap> _logger;

        static Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();

        public IpcHandlerMap(ILogger<IpcHandlerMap> logger)
        {
            _logger = logger;
        }

        public static void RegisterHandlers(IServiceCollection serviceCollection) {
            var classes =
                 Assembly.GetCallingAssembly().GetTypes()
                 .Union(Assembly.GetExecutingAssembly().GetTypes())
                 .Union(Assembly.GetEntryAssembly().GetTypes())
                 //.Where(p => p.IsSubclassOf(typeof(IpcEndpointHandler)))
                 .Where(p => p.GetCustomAttribute<SoMEndpointControllerAttribute>() != null)
                 .Select(p => p.GetTypeInfo()).ToArray();
            
            foreach ( TypeInfo typeInfo in classes)
            {
                var methods = typeInfo.DeclaredMethods
                   .Where(m => m.GetCustomAttribute<SoMEndpointAttribute>() != null)
                   .ToArray();
                serviceCollection.AddSingleton(typeInfo.AsType());
                foreach (MethodInfo mi in methods)
                {
                    var rpcAttr = mi.GetCustomAttribute<SoMEndpointAttribute>();
                    _methods[rpcAttr.Name] = mi;
                }
            }
        }

        public MethodInfo GetEndpointInfo(string endpointName) {
            try
            {
                return _methods[endpointName];
            }
            catch (KeyNotFoundException)
            {
                _logger.LogError("Endpoint {Endpoint} not found", endpointName);
                return null;
            }
            catch ( ArgumentNullException )
            {
                _logger.LogError("Endpoint {Endpoint} is null", endpointName);
                return null;
            }
        }

    }
}
