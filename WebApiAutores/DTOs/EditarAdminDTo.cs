using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class EditarAdminDTo
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
