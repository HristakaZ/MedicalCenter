using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.User
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "Имейлът е задължителен.")]
        [Display(Name = "Имейл")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Паролата е задължителна.")]
        [Display(Name = "Парола")]
        public string Password { get; set; }
    }
}
