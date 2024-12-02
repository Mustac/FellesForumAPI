using System.ComponentModel.DataAnnotations;

namespace FellesForumAPI.Models.DTO
{
    public class RegisterUserDto : LoginPhoneDto
    {
        [Required]
        [StringLength(maximumLength:30, MinimumLength = 2, ErrorMessage = "Name must be from 2 to 30 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "Name must be from 2 to 30 characters")]
        public string? LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
