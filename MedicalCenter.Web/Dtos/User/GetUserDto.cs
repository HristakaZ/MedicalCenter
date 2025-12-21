using MedicalCenter.Web.Models;

namespace MedicalCenter.Web.Dtos.User
{
    public class GetUserDto
    {
        public int ID { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public Role Role { get; set; }
    }
}
