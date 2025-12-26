using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalCenter.Web.Dtos.MedicalExamination
{
    public class AddMedicalExaminationDto
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string SelectedDoctor { get; set; }

        public List<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();
    }
}
