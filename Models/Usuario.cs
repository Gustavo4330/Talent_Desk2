using System;
using System.ComponentModel.DataAnnotations;

namespace TalentDesk.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe seu nome.")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "Informe seu email.")]
        [EmailAddress(ErrorMessage = "Informe um email válido.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Informe sua senha.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        public string Senha { get; set; } = "";

        public string? Whatsapp { get; set; }

        public DateTime? DataNascimento { get; set; }

        public string? FotoUrl { get; set; }
    }
}