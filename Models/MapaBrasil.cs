namespace TalentDesk.Models
{
    // Posiciona as cidades no mapa do Brasil (Views/Painel/_MapaBrasilEstados.cshtml).
    // As constantes da projeção são as mesmas usadas para gerar o desenho dos estados.
    public static class MapaBrasil
    {
        public const int Largura = 600;
        public const int Altura = 615;

        private const double Margem = 10;
        private const double LongitudeMinima = -74.002046;
        private const double LatitudeMaxima = 5.257981;
        private const double CossenoLatitude = 0.9692648467991987;
        private const double Escala = 15.266427994186785;

        public static readonly IReadOnlyList<string> Regioes = new[] { "Norte", "Nordeste", "Centro-Oeste", "Sudeste", "Sul" };

        private static readonly Dictionary<string, string> RegiaoPorUf = new()
        {
            ["AC"] = "Norte", ["AP"] = "Norte", ["AM"] = "Norte", ["PA"] = "Norte", ["RO"] = "Norte", ["RR"] = "Norte", ["TO"] = "Norte",
            ["AL"] = "Nordeste", ["BA"] = "Nordeste", ["CE"] = "Nordeste", ["MA"] = "Nordeste", ["PB"] = "Nordeste",
            ["PE"] = "Nordeste", ["PI"] = "Nordeste", ["RN"] = "Nordeste", ["SE"] = "Nordeste",
            ["DF"] = "Centro-Oeste", ["GO"] = "Centro-Oeste", ["MT"] = "Centro-Oeste", ["MS"] = "Centro-Oeste",
            ["ES"] = "Sudeste", ["MG"] = "Sudeste", ["RJ"] = "Sudeste", ["SP"] = "Sudeste",
            ["PR"] = "Sul", ["RS"] = "Sul", ["SC"] = "Sul"
        };

        // Coordenadas (latitude, longitude) das capitais: usadas quando a cidade não está na lista abaixo
        private static readonly Dictionary<string, (double Lat, double Lon)> CapitalPorUf = new()
        {
            ["AC"] = (-9.97, -67.81), ["AL"] = (-9.67, -35.74), ["AP"] = (0.03, -51.07), ["AM"] = (-3.12, -60.02),
            ["BA"] = (-12.97, -38.50), ["CE"] = (-3.73, -38.53), ["DF"] = (-15.79, -47.88), ["ES"] = (-20.32, -40.34),
            ["GO"] = (-16.69, -49.26), ["MA"] = (-2.53, -44.30), ["MT"] = (-15.60, -56.10), ["MS"] = (-20.47, -54.62),
            ["MG"] = (-19.92, -43.94), ["PA"] = (-1.46, -48.50), ["PB"] = (-7.12, -34.86), ["PR"] = (-25.43, -49.27),
            ["PE"] = (-8.05, -34.88), ["PI"] = (-5.09, -42.80), ["RJ"] = (-22.91, -43.17), ["RN"] = (-5.79, -35.21),
            ["RS"] = (-30.03, -51.23), ["RO"] = (-8.76, -63.90), ["RR"] = (2.82, -60.67), ["SC"] = (-27.60, -48.55),
            ["SP"] = (-23.55, -46.63), ["SE"] = (-10.91, -37.07), ["TO"] = (-10.18, -48.33)
        };

        // Outras cidades grandes (chave sem acento e em minúsculas)
        private static readonly Dictionary<string, (double Lat, double Lon)> Cidades = new()
        {
            ["campinas-sp"] = (-22.91, -47.06), ["santos-sp"] = (-23.96, -46.33), ["ribeirao preto-sp"] = (-21.18, -47.81),
            ["sao jose dos campos-sp"] = (-23.18, -45.89), ["sorocaba-sp"] = (-23.50, -47.46), ["guarulhos-sp"] = (-23.46, -46.53),
            ["osasco-sp"] = (-23.53, -46.79), ["santo andre-sp"] = (-23.66, -46.53), ["sao bernardo do campo-sp"] = (-23.69, -46.56),
            ["bauru-sp"] = (-22.31, -49.06), ["niteroi-rj"] = (-22.88, -43.10), ["petropolis-rj"] = (-22.51, -43.18),
            ["juiz de fora-mg"] = (-21.76, -43.35), ["uberlandia-mg"] = (-18.92, -48.28), ["contagem-mg"] = (-19.93, -44.05),
            ["montes claros-mg"] = (-16.73, -43.86), ["vila velha-es"] = (-20.33, -40.29), ["londrina-pr"] = (-23.31, -51.16),
            ["maringa-pr"] = (-23.42, -51.94), ["joinville-sc"] = (-26.30, -48.85), ["blumenau-sc"] = (-26.92, -49.07),
            ["caxias do sul-rs"] = (-29.17, -51.18), ["pelotas-rs"] = (-31.77, -52.34), ["feira de santana-ba"] = (-12.27, -38.97),
            ["campina grande-pb"] = (-7.23, -35.88), ["caruaru-pe"] = (-8.28, -35.98), ["anapolis-go"] = (-16.33, -48.95),
            ["santarem-pa"] = (-2.44, -54.71), ["imperatriz-ma"] = (-5.52, -47.47)
        };

        // "Campinas, SP" -> ("Campinas", "SP"). Sem UF válida devolve null.
        public static (string Cidade, string Uf)? SepararCidade(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return null;

            int virgula = texto.LastIndexOf(',');
            if (virgula < 0) return null;

            string cidade = texto[..virgula].Trim();
            string uf = texto[(virgula + 1)..].Trim().ToUpperInvariant();

            return RegiaoPorUf.ContainsKey(uf) ? (cidade, uf) : null;
        }

        public static string RegiaoDaUf(string uf) => RegiaoPorUf[uf];

        // Ponto (x, y) da cidade no desenho; cidade desconhecida fica na capital do estado
        public static (double X, double Y) Posicao(string cidade, string uf)
        {
            var coordenada = Cidades.TryGetValue($"{Conexao.SemAcento(cidade)}-{uf.ToLowerInvariant()}", out var conhecida)
                ? conhecida
                : CapitalPorUf[uf];

            double x = Margem + (coordenada.Lon - LongitudeMinima) * CossenoLatitude * Escala;
            double y = Margem + (LatitudeMaxima - coordenada.Lat) * Escala;
            return (Math.Round(x, 1), Math.Round(y, 1));
        }
    }
}
