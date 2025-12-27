using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.User
{
    public class RegisterPatientUserDto
    {
        [MaxLength(100)]
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$", ErrorMessage = "The password must contain 1 number, 1 lowercase letter, 1 uppercase letter and 1 special character and must be at least 8 characters long.")]
        public string Password { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [MaxLength(50)]
        [Required]
        public string Surname { get; set; }

        public int SSN { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public string SelectedDoctor { get; set; }

        public List<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();
    }
}
