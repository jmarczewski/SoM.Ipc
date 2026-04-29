using Microsoft.Extensions.DependencyInjection;
using SoM.Ipc.Utils;
using System;
using System.ComponentModel.Design;

namespace SoM.Ipc
{

    public static class IpcServerExtensions
    {
       
        public static void InitializeSoMIpcCommonServices(this IServiceCollection services)
        {
            /* Add Repository */
            services.AddSingleton<SoMIpcServices>();

            /* Add Endpoint Mapper */
            services.AddSingleton<IpcHandlerMap>();


            /* use reflection to find Endpoint handlers */
            IpcHandlerMap.RegisterHandlers(services);

            /* use reflection to find end register all Dto Objects for json Serialization */
            var ipcMessageTypes = PolymorphicTypeFinder.GetSoMIpcMessageDerivedTypes(null);
            SoMIpcMessage.TypeResolver = new PolymorphicJsonTypeResolver(ipcMessageTypes);

            /* Add Keyed IpcServer and Connector in Framework libraries */

        }

    }

}

