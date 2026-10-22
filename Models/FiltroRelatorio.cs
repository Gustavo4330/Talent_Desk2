namespace TalentDesk.Models
{
    // Filtros da tela de relatórios (ex.: ?periodo=30&area=Design)
    public class FiltroRelatorio
    {
        public const string PeriodoTudo = "tudo";

        public static readonly IReadOnlyList<(string Valor, string Rotulo, int Dias)> OpcoesPeriodo = new[]
        {
            (PeriodoTudo, "Todo o período", 0),
            ("7", "Últimos 7 dias", 7),
            ("30", "Últimos 30 dias", 30),
            ("90", "Últimos 90 dias", 90),
            ("365", "Últimos 12 meses", 365)
        };

        public string Periodo { get; set; } = PeriodoTudo;
        public string? Area { get; set; }

        // Valor desconhecido na URL vale como "todo o período" (o ASP.NET lê estas
        // propriedades ao validar, antes mesmo de Normalizar() ser chamado)
        private (string Valor, string Rotulo, int Dias) OpcaoAtual =>
            OpcoesPeriodo.FirstOrDefault(o => o.Valor == Periodo, OpcoesPeriodo[0]);

        // 0 = todo o período
        public int Dias => OpcaoAtual.Dias;

        public string RotuloPeriodo => OpcaoAtual.Rotulo;

        public int QuantidadeAtivos => (Dias != 0 ? 1 : 0) + (string.IsNullOrWhiteSpace(Area) ? 0 : 1);

        public void Normalizar()
        {
            if (!OpcoesPeriodo.Any(o => o.Valor == Periodo))
            {
                Periodo = PeriodoTudo;
            }
        }
    }
}
