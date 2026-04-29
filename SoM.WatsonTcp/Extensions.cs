using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WatsonTcp;

namespace SoM.Ipc
{
    public static class Extensions
    {

        public static IpcMetadata AsIpcMetaData(this ClientMetadata metadata) =>
            new IpcMetadata()
            {
                ClientIpPort = metadata != null ? metadata.IpPort : null,
                ClientCertificate = null,
                TransportFrameworkContextObj = metadata
            };

        public static IpcMetadata AsIpcMetaData(this WatsonTcpClient client) =>
    new IpcMetadata()
    {
        ClientIpPort = $"?*:{client.Settings.LocalPort}",
        ClientCertificate = null,
        TransportFrameworkContextObj = client.Settings
    };


        public static void AddWatsonTcpConnector(this IServiceCollection services)
        {
            services.AddKeyedTransient<SoM.Ipc.IServerConnector, SoM.Ipc.WatsonTcpServerConnector>("WatsonTcp");
        }

    }
}
