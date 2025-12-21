using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Models
{
    public class Specialty : BaseEntity
    {
        [MaxLength(50)]
        public string Description { get; set; }

        public List<Doctor> Doctors { get; set; }
    }
}
