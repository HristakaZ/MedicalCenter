using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalCenter.Web.Dtos.User
{
    public class EditDoctorDto : EditUserDto
    {
        public int Room { get; set; }

        public string SelectedSpecialty { get; set; }

        public List<SelectListItem> Specialties { get; set; } = new List<SelectListItem>();

        public string SelectedRole { get; set; }

        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
    }
}
