using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace SoM.Ipc
{
    public static class PolymorphicTypeFinder
    {
        /// <summary>
        /// Find all Subclasses of SoMIpcMessage decorated with SomIpcAttribute in all referenced 
        /// non-system ( not starting with System nor Microsoft ) 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        public static JsonDerivedType[] GetSoMIpcMessageDerivedTypes(ILogger logger)
        {
            List<JsonDerivedType> Result = new List<JsonDerivedType>();
            Dictionary<Type, object> Used = new Dictionary<Type, object>();
            
            var searchIn = Assembly.GetEntryAssembly().GetReferencedAssemblies()
                .Where((a) =>
                !a.Name.StartsWith("Microsoft") &&
                !a.Name.StartsWith("System")).ToList();
            searchIn.Add(Assembly.GetEntryAssembly().GetName());

            foreach( AssemblyName name in searchIn)
            {
                try
                {
                    var ipcMsgs = from t in Assembly.Load(name.Name).GetTypes()
                                  where t.IsDefined(typeof(SoMIpcAttribute), true)
                                  select t;
                    foreach (Type t in ipcMsgs)
                    {
                        SoMIpcAttribute attr = t.GetCustomAttribute<SoMIpcAttribute>();
                        var Conflicting = Used.Where((p) => p.Value.ToString() == attr.Discriminator).FirstOrDefault();
                        if (Conflicting.Key != null)
                        {
                            throw new JsonException($"SoMIpcMessage Type Discriminator `{attr.Discriminator}` already in use by type {Conflicting.Key}");
                        }
                        JsonDerivedType jdt = new JsonDerivedType(t, attr.Discriminator);
                        Used[t] = attr.Discriminator;
                        Result.Add(jdt);
                    }
                } catch ( ReflectionTypeLoadException tlex)
                {
                    if (logger != null)
                        logger.LogWarning($"SoMIpc failed to type-check assembly {String.Join<Exception>(" ; ", tlex.LoaderExceptions)}");
                    else
                    {
                        System.Diagnostics.Trace.WriteLine($"SoMIpc failed to type-check assembly {String.Join<Exception>(" ; ", tlex.LoaderExceptions)}");
                    }
                }
            }
            return Result.ToArray();
        }


    }
}
