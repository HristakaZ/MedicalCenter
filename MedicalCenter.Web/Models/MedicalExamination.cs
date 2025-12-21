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

        public Patient Patient { get; set; }

        public Doctor Doctor { get; set; }
    }
}
