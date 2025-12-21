using MedicalCenter.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.User
{
    public class EditUserDto
    {
        public int ID { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [MaxLength(100)]
        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$", ErrorMessage = "The password must contain 1 number, 1 lowercase letter, 1 uppercase letter and 1 special character and must be at least 8 characters long.")]
        public string Password { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [MaxLength(50)]
        [Required]
        public string Surname { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}
