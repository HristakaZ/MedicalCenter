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
    }
}
