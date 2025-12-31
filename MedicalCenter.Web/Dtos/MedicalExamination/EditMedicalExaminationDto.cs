using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.MedicalExamination
{
    public class EditMedicalExaminationDto
    {
        public int ID { get; set; }

        [MaxLength(500, ErrorMessage = "Диагнозата не може да надвишава 500 символа.")]
        [Display(Name = "Диагноза")]
        public string? Diagnosis { get; set; }

        [MaxLength(500, ErrorMessage = "Препоръката не може да надвишава 500 символа.")]
        [Display(Name = "Препоръка")]
        public string? Recommendation { get; set; }

        [Display(Name = "Начални дата и час")]
        [Required(ErrorMessage = "Началните дата и час са задължителни.")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Крайни дата и час")]
        [Required(ErrorMessage = "Крайните дата и час са задължителни.")]
        public DateTime EndTime { get; set; }
    }
}
