using Microsoft.AspNetCore.Mvc;
using TalentDesk.Models;
using TalentDesk.Repositories;
using TalentDesk.Services;

namespace TalentDesk.Controllers
{
    // Demais itens do menu lateral
    public class PainelController : Controller
    {
        private readonly RelatorioService _relatorioService = new(new CurriculoRepository());

        [HttpGet]
        public IActionResult Relatorios([FromQuery] FiltroRelatorio filtro)
        {
            return View(_relatorioService.Montar(filtro));
        }

        [HttpGet]
        public IActionResult Vagas()
        {
            ViewData["Menu"] = "vagas";
            ViewData["Title"] = "Vagas";
            ViewData["Descricao"] = "Em breve você poderá cadastrar vagas e vincular currículos a cada processo seletivo.";
            return View("EmBreve");
        }

        [HttpGet]
        public IActionResult Configuracoes()
        {
            ViewData["Menu"] = "configuracoes";
            ViewData["Title"] = "Configurações";
            ViewData["Descricao"] = "Em breve você poderá editar seus dados, trocar a senha e ajustar preferências do sistema.";
            return View("EmBreve");
        }
    }
}
