namespace TalentDesk.Models
{
    public class FaixaIdade
    {
        public string Valor { get; }
        public string Rotulo { get; }
        public int IdadeMinima { get; }
        public int IdadeMaxima { get; }

        public FaixaIdade(string valor, string rotulo, int idadeMinima, int idadeMaxima)
        {
            Valor = valor;
            Rotulo = rotulo;
            IdadeMinima = idadeMinima;
            IdadeMaxima = idadeMaxima;
        }

        public bool Contem(int idade)
        {
            return idade >= IdadeMinima && idade <= IdadeMaxima;
        }

        // Faixas exibidas no filtro: menor de 18, 18 a 20, depois de 5 em 5 anos até 60 e acima de 60.
        public static IReadOnlyList<FaixaIdade> Todas { get; } = CriarFaixas();

        public static FaixaIdade? ObterPorValor(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            return Todas.FirstOrDefault(faixa => faixa.Valor == valor);
        }

        private static List<FaixaIdade> CriarFaixas()
        {
            var faixas = new List<FaixaIdade>
            {
                new FaixaIdade("menor-18", "Menor de 18", 0, 17),
                new FaixaIdade("18-20", "18 a 20 anos", 18, 20)
            };

            for (int inicio = 21; inicio <= 56; inicio += 5)
            {
                int fim = inicio + 4;
                faixas.Add(new FaixaIdade($"{inicio}-{fim}", $"{inicio} a {fim} anos", inicio, fim));
            }

            faixas.Add(new FaixaIdade("acima-60", "Acima de 60", 61, int.MaxValue));

            return faixas;
        }
    }
}
