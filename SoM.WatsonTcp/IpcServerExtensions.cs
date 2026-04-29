

using Microsoft.Extensions.DependencyInjection;
using SoM.Ipc.Model;
using SoM.Ipc.Utils;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace SoM.Ipc
{

    public static class IpcServerExtensions
    {
        public static void AddKeyedSoMIpcWatsonTcpService(this IServiceCollection services, string key, 
            Action<WatsonTcpConfiguration> optionsBuilder = null, string customConfigSectionName = null) 
        {
            /* create Info object needed for service initialization */
            SoMIpcServiceInfo info = new SoMIpcServiceInfo(key, "WatsonTcp", optionsBuilder, customConfigSectionName);

            /* Store the config object for use during service initialization */
            SoMIpcServices.RegisterInfo(key, info);

            /* register common services like Type and Endpoint Resolvers */
            services.InitializeSoMIpcCommonServices();

            /* Add Connector to be used by IpcService ( WatsonTcp ) */
            services.AddKeyedTransient<SoM.Ipc.IServerConnector, SoM.Ipc.WatsonTcpServerConnector>(key);

            /* add IPC Service with key */
            services.AddKeyedSingleton<IpcServer>(key);



            ///* Add Repository */
            //services.AddSingleton<SoMIpcServices>();
            
            ///* Add Endpoint Mapper */
            //services.AddSingleton<IpcHandlerMap>();

            ///* Add Connector to be used by IpcService ( WatsonTcp ) */
            //services.AddKeyedTransient<SoM.Ipc.IServerConnector, SoM.Ipc.WatsonTcpServerConnector>(key);

            ///* add IPC Service */
            //services.AddKeyedSingleton<IpcServer>(key);

            ///* use reflection to find Endpoint handlers */
            //IpcHandlerMap.RegisterHandlers(services);

            ///* use reflection to find end register all Dto Objects for json Serialization */ 
            //var ipcMessageTypes = PolymorphicTypeFinder.GetSoMIpcMessageDerivedTypes();
            //SoMIpcMessage.TypeResolver = new PolymorphicJsonTypeResolver(ipcMessageTypes);

 
        }


      

    }
}