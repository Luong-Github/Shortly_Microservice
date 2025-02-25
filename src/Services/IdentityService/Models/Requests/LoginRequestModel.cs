using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models.Requests
{
    public class LoginRequestModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
