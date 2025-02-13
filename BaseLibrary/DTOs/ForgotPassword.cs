using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTOs
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}