namespace TalentDesk.Models
{
    public class EstatisticasDashboard
    {
        public int TotalCurriculos { get; set; }
        public int TotalNoInicioDoMes { get; set; }

        public int NovosEstaSemana { get; set; }
        public int NovosSemanaAnterior { get; set; }

        public int Favoritos { get; set; }
        public int EmProcessoSeletivo { get; set; }

        // Variação percentual em relação ao mês anterior (null quando não dá para calcular)
        public int? VariacaoTotal => CalcularVariacao(TotalCurriculos, TotalNoInicioDoMes);

        // Variação percentual em relação à semana anterior
        public int? VariacaoNovos => CalcularVariacao(NovosEstaSemana, NovosSemanaAnterior);

        private static int? CalcularVariacao(int atual, int anterior)
        {
            if (anterior <= 0) return null;
            return (int)Math.Round((atual - anterior) * 100.0 / anterior);
        }
    }
}
