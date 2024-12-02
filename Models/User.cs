using System.ComponentModel.DataAnnotations;

namespace FellesForumAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(maximumLength: 8, MinimumLength = 6)]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string LiveInCity { get; set; } = string.Empty;

        [StringLength(50)]
        public string Street { get; set; } = string.Empty;


        [StringLength(50)]
        public string BuildingNumber { get; set; } = string.Empty;

        [StringLength(50)]
        public string ApartmentLetter { get; set; } = string.Empty;

        public DateTime Created { get; set; }
        public DateTime LastLogin { get; set; }
        public string LastToken { get; set; } = string.Empty;

        public bool IsVerified { get; set; }
        
    }
}
