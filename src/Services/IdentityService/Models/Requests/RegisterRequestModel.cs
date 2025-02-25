using Microsoft.AspNetCore.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models.Requests
{
    public sealed class RegisterRequestModel
    {
        [Required]
        public string Email { get;set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
