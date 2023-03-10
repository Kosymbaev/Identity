using System.ComponentModel.DataAnnotations;


namespace MyProject.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        [Required]
        public string LastPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}