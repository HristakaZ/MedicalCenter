using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.User
{
    public class EditDoctorDto : EditUserDto
    {
        [Display(Name = "Стая")]
        [Required(ErrorMessage = "Стаята е задължителна")]
        public int Room { get; set; }

        [Display(Name = "Специалност")]
        [Required(ErrorMessage = "Специалността е задължителна")]
        public string SelectedSpecialty { get; set; }

        [Display(Name = "Специалности")]
        public List<SelectListItem> Specialties { get; set; } = new List<SelectListItem>();

        [Display(Name = "Роля")]
        public string? SelectedRole { get; set; }

        [Display(Name = "Роли")]
        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
    }
}
