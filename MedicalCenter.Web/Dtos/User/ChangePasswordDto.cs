using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.User
{
    public class ChangePasswordDto
    {
        public int UserID { get; set; }

        [Display(Name = "Текуща парола")]
        [Required(ErrorMessage = "Текущата парола е задължителна.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Новата парола е задължителна.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$", ErrorMessage = "Паролата трябва да съдържа 1 число, 1 малка буква, 1 главна буква, един специален символ и да е поне 8 символа.")]
        [Display(Name = "Нова парола")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "'Потвърди нова парола' е задължително.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$", ErrorMessage = "Паролата трябва да съдържа 1 число, 1 малка буква, 1 главна буква, един специален символ и да е поне 8 символа.")]
        [Compare(nameof(NewPassword), ErrorMessage = "'Нова парола' и 'Потвърди нова парола' не съвпадат.")]
        [Display(Name = "Потвърди нова парола")]
        public string ConfirmNewPassword { get; set; }
    }
}
