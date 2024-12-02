using Microsoft.AspNetCore.Mvc;

namespace FellesForumAPI.Helpers
{
    public static class ApiResponseExtensions
    {
        // ----- Non-Generic ApiResponse Methods -----

        /// <summary>
        /// Returns a 200 OK response with the provided ApiResponse.
        /// Use this when the request was successful and no data needs to be returned.
        /// </summary>
        public static IActionResult Success(this ApiResponse response)
        {
            response.StatusCode = 200;
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }

        /// <summary>
        /// Returns a 201 Created response with the provided ApiResponse.
        /// Use this when a resource has been successfully created and no data needs to be returned.
        /// </summary>
        public static IActionResult Created(this ApiResponse response)
        {
            response.StatusCode = 201;
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }

        /// <summary>
        /// Returns a 400 Bad Request response with the provided ApiResponse.
        /// Use this when the request cannot be processed due to client-side errors.
        /// </summary>
        public static IActionResult BadRequest(this ApiResponse response)
        {
            response.StatusCode = 400;
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }

        /// <summary>
        /// Returns a 404 Not Found response with the provided ApiResponse.
        /// Use this when the requested resource could not be found.
        /// </summary>
        public static IActionResult NotFound(this ApiResponse response)
        {
            response.StatusCode = 404;
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }

        /// <summary>
        /// Returns a 500 Internal Server Error response with the provided ApiResponse.
        /// Use this for unexpected server errors or unhandled exceptions.
        /// </summary>
        public static IActionResult ServerError(this ApiResponse response, string message = "An unexpected error occurred.")
        {
            response.StatusCode = 500;
            response.Message = message;
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }

        // ----- Generic ApiResponse<T> Methods -----

        /// <summary>
        /// Returns a 200 OK response with the provided ApiResponse containing data.
        /// Use this when the request was successful and has data to return.
        /// </summary>
        public static IActionResult Success<T>(this ApiResponse<T> response)
        {
            response.StatusCode = 200;
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }

        /// <summary>
        /// Returns a 201 Created response with the provided ApiResponse containing data.
        /// Use this when a resource has been successfully created and data is returned.
        /// </summary>
        public static IActionResult Created<T>(this ApiResponse<T> response)
        {
            response.StatusCode = 201;
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }

        /// <summary>
        /// Returns a 400 Bad Request response with the provided ApiResponse containing data.
        /// Use this when the request cannot be processed due to client-side errors.
        /// </summary>
        public static IActionResult BadRequest<T>(this ApiResponse<T> response)
        {
            response.StatusCode = 400;
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }

        /// <summary>
        /// Returns a 404 Not Found response with the provided Ap77iResponse containing data.
        /// Use this when the requested resource could not be found.
        /// </summary>
        public static IActionResult NotFound<T>(this ApiResponse<T> response)
        {
            response.StatusCode = 404;
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }

        /// <summary>
        /// Returns a 500 Internal Server Error response with the provided ApiResponse containing data.
        /// Use this for unexpected server errors or unhandled exceptions.
        /// </summary>
        public static IActionResult ServerError<T>(this ApiResponse<T> response, string message = "An unexpected error occurred.")
        {
            response.StatusCode = 500;
            response.Message = message;
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}
