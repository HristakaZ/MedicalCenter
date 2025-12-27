using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.User
{
    public class EditPatientDto : EditUserDto
    {
        public int SSN { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public string SelectedDoctor { get; set; }

        public List<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();

        public string? SelectedRole { get; set; }

        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
    }
}
