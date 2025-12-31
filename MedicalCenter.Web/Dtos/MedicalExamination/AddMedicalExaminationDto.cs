using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.MedicalExamination
{
    public class AddMedicalExaminationDto
    {
        [Display(Name = "Начални дата и час")]
        [Required(ErrorMessage = "Началните дата и час са задължителни.")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Крайни дата и час")]
        [Required(ErrorMessage = "Крайните дата и час са задължителни.")]
        public DateTime EndTime { get; set; }
    }
}
