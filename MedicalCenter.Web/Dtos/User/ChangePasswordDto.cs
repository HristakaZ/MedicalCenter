using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.User
{
    public class ChangePasswordDto
    {
        public int UserID { get; set; }

        public string CurrentPassword { get; set; }

        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$", ErrorMessage = "The password must contain 1 number, 1 lowercase letter, 1 uppercase letter and 1 special character and must be at least 8 characters long.")]
        public string NewPassword { get; set; }
    }
}
