using System;
using System.Collections.Generic;
using System.Text;

namespace SoM.Ipc.Model
{
    public class SoMIpcServiceInfo
    {
        public SoMIpcServiceInfo(object key, string frameworkName, object optionsBuilder, string customConfigSectionName)
        {
            Key = key;
            FrameworkName = frameworkName;
            OptionsBuilder = optionsBuilder;
            CustomConfigSectionName = customConfigSectionName;
        }

        public object Key { get; set; } = null;
       
        public string FrameworkName { get; set; } = null;

        public object OptionsBuilder { get; set; } = null;

        public string CustomConfigSectionName { get; set; } = null;


        

    }
}
