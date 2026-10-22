using TalentDesk.Repositories;

namespace TalentDesk.Models
{
    // Uma barra do gráfico "Evolução dos cadastros".
    // Variacao = % em relação à barra anterior (null quando a anterior é zero).
    public record BarraEvolucao(string Rotulo, string Dica, int Quantidade, int? Variacao = null);

    // Uma bolinha do mapa: os candidatos de uma cidade
    public record PontoMapa(string Cidade, string Uf, double X, double Y, int Quantidade);

    public class RelatoriosViewModel
    {
        public FiltroRelatorio Filtro { get; set; } = new();
        public List<string> Areas { get; set; } = new();

        // ===== Indicadores do topo =====
        public int Total { get; set; }

        // Cadastros do período x período anterior de mesmo tamanho
        // (em "todo o período" compara os últimos 30 dias com os 30 anteriores)
        public int? Crescimento { get; set; }
        public string RotuloCrescimento { get; set; } = string.Empty;

        public int EmProcesso { get; set; }
        public int Aprovados { get; set; }
        public int Reprovados { get; set; }
        public double MediaExperiencia { get; set; }
        public double MediaIdade { get; set; }

        public int PercentualEmProcesso => Total == 0 ? 0 : (int)Math.Round(EmProcesso * 100.0 / Total);

        // Aprovados entre os que já tiveram resposta (aprovado ou reprovado)
        public int? TaxaAprovacao => Aprovados + Reprovados == 0
            ? null
            : (int)Math.Round(Aprovados * 100.0 / (Aprovados + Reprovados));

        // ===== Semáforo de cada indicador (regras em MetasDashboard) =====
        public NivelIndicador NivelCrescimento => MetasDashboard.Crescimento.Classificar(Crescimento);
        public NivelIndicador NivelEmProcesso => MetasDashboard.EmProcesso.Classificar(Total == 0 ? null : PercentualEmProcesso);
        public NivelIndicador NivelAprovacao => MetasDashboard.TaxaAprovacao.Classificar(TaxaAprovacao);
        public NivelIndicador NivelExperiencia => MetasDashboard.ExperienciaMedia.Classificar(Total == 0 ? null : MediaExperiencia);

        // ===== Gráficos =====
        public string TituloEvolucao { get; set; } = string.Empty;
        public List<BarraEvolucao> Evolucao { get; set; } = new();
        public List<ItemContagem> Funil { get; set; } = new();
        public List<ItemContagem> PorArea { get; set; } = new();
        public List<ItemContagem> PorFormacao { get; set; } = new();

        // ===== Mapa =====
        public List<PontoMapa> Pontos { get; set; } = new();
        public List<ItemContagem> PorRegiao { get; set; } = new();
        public int SemLocalizacao { get; set; }
    }
}
