using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.User
{
    public class EditPatientDto : EditUserDto
    {
        [Display(Name = "ЕГН")]
        [Required(ErrorMessage = "ЕГН-то е задължително.")]
        public int SSN { get; set; }

        [MaxLength(20)]
        [Display(Name = "Телефонен номер")]
        [Required(ErrorMessage = "Телефонният номер е задължителен.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Доктор")]
        [Required(ErrorMessage = "Докторът е задължителен.")]
        public string SelectedDoctor { get; set; }

        [Display(Name = "Доктори")]
        public List<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();

        [Display(Name = "Роля")]
        public string? SelectedRole { get; set; }

        [Display(Name = "Роли")]
        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
    }
}
