namespace TalentDesk.Models
{
    // Etapas do processo seletivo de um currículo
    public static class StatusCurriculo
    {
        public const string Novo = "Novo";
        public const string EmAnalise = "Em análise";
        public const string Entrevista = "Entrevista";
        public const string Aprovado = "Aprovado";
        public const string Reprovado = "Reprovado";

        public static IReadOnlyList<string> Todos { get; } = new[]
        {
            Novo, EmAnalise, Entrevista, Aprovado, Reprovado
        };

        // Status que contam como "em processo seletivo" no dashboard
        public static IReadOnlyList<string> EmProcesso { get; } = new[] { EmAnalise, Entrevista };

        public static bool EhValido(string? status)
        {
            return status != null && Todos.Contains(status);
        }

        // Classe CSS usada no selo colorido de cada status
        public static string ClasseCss(string status) => status switch
        {
            Novo => "status-novo",
            EmAnalise => "status-analise",
            Entrevista => "status-entrevista",
            Aprovado => "status-aprovado",
            Reprovado => "status-reprovado",
            _ => "status-novo"
        };
    }
}
