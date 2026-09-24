namespace TalentDesk.Models
{
    // Opções do filtro "Experiência" (em anos)
    public class FaixaExperiencia
    {
        public string Valor { get; }
        public string Rotulo { get; }
        public int Minimo { get; }
        public int Maximo { get; }

        public FaixaExperiencia(string valor, string rotulo, int minimo, int maximo)
        {
            Valor = valor;
            Rotulo = rotulo;
            Minimo = minimo;
            Maximo = maximo;
        }

        public static IReadOnlyList<FaixaExperiencia> Todas { get; } = new List<FaixaExperiencia>
        {
            new("0-2", "Até 2 anos", 0, 2),
            new("3-5", "3 a 5 anos", 3, 5),
            new("6-10", "6 a 10 anos", 6, 10),
            new("10+", "Mais de 10 anos", 11, 100)
        };

        public static FaixaExperiencia? ObterPorValor(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            return Todas.FirstOrDefault(f => f.Valor == valor);
        }
    }
}
