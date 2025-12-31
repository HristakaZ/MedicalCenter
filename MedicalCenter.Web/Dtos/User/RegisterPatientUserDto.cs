using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.User
{
    public class RegisterPatientUserDto
    {
        [MaxLength(100, ErrorMessage = "Имейлът не може да надвишава 100 символа.")]
        [EmailAddress(ErrorMessage = "Въведеният имейл не е валиден имейл адрес.")]
        [Required(ErrorMessage = "Имейлът е задължителен.")]
        [Display(Name = "Имейл")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Паролата е задължителна.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$", ErrorMessage = "Паролата трябва да съдържа 1 число, 1 малка буква, 1 главна буква, един специален символ и да е поне 8 символа.")]
        [Display(Name = "Парола")]
        public string Password { get; set; }

        [MaxLength(50, ErrorMessage = "Паролата не може да надвишава 50 символа.")]
        [Required(ErrorMessage = "Името е задължително.")]
        [Display(Name = "Име")]
        public string Name { get; set; }

        [MaxLength(50, ErrorMessage = "Фамилията не може да надвишава 50 символа.")]
        [Required(ErrorMessage = "Фамилията е задължителна.")]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Display(Name = "ЕГН")]
        [Required(ErrorMessage = "ЕГН-то е задължително.")]
        public int SSN { get; set; }

        [MaxLength(20, ErrorMessage = "Телефонният номер не може да надвишава 20 символа.")]
        [Display(Name = "Телефонен номер")]
        [Required(ErrorMessage = "Телефонният номер е задължителен.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Доктор")]
        [Required(ErrorMessage = "Докторът е задължителен.")]
        public string SelectedDoctor { get; set; }

        [Display(Name = "Доктори")]
        public List<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();
    }
}
