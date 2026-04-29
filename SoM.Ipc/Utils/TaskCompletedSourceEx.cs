using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SoM
{
    /// <summary>
    /// Allows  awaiting for completion of
    /// commands sent to the command queue + adds timeout for
    /// remote execution calls. Used internally. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskCompletionSourceEx<T> : TaskCompletionSource<T>
    {
        public TimeSpan Timeout { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public TaskCompletionSourceEx() : base() { }

        public TaskCompletionSourceEx(T value) : base(value) { }
    }

}
