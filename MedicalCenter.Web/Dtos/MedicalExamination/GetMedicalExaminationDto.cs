namespace MedicalCenter.Web.Dtos.MedicalExamination
{
    public class GetMedicalExaminationDto
    {
        public int ID { get; set; }

        public string? Diagnosis { get; set; }

        public string? Recommendation { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string PatientName { get; set; }

        public string DoctorName { get; set; }
    }
}
