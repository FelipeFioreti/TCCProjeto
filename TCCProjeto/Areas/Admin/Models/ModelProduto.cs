using System.ComponentModel.DataAnnotations;

namespace TCCProjeto.Areas.Admin.Models
{
    public class ModelProduto
    {
        [Required, MaxLength(80, ErrorMessage = "Nome não pode exceder 80 caracteres!!")]
        public string? Nome { get; set; }
        [Required, MaxLength(50, ErrorMessage = "Item não pode exceder 50 caracteres!!")]
        public string? Item { get; set; }
        [Required]
        public double Valor { get; set; }
        [Required]
        public int Quantidade { get; set; }
        [Required]
        [Display(Name = "Produto disponível?")]
        public bool Ativo { get; set; }
    }
}
