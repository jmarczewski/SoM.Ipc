using System;
using System.Collections.Generic;
using System.Text;

namespace SoM.Ipc
{

    /// <summary>
    /// Add to sub classes of SoMIpcMessage instead of registering JsonDerivedType
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SoMIpcAttribute : System.Attribute
    {
        public string Discriminator { get; private set; }

        public SoMIpcAttribute(string descriminator)
        {
            Discriminator = descriminator;
        }
    }
}
