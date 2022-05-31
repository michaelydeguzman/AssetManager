using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application
{
    public class HandlerResult<T> where T : class
    {
        public T Entity { get; set; }
        public ResultType ResultType { get; set; }
        public string Message { get; set; }
    }

    public enum ResultType
    {
        Fail,
        BadRequest,
        NoData,
        Success
    }
}
