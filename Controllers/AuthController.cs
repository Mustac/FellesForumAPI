using FellesForumAPI.Data;
using FellesForumAPI.Helpers;
using FellesForumAPI.Models;
using FellesForumAPI.Models.DTO;
using FellesForumAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FellesForumAPI.Controllers
{
    //[Authorize]d
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        int timeToResendSms = 30;
        private readonly UserService _userService;

        public AuthController(ApplicationDbContext db, UserService userService) : base(db)
        {
            _userService = userService;
        }


        //[HttpPost("login/phone")]
        //[EnableRateLimiting("IpRateLimit")]
        //public async Task<IActionResult> LoginPhoneAsync([FromBody] LoginPhoneModel userPhone, [FromServices] CaptchaService captchaService)
        //{
        //    // Step 1: Verify the reCAPTCHA token
        //    var isCaptchaValid = await captchaService.VerifyCaptchaAsync(userPhone.CaptchaToken);
        //    if (!isCaptchaValid)
        //    {
        //        return new ApiResponse<string>(message: "CAPTCHA verification failed.").BadRequest();
        //    }
            
        //    // Step 2: Find or create the user
        //    var outcomeUserFind = await _userService.GetUserAsync(userPhone.Phone);
        //    User user = outcomeUserFind.Data;

        //    // If user does not exist, create a new user
        //    if (!outcomeUserFind.IsSuccessful)
        //    {
        //        var outcomeCreateUser = await _userService.CreateUserAsync(userPhone.Phone);

        //        if (outcomeCreateUser.IsSuccessful)
        //            user = outcomeCreateUser.Data;
        //        else
        //            return new ApiResponse<string>(message: outcomeCreateUser.Message).ServerError();
        //    }
        //    // If user exists, update last login time
        //    else if (outcomeUserFind.IsSuccessful)
        //    {
        //        var outcomeUpdateUser = await _userService.UpdateUserLoginTimeAsync(user);

        //        if (outcomeUpdateUser.IsSuccessful)
        //            user = outcomeUpdateUser.Data;
        //        else
        //            return new ApiResponse<string>(message: outcomeUpdateUser.Message).ServerError();
        //    }

        //    // Step 3: Send SMS token for verification (replace with actual SMS logic)
        //    //var token = CreateToken();  // Generate a random token for SMS
        //    //var outcomeSendSms = await _smsService.SendSmsAsync(user.Phone, token);

        //    //if (!outcomeSendSms.IsSuccessful)
        //    //{
        //    //    return new ApiResponse<string>(message: "Failed to send SMS token.").ServerError();
        //    //}

        //    return new ApiResponse<string>(message: "A verification token has been sent to the user's phone.").Success();
        //}



        [HttpGet("check/phone")]
        public async Task<IActionResult> CheckIfUserExistsAsync([FromQuery] string phone)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Phone == phone);

            if (user != null)
            {
                return Ok(new { message = user.IsVerified?"User is registered and verified":"User is registrated but not verified", userId = user.Id });
            }

            return NotFound(new { message = "User is not registered" });
        }


      
    }
}
