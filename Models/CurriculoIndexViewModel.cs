namespace TalentDesk.Models
{
    // Tudo que a tela inicial (dashboard) precisa para ser desenhada
    public class CurriculoIndexViewModel
    {
        public FiltroCurriculo Filtro { get; set; } = new();

        // Página atual da tabela
        public List<Curriculo> Curriculos { get; set; } = new();
        public int TotalEncontrado { get; set; }

        // Painel da direita
        public Curriculo? Selecionado { get; set; }

        // Cards do topo
        public EstatisticasDashboard Estatisticas { get; set; } = new();

        // Opções dos filtros
        public List<string> Areas { get; set; } = new();
        public List<string> Formacoes { get; set; } = new();
        public List<string> Cidades { get; set; } = new();

        public HashSet<int> FavoritosIds { get; set; } = new();

        public bool EhFavorito(int curriculoId) => FavoritosIds.Contains(curriculoId);

        // ===== Paginação =====

        public int TotalPaginas => Math.Max(1, (int)Math.Ceiling(TotalEncontrado / (double)Filtro.Itens));

        public int PrimeiroItem => TotalEncontrado == 0 ? 0 : (Filtro.Pagina - 1) * Filtro.Itens + 1;

        public int UltimoItem => Math.Min(Filtro.Pagina * Filtro.Itens, TotalEncontrado);

        // Números que aparecem na paginação. null = "..."
        // Ex.: 1 2 3 4 5 ... 250
        public List<int?> PaginasVisiveis()
        {
            var paginas = new List<int?>();
            int total = TotalPaginas;
            int atual = Filtro.Pagina;

            if (total <= 7)
            {
                for (int i = 1; i <= total; i++) paginas.Add(i);
                return paginas;
            }

            int inicio = Math.Max(2, atual - 1);
            int fim = Math.Min(total - 1, atual + 1);

            if (atual <= 4) { inicio = 2; fim = 5; }
            if (atual >= total - 3) { inicio = total - 4; fim = total - 1; }

            paginas.Add(1);
            if (inicio > 2) paginas.Add(null);
            for (int i = inicio; i <= fim; i++) paginas.Add(i);
            if (fim < total - 1) paginas.Add(null);
            paginas.Add(total);

            return paginas;
        }

        // ===== Textos da tela =====

        public string Titulo => Filtro.Visao switch
        {
            FiltroCurriculo.VisaoFavoritos => "Favoritos",
            FiltroCurriculo.VisaoCurriculos => "Todos os currículos",
            _ => "Currículos recentes"
        };

        public string Subtitulo => Filtro.Visao switch
        {
            FiltroCurriculo.VisaoFavoritos => "Currículos que você salvou para acompanhar de perto.",
            FiltroCurriculo.VisaoCurriculos => "Pesquise e filtre todos os currículos cadastrados.",
            _ => "Confira os currículos mais recentes cadastrados na plataforma."
        };
    }
}
