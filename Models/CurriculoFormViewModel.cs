using System.ComponentModel.DataAnnotations;

namespace TalentDesk.Models
{
    public class ExperienciaFormItem
    {
        [StringLength(150)]
        public string? Cargo { get; set; }

        [StringLength(150)]
        public string? Empresa { get; set; }

        [StringLength(60)]
        public string? Periodo { get; set; }

        [StringLength(1000)]
        public string? Descricao { get; set; }

        public bool EstaVazia =>
            string.IsNullOrWhiteSpace(Cargo) && string.IsNullOrWhiteSpace(Empresa);
    }

    // Formulário de cadastro e edição de currículo
    public class CurriculoFormViewModel
    {
        public const int QuantidadeExperiencias = 3;

        public int? Id { get; set; }

        [Required(ErrorMessage = "Informe o nome.")]
        [StringLength(150)]
        [Display(Name = "Nome completo")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o cargo.")]
        [StringLength(150)]
        [Display(Name = "Cargo")]
        public string Cargo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a área de atuação.")]
        [StringLength(100)]
        [Display(Name = "Área de atuação")]
        public string Area { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a idade.")]
        [Range(14, 100, ErrorMessage = "A idade deve estar entre 14 e 100 anos.")]
        [Display(Name = "Idade")]
        public int? Idade { get; set; }

        [Required(ErrorMessage = "Informe os anos de experiência.")]
        [Range(0, 60, ErrorMessage = "Informe um valor entre 0 e 60.")]
        [Display(Name = "Anos de experiência")]
        public int? AnosExperiencia { get; set; }

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [StringLength(150)]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [StringLength(30)]
        [Display(Name = "Telefone")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Informe a cidade.")]
        [StringLength(100)]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "LinkedIn")]
        public string? Linkedin { get; set; }

        [StringLength(2000)]
        [Display(Name = "Resumo profissional")]
        public string? Resumo { get; set; }

        [Required(ErrorMessage = "Informe o curso.")]
        [StringLength(150)]
        [Display(Name = "Curso")]
        public string Formacao { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Formação completa")]
        public string? FormacaoDetalhada { get; set; }

        [StringLength(150)]
        [Display(Name = "Instituição")]
        public string? Instituicao { get; set; }

        [StringLength(50)]
        [Display(Name = "Conclusão")]
        public string? ConclusaoFormacao { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = StatusCurriculo.Novo;

        [StringLength(1000)]
        [Display(Name = "Competências")]
        public string? Habilidades { get; set; }

        public List<ExperienciaFormItem> Experiencias { get; set; } = new();

        // Foto atual (só para exibir na edição)
        public string? FotoUrl { get; set; }

        // Opções de sugestão nos campos
        public List<string> SugestoesAreas { get; set; } = new();
        public List<string> SugestoesFormacoes { get; set; } = new();
        public List<string> SugestoesCidades { get; set; } = new();

        public bool EhEdicao => Id.HasValue && Id > 0;

        // Garante sempre 3 blocos de experiência no formulário
        public void CompletarExperiencias()
        {
            while (Experiencias.Count < QuantidadeExperiencias)
            {
                Experiencias.Add(new ExperienciaFormItem());
            }
        }

        public static CurriculoFormViewModel DeCurriculo(Curriculo c)
        {
            var form = new CurriculoFormViewModel
            {
                Id = c.Id,
                Nome = c.Nome,
                Cargo = c.Cargo,
                Area = c.Area,
                Idade = c.Idade,
                AnosExperiencia = c.AnosExperiencia,
                Email = c.Email,
                Telefone = c.Telefone,
                Cidade = c.Cidade,
                Linkedin = c.Linkedin,
                Resumo = c.Resumo,
                Formacao = c.Formacao,
                FormacaoDetalhada = c.FormacaoDetalhada,
                Instituicao = c.Instituicao,
                ConclusaoFormacao = c.ConclusaoFormacao,
                Status = c.Status,
                FotoUrl = c.FotoUrl,
                Habilidades = string.Join(", ", c.Habilidades),
                Experiencias = c.Experiencias.Select(e => new ExperienciaFormItem
                {
                    Cargo = e.Cargo,
                    Empresa = e.Empresa,
                    Periodo = e.Periodo,
                    Descricao = e.Descricao
                }).ToList()
            };

            form.CompletarExperiencias();
            return form;
        }

        public Curriculo ParaCurriculo()
        {
            return new Curriculo
            {
                Id = Id ?? 0,
                Nome = Nome.Trim(),
                Cargo = Cargo.Trim(),
                Area = Area.Trim(),
                Idade = Idade ?? 0,
                AnosExperiencia = AnosExperiencia ?? 0,
                Email = Email.Trim(),
                Telefone = Telefone?.Trim() ?? string.Empty,
                Cidade = Cidade.Trim(),
                Linkedin = Linkedin?.Trim() ?? string.Empty,
                Resumo = Resumo?.Trim() ?? string.Empty,
                Formacao = Formacao.Trim(),
                FormacaoDetalhada = FormacaoDetalhada?.Trim() ?? string.Empty,
                Instituicao = Instituicao?.Trim() ?? string.Empty,
                ConclusaoFormacao = ConclusaoFormacao?.Trim() ?? string.Empty,
                Status = StatusCurriculo.EhValido(Status) ? Status : StatusCurriculo.Novo,
                FotoUrl = FotoUrl ?? string.Empty,
                Habilidades = (Habilidades ?? string.Empty)
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(30)
                    .ToList(),
                Experiencias = Experiencias
                    .Where(e => !e.EstaVazia)
                    .Select(e => new ExperienciaProfissional
                    {
                        Cargo = e.Cargo?.Trim() ?? string.Empty,
                        Empresa = e.Empresa?.Trim() ?? string.Empty,
                        Periodo = e.Periodo?.Trim() ?? string.Empty,
                        Descricao = e.Descricao?.Trim() ?? string.Empty
                    })
                    .ToList()
            };
        }
    }
}
