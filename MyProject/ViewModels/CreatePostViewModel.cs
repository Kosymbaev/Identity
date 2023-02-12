using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MyProject.ViewModels
{
    public class CreatePostViewModel
    {
        [Required(ErrorMessage = "Не указан Title")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Не указан Description")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        public IFormFile Photo { get; set; }
    }
}
