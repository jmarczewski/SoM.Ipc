using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoM.Ipc.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using System.Text;

namespace SoM.Ipc.Utils
{

    /// <summary>
    /// Repository for all SoMIpc services running in App ( Server )
    /// </summary>
    public class SoMIpcServices
    {
        IConfiguration _configuration;
        IServiceProvider _serviceProvider;

        static Dictionary<string, SoMIpcServiceInfo> _infos = new Dictionary<string, SoMIpcServiceInfo>();

        public SoMIpcServices(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;

        }

        public IpcServer Get(string key)
        {
            return _serviceProvider.GetKeyedService<IpcServer>(key);
        }

        public SoM.Ipc.IServerConnector GetConnector(string key)
        {
            return _serviceProvider.GetKeyedService<IServerConnector>(key);
        }

        public object GetConfigurationBuilder(string key)
        {
            if (!_infos.ContainsKey(key))
                return null;
            return _infos[key].OptionsBuilder;
        }

        public IConfigurationSection GetSection(string key)
        {
            if (!_infos.ContainsKey(key))
                throw new KeyNotFoundException($"SoMIpc service with key `{key}` does not exist");
            SoMIpcServiceInfo info = _infos[key];
            if (info.CustomConfigSectionName != null)
                return _configuration.GetSection(info.CustomConfigSectionName);
            else
                return _configuration.GetSection(key);
        }



        public static void RegisterInfo(string key, SoMIpcServiceInfo value)
        {
            if ( value == null)
                throw new ArgumentNullException("SoMIpcServiceInfo cannot be null");
            _infos[key] = value;
        }


    }
}
