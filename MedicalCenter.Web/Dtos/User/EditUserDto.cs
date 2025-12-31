using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.User
{
    public class EditUserDto
    {
        public int ID { get; set; }

        [MaxLength(50, ErrorMessage = "Името не може да надвишава 50 символа.")]
        [Required(ErrorMessage = "Името е задължително.")]
        [Display(Name = "Име")]
        public string Name { get; set; }

        [MaxLength(50, ErrorMessage = "Фамилията не може да надвишава 50 символа.")]
        [Required(ErrorMessage = "Фамилията е задължителна.")]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }
    }
}
