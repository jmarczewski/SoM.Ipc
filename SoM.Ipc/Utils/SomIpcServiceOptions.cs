using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace SoM.Ipc
{


    /// <summary>
    /// Base Options class. We only assume Uri to be valid for each framework.
    /// More options should be placed in derived per framework classes ( like WatsonTcpConfiguration )
    /// </summary>
    public class SomIpcServiceOptions
    {
        public static Dictionary<string, SomIpcServiceOptions> Registered { get; } = new Dictionary<string, SomIpcServiceOptions>();

        public static void AddOptions(string key, SomIpcServiceOptions options)
        {
            Registered[key] = options;
        }

        public string Uri { get; set; }

    }
}

