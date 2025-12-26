using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.MedicalExamination
{
    public class EditMedicalExaminationDto
    {
        public int ID { get; set; }

        [MaxLength(500)]
        public string? Diagnosis { get; set; }

        [MaxLength(500)]
        public string? Recommendation { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public string SelectedDoctor { get; set; }

        public List<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();
    }
}
