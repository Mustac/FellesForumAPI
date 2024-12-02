using FellesForumAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace FellesForumAPI.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly ApplicationDbContext _db;

        protected BaseController(ApplicationDbContext db)
        {
            _db = db;
        }


        // Utilized across endpoints to eliminate repetitive error handling for internal server errors.
        protected async Task<IActionResult> ApiCall(Func<Task<IActionResult>> func, string internalErrorText = "An unexpected error occurred. Please try again later.")
        {
            try
            {
                return await func.Invoke();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, internalErrorText);
            }
        }
    }
}
