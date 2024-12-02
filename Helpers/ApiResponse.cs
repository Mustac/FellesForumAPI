using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FellesForumAPI.Helpers
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public ApiResponse(string message = "")
        {
            Message = message;
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }

        public ApiResponse(T data, string message = "") : base(message)
        {
            Data = data;
        }

        public ApiResponse(string message = "") : base(message)
        {
            Data = default;
        }

    }
}
