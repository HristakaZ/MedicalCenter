using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Models
{
    public class Patient : User
    {
        public int SSN { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public List<MedicalExamination> MedicalExaminations { get; set; }

        public List<Doctor> Doctors { get; set; }
    }
}
