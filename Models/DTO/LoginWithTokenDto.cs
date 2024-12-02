using System.ComponentModel.DataAnnotations;

namespace FellesForumAPI.Models.DTO
{
    public class LoginWithTokenDto : LoginPhoneDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
