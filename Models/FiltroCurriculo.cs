namespace TalentDesk.Models
{
    // Um filtro ligado, mostrado como "chip" acima da tabela
    public record FiltroAtivo(string Chave, string Rotulo, string Valor);

    // Tudo que vem na URL da tela de currículos (ex.: ?busca=ana&area=Design&pagina=2)
    public class FiltroCurriculo
    {
        public const string VisaoDashboard = "dashboard";
        public const string VisaoCurriculos = "curriculos";
        public const string VisaoFavoritos = "favoritos";

        public static readonly int[] OpcoesItensPorPagina = { 5, 10, 20 };

        public const string OrdemRecentes = "recentes";
        public const string OrdemNome = "nome";
        public const string OrdemMaisExperiencia = "mais-experiencia";
        public const string OrdemMenosExperiencia = "menos-experiencia";

        public static readonly IReadOnlyList<(string Valor, string Rotulo)> OpcoesOrdem = new[]
        {
            (OrdemRecentes, "Mais recentes"),
            (OrdemNome, "Nome (A–Z)"),
            (OrdemMaisExperiencia, "Mais experiência"),
            (OrdemMenosExperiencia, "Menos experiência")
        };

        public string? Busca { get; set; }
        public string? Area { get; set; }
        public string? Formacao { get; set; }
        public string? Experiencia { get; set; }
        public string? Localizacao { get; set; }

        // Valor de uma FaixaIdade (ex.: "26-30")
        public string? Idade { get; set; }
        public string? Status { get; set; }

        public int Pagina { get; set; } = 1;
        public int Itens { get; set; } = 5;

        public string Ordem { get; set; } = OrdemRecentes;

        public string Visao { get; set; } = VisaoDashboard;

        // Currículo aberto no painel da direita (0 = painel fechado)
        public int? Selecionado { get; set; }

        public bool EhFavoritos => Visao == VisaoFavoritos;

        public int QuantidadeFiltrosExtras =>
            (string.IsNullOrWhiteSpace(Idade) ? 0 : 1) + (string.IsNullOrWhiteSpace(Status) ? 0 : 1);

        public bool TemFiltroAtivo =>
            !string.IsNullOrWhiteSpace(Busca) ||
            !string.IsNullOrWhiteSpace(Area) ||
            !string.IsNullOrWhiteSpace(Formacao) ||
            !string.IsNullOrWhiteSpace(Experiencia) ||
            !string.IsNullOrWhiteSpace(Localizacao) ||
            QuantidadeFiltrosExtras > 0;

        // Corrige valores inválidos digitados na URL
        public void Normalizar()
        {
            if (Visao != VisaoCurriculos && Visao != VisaoFavoritos)
            {
                Visao = VisaoDashboard;
            }

            if (!OpcoesItensPorPagina.Contains(Itens))
            {
                Itens = OpcoesItensPorPagina[0];
            }

            if (Pagina < 1)
            {
                Pagina = 1;
            }

            if (!StatusCurriculo.EhValido(Status))
            {
                Status = null;
            }

            if (FaixaExperiencia.ObterPorValor(Experiencia) == null)
            {
                Experiencia = null;
            }

            if (FaixaIdade.ObterPorValor(Idade) == null)
            {
                Idade = null;
            }

            if (!OpcoesOrdem.Any(opcao => opcao.Valor == Ordem))
            {
                Ordem = OrdemRecentes;
            }
        }

        // Filtros ligados com rótulos amigáveis, na ordem em que aparecem na tela
        public List<FiltroAtivo> FiltrosAtivos()
        {
            var ativos = new List<FiltroAtivo>();

            void Adicionar(string chave, string rotulo, string? valor)
            {
                if (!string.IsNullOrWhiteSpace(valor)) ativos.Add(new FiltroAtivo(chave, rotulo, valor));
            }

            Adicionar("busca", "Busca", Busca);
            Adicionar("area", "Área", Area);
            Adicionar("formacao", "Formação", Formacao);
            Adicionar("experiencia", "Experiência", FaixaExperiencia.ObterPorValor(Experiencia)?.Rotulo);
            Adicionar("localizacao", "Localização", Localizacao);
            Adicionar("idade", "Idade", FaixaIdade.ObterPorValor(Idade)?.Rotulo);
            Adicionar("status", "Status", Status);

            return ativos;
        }

        // Monta os parâmetros da URL mantendo os filtros atuais.
        // Usado nos links de paginação, "Ver mais", etc.
        // "sem" tira um filtro da URL (link do × de cada chip).
        public Dictionary<string, string> ParaRota(int? pagina = null, int? selecionado = null, bool manterSelecionado = false, string? sem = null)
        {
            var rota = new Dictionary<string, string>();

            void Adicionar(string chave, string? valor)
            {
                if (!string.IsNullOrWhiteSpace(valor)) rota[chave] = valor;
            }

            if (Visao != VisaoDashboard) Adicionar("visao", Visao);
            Adicionar("busca", Busca);
            Adicionar("area", Area);
            Adicionar("formacao", Formacao);
            Adicionar("experiencia", Experiencia);
            Adicionar("localizacao", Localizacao);
            Adicionar("idade", Idade);
            Adicionar("status", Status);
            if (Ordem != OrdemRecentes) Adicionar("ordem", Ordem);

            int paginaFinal = pagina ?? Pagina;
            if (paginaFinal > 1) Adicionar("pagina", paginaFinal.ToString());
            if (Itens != OpcoesItensPorPagina[0]) Adicionar("itens", Itens.ToString());

            int? selecionadoFinal = selecionado ?? (manterSelecionado ? Selecionado : null);
            if (selecionadoFinal.HasValue) Adicionar("selecionado", selecionadoFinal.Value.ToString());

            if (sem != null) rota.Remove(sem);

            return rota;
        }
    }
}
