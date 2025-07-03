using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ContactsApp.Models
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Profile Image URL")]
        public string ProfileImageUrl { get; set; }

        [Display(Name = "Upload New Image")]
        public IFormFile ProfileImageFile { get; set; }

        // Password change
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
