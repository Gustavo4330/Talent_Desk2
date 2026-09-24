using System.ComponentModel.DataAnnotations;

namespace TalentDesk.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;

        [Display(Name = "WhatsApp")]
        public string? Whatsapp { get; set; }

        [Display(Name = "Data de nascimento")]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        [Display(Name = "Foto URL")]
        [DataType(DataType.Url)]
        public string? FotoUrl { get; set; }
    }
}
