
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace SoM.Ipc
{
    #region Base class for IPC


    /* add JsonDerivedType for every subclass of SoMIpcMessage subclass that is to be sent over IPC here */
    /*  ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩ ⇩  */
    // ie.  [JsonDerivedType(typeof(SoMIpcMessage<MyAppIpcMsg), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(SoMIpcMessage), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(Ping), typeDiscriminator: "_ping")]

    [JsonPolymorphic(
    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    public class SoMIpcMessage
    {

        #region Properties: Ipc, IpcName (Required) + IpcError (optional)

        [JsonPropertyName("two_way")]
        public bool TwoWay { get; set; } = false;

        [JsonPropertyName("ipc_ep")] 
        public string Endpoint { get; set; } = null;

        [JsonPropertyName("ipc_error")]
        public SoMIpcError IpcError { get; set; }

        /// <summary>
        /// Adds command timeout + allows await on SoMIpcMessage commands 
        /// </summary>
        [JsonIgnore]
        public TaskCompletionSourceEx<SoMIpcMessage> CompletedToken { get; set; }

        /// <summary>
        /// Net. Framework compatibility to allow polymorphic json serialization on custom message types
        /// </summary>
        [JsonIgnore]
        public static DefaultJsonTypeInfoResolver TypeResolver { get; set; } = null;


        #endregion

        #region   Serialization / Deserialization to JSON  ( works with all derived classes deserializing to derived class )
      
        public virtual string Serialize()
        {
            JsonSerializerOptions options = new JsonSerializerOptions() {};
            if ( TypeResolver != null)
                options.TypeInfoResolver = TypeResolver;


            var json = JsonSerializer.Serialize(this, options);
            return json;
        }


        public static SoMIpcMessage Deserialize(byte[] json)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions(){};
                if (TypeResolver != null)
                    options.TypeInfoResolver = TypeResolver;
                var obj = JsonSerializer.Deserialize<SoMIpcMessage>(
                    Encoding.UTF8.GetString(json), options);
                return obj;
            } catch ( Exception ex) {
                return new SoMIpcMessage()
                {
                    IpcError = new SoMIpcError()
                    {
                        ErrorType = $"{ex.GetType()}",
                        Description = $"payload: {json.Length} bytes, message: {ex.Message}"
                    }

                };

            }
        }

        public static SoMIpcMessage Deserialize(string json)
        {
            JsonSerializerOptions options = new JsonSerializerOptions() { };
            if (TypeResolver != null)
                options.TypeInfoResolver = TypeResolver;
            
            var obj = JsonSerializer.Deserialize<SoMIpcMessage>(json, options);
            return obj;
        }

    
        #endregion
    }

    #endregion

   

}
