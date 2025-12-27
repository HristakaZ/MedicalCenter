using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.User
{
    public class EditUserDto
    {
        public int ID { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; }

        [MaxLength(50)]
        [Required]
        public string Surname { get; set; }
    }
}
