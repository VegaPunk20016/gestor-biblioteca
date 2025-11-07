using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadHub.Application.Common
{
    public class Result
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }

        public static Result Ok(object? data = null, string? message = null)
            => new Result { Success = true, Message = message, Data = data };

        public static Result Fail(string message)
            => new Result { Success = false, Message = message };
    }
}
