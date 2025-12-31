using System.ComponentModel.DataAnnotations;

namespace MedicalCenter.Web.ViewModels.Specialty
{
    public class GetSpecialtyViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }
    }
}
