using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TCCProjeto.Entities
{
    public class Produto
    {
        public int Id { get; set; }
        [Required, MaxLength(80, ErrorMessage = "Nome não pode exceder 80 caracteres!!")]
        public string? Nome { get; set; }
        [Required]
        public string? Item { get; set; }
        [Required]
        public double Valor { get; set; }
        [Required]
        public int Quantidade { get; set; }
        [Required]
        public bool Ativo { get; set; }
    }
}