using Microsoft.AspNetCore.Http;

namespace TalentDesk.Services
{
    // Guarda os IDs dos currículos favoritos em um cookie do navegador.
    // Ex.: talentdesk_favoritos = "1,3,5"
    public class FavoritoService
    {
        private const string NomeCookie = "talentdesk_favoritos";

        public HashSet<int> ObterFavoritos(HttpRequest request)
        {
            var favoritos = new HashSet<int>();

            string? valor = request.Cookies[NomeCookie];
            if (string.IsNullOrWhiteSpace(valor))
            {
                return favoritos;
            }

            foreach (string parte in valor.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(parte, out int id))
                {
                    favoritos.Add(id);
                }
            }

            return favoritos;
        }

        // Se já é favorito, remove. Se não é, adiciona.
        // Retorna true quando o currículo passou a ser favorito.
        public bool AlternarFavorito(HttpRequest request, HttpResponse response, int curriculoId)
        {
            var favoritos = ObterFavoritos(request);
            bool agoraEhFavorito;

            if (favoritos.Contains(curriculoId))
            {
                favoritos.Remove(curriculoId);
                agoraEhFavorito = false;
            }
            else
            {
                favoritos.Add(curriculoId);
                agoraEhFavorito = true;
            }

            SalvarFavoritos(response, favoritos);
            return agoraEhFavorito;
        }

        // Usado quando um currículo é excluído
        public void RemoverFavorito(HttpRequest request, HttpResponse response, int curriculoId)
        {
            var favoritos = ObterFavoritos(request);
            if (favoritos.Remove(curriculoId))
            {
                SalvarFavoritos(response, favoritos);
            }
        }

        private static void SalvarFavoritos(HttpResponse response, HashSet<int> favoritos)
        {
            var opcoes = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddYears(1),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            };

            response.Cookies.Append(NomeCookie, string.Join(",", favoritos.OrderBy(id => id)), opcoes);
        }
    }
}
