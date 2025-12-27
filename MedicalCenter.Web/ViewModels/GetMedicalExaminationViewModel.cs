namespace MedicalCenter.Web.ViewModels
{
    public class GetMedicalExaminationViewModel
    {
        public int ID { get; set; }

        public string? Diagnosis { get; set; }

        public string? Recommendation { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string PatientName { get; set; }

        public string DoctorName { get; set; }

        public string DoctorSpecialty { get; set; }
    }
}
