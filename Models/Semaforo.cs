namespace TalentDesk.Models
{
    public enum NivelIndicador { Neutro, Ruim, Atencao, Bom }

    // Uma meta do dashboard: abaixo de "Minimo" é ruim (vermelho),
    // entre "Minimo" e "Ideal" é atenção (amarelo) e acima de "Ideal" é bom (verde).
    public record Meta(double Minimo, double Ideal, string Unidade)
    {
        public NivelIndicador Classificar(double? valor)
        {
            if (valor == null) return NivelIndicador.Neutro;
            if (valor > Ideal) return NivelIndicador.Bom;
            if (valor >= Minimo) return NivelIndicador.Atencao;
            return NivelIndicador.Ruim;
        }

        public string Texto => $"Meta: acima de {Ideal:0.#}{Unidade}";
    }

    // Todas as metas do dashboard ficam aqui, para ser fácil de ajustar
    public static class MetasDashboard
    {
        // Aprovados / (aprovados + reprovados)
        public static readonly Meta TaxaAprovacao = new(50, 70, "%");

        // Currículos em análise ou entrevista / total
        public static readonly Meta EmProcesso = new(20, 40, "%");

        // Cadastros do período x período anterior (negativo é ruim)
        public static readonly Meta Crescimento = new(0, 10, "%");

        // Média de anos de experiência dos candidatos
        public static readonly Meta ExperienciaMedia = new(2, 5, " anos");
    }

    // Textos e classes CSS de cada nível (a cor sempre vem com ícone e texto)
    public static class Semaforo
    {
        public static string Classe(NivelIndicador nivel) => nivel switch
        {
            NivelIndicador.Bom => "nivel-bom",
            NivelIndicador.Atencao => "nivel-atencao",
            NivelIndicador.Ruim => "nivel-ruim",
            _ => "nivel-neutro"
        };

        public static string Rotulo(NivelIndicador nivel) => nivel switch
        {
            NivelIndicador.Bom => "Bom",
            NivelIndicador.Atencao => "Atenção",
            NivelIndicador.Ruim => "Crítico",
            _ => "Sem dados"
        };

        public static string Icone(NivelIndicador nivel) => nivel switch
        {
            NivelIndicador.Bom => "✓",
            NivelIndicador.Atencao => "!",
            NivelIndicador.Ruim => "✕",
            _ => "–"
        };

        // Seta e sinal de uma variação percentual (ex.: "↑ 20%")
        public static string TextoVariacao(int variacao) =>
            variacao > 0 ? $"↑ {variacao}%" : variacao < 0 ? $"↓ {Math.Abs(variacao)}%" : "0%";
    }
}
