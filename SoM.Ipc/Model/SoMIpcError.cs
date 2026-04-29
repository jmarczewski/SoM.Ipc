using System;
using System.Text.Json.Serialization;

namespace SoM.Ipc
{
    /* Add as Error property to SoMIpcMessage or derived */

    #region Ipc Errors

    public class SoMIpcError
    {
        [JsonPropertyName("msg")]
        public string Description { get; set; }

        [JsonPropertyName("err_type")]
        public string ErrorType { get; set; } = "n/a";

        public static SoMIpcError Create(string description, Exception ex = null)
        {
            SoMIpcError e = new SoMIpcError()
            {
                Description = description,
            };
            if (ex != null)
                e.ErrorType = ex.GetType().ToString();
            return e;
        }

        public static SoMIpcError Create(Exception ex)
        {
            string description = ex.Message;
            if (description.Length > 100)
                description = description.Substring(0, 97) + "...";
            SoMIpcError e = new SoMIpcError()
            {
                Description = description,
            };
            if (ex != null)
                e.ErrorType = ex.GetType().ToString();
            return e;
        }

    }

    #endregion



    #region  RemoteExecError classes ( helpers )

    public class RemoteExecFailException : Exception
    {
        public RemoteExecFailException() : base("Remoting system reported error on other end.") { }
    }

    #endregion

}
