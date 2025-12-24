using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Models
{
    public class MedicalExamination : BaseEntity
    {
        [MaxLength(500)]
        public string? Diagnosis { get; set; }

        [MaxLength(500)]
        public string? Recommendation { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public virtual Patient Patient { get; set; }

        public int PatientID { get; set; }

        public virtual Doctor Doctor { get; set; }

        public int DoctorID { get; set; }
    }
}
