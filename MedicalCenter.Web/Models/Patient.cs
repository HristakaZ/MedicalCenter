using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Models
{
    public class Patient : User
    {
        public int SSN { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public virtual List<MedicalExamination> MedicalExaminations { get; set; }

        public virtual Doctor Doctor { get; set; }

        public int DoctorID { get; set; }
    }
}
