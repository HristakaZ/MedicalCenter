using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Models
{
    public class User : BaseEntity
    {
        [MaxLength(100)]
        public string Email { get; set; }

        public string Password { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Surname { get; set; }

        public virtual Role Role { get; set; }

        public int RoleID { get; set; }
    }
}
