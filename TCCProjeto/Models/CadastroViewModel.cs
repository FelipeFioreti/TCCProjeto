using System.ComponentModel.DataAnnotations;

namespace TCCProjeto.Models
{
    public class CadastroViewModel
    {
        [Required(ErrorMessage = "O Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Confirmar o Email é obrigatório")]
        [EmailAddress]
        [Compare("Email",ErrorMessage = "Os Emails não são compatíveis")]
        public string? ConfirmEmail { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório")]
        [MaxLength(80,ErrorMessage = "O nome não pode excerder 80 caracteres")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "A Senha é obrigatória")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Confirmar a Senha é obrigatório")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As senhas não são compatíveis")]
        public string? ConfirmPassword { get; set; }
        [Required(ErrorMessage = "O Termo é obrigatório.")]
        public bool Termos { get; set; }

    }
}