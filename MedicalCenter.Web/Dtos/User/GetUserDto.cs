using MedicalCenter.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.User
{
    public class GetUserDto
    {
        public int ID { get; set; }

        [Display(Name = "Имейл")]
        public string Email { get; set; }

        [Display(Name = "Име")]
        public string Name { get; set; }

        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Display(Name = "Роля")]
        public Role Role { get; set; }
    }
}
