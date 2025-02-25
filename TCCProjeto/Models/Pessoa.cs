using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TCCProjeto.Models
{
    public class Pessoa : IdentityUser
    {
        [Required, MaxLength(80, ErrorMessage = "Nome não pode exceder 80 caracteres!!")]
        public string? Nome { get; set; }
        public double Pontos { get; set; }
    }
}
    