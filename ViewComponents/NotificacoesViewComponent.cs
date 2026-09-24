using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using TalentDesk.Models;
using TalentDesk.Repositories;

namespace TalentDesk.ViewComponents
{
    // Sino do topo: lista os currículos novos dos últimos 7 dias
    public class NotificacoesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            List<Curriculo> novos;

            try
            {
                novos = new CurriculoRepository().ListarNovos(dias: 7, limite: 5);
            }
            catch (MySqlException)
            {
                novos = new List<Curriculo>();
            }

            return View(novos);
        }
    }
}
