namespace MedicalCenter.Web.Models
{
    public class Doctor : User
    {
        public int Room { get; set; }

        public Specialty Specialty { get; set; }

        public List<Patient> Patients { get; set; }

        public List<MedicalExamination> MedicalExaminations { get; set; }
    }
}
