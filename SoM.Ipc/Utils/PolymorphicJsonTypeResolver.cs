using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Xml.Linq;

namespace SoM.Ipc
{
    /// <summary>
    /// Manual Polymorphic Type Resolving for compatibility with Net Standard 2.0
    /// </summary>
    public class PolymorphicJsonTypeResolver : DefaultJsonTypeInfoResolver
    {

        public PolymorphicJsonTypeResolver(IList<JsonDerivedType> derivedTypes) : base()
        {
            if ( derivedTypes != null)
                foreach(JsonDerivedType derivedType in derivedTypes)
                    if ( ! DeridedTypes.Contains(derivedType) )
                        DeridedTypes.Add(derivedType);
        }

        public IList<JsonDerivedType> DeridedTypes { get; set;  } = new List<JsonDerivedType>();

        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

            Type baseType = typeof(SoMIpcMessage);
            if (jsonTypeInfo.Type == baseType)
            {
                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "$type",
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                };
                foreach (JsonDerivedType jdt in DeridedTypes)
                {
                    if ( !jsonTypeInfo.PolymorphismOptions.DerivedTypes.Contains(jdt))
                    jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(jdt);

                }
            }

            return jsonTypeInfo;
        }
    }

}
