namespace MedicalCenter.Web.Models
{
    public class Doctor : User
    {
        public int Room { get; set; }

        public virtual Specialty Specialty { get; set; }

        public int SpecialtyID { get; set; }

        public virtual List<Patient> Patients { get; set; }

        public virtual List<MedicalExamination> MedicalExaminations { get; set; }
    }
}
