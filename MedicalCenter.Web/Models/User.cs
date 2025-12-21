using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Models
{
    public class User : BaseEntity
    {
        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Password { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Surname { get; set; }

        public Role Role { get; set; }
    }
}
