using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.Dtos.Specialty
{
    public class AddSpecialtyDto
    {
        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Описанието е задължително.")]
        public string Description { get; set; }
    }
}
