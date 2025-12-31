using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.ViewModels.MedicalExamination
{
    public class GetMedicalExaminationViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Диагноза")]
        public string? Diagnosis { get; set; }

        [Display(Name = "Препоръка")]
        public string? Recommendation { get; set; }

        [Display(Name = "Начален час")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Краен час")]
        public DateTime EndTime { get; set; }

        [Display(Name = "Име на пациент")]
        public string PatientName { get; set; }

        [Display(Name = "Име на доктор")]
        public string DoctorName { get; set; }

        [Display(Name = "Специалност")]
        public string DoctorSpecialty { get; set; }
    }
}
