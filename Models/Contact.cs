using System.ComponentModel.DataAnnotations;

namespace ContactsApp.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }
    }
}