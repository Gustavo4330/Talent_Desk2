using Microsoft.AspNetCore.Http;

namespace TalentDesk.Services
{
    // Guarda e lê o usuário logado na sessão do ASP.NET
    public static class SessaoUsuario
    {
        private const string ChaveId = "UsuarioId";
        private const string ChaveNome = "UsuarioNome";
        private const string ChavePerfil = "UsuarioPerfil";

        public static void Entrar(ISession sessao, int id, string nome, string perfil)
        {
            sessao.SetInt32(ChaveId, id);
            sessao.SetString(ChaveNome, nome);
            sessao.SetString(ChavePerfil, perfil);
        }

        public static void Sair(ISession sessao)
        {
            sessao.Clear();
        }

        public static bool EstaLogado(ISession sessao) => sessao.GetInt32(ChaveId).HasValue;

        public static string? Nome(ISession sessao) => sessao.GetString(ChaveNome);

        public static string? Perfil(ISession sessao) => sessao.GetString(ChavePerfil);

        public static string PrimeiroNome(ISession sessao)
        {
            string? nome = Nome(sessao);
            if (string.IsNullOrWhiteSpace(nome)) return string.Empty;
            return nome.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        }

        public static string Iniciais(ISession sessao)
        {
            var partes = (Nome(sessao) ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 0) return "?";
            if (partes.Length == 1) return partes[0][..1].ToUpper();
            return (partes[0][..1] + partes[^1][..1]).ToUpper();
        }
    }
}
