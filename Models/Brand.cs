using Microsoft.AspNetCore.Routing.Constraints;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Brand
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name{ get; set; }

        [Display(Name="Established Year")] //Display name for the form is this , instead of EstablishedYear
        public int EstablishedYear { get; set; }

        [Display(Name = "Brand Logo")] //Display name for the form is this , instead of BrandLogo
        public string BrandLogo { get; set; }   

    }
}
