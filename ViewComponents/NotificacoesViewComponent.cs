using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
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
            catch (SqliteException)
            {
                novos = new List<Curriculo>();
            }

            return View(novos);
        }
    }
}
