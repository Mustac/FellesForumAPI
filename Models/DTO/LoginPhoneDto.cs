using System.ComponentModel.DataAnnotations;

namespace FellesForumAPI.Models.DTO;

public class LoginPhoneDto
{
    [Required(ErrorMessage = "Phone number is required.")]
    [Range(10000000, 9999999999, ErrorMessage = "Phone has to be 8 to 10 digits long.")]
    public int? Phone { get; set; }

}
