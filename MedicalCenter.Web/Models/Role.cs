using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Models
{
    public class Role : BaseEntity
    {
        [MaxLength(50)]
        public string Description { get; set; }

        public List<User> Users { get; set; }
    }
}
