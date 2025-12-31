using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.Specialty
{
    public class EditSpecialtyDto
    {
        public int ID { get; set; }

        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Описанието е задължително.")]
        public string Description { get; set; }
    }
}
