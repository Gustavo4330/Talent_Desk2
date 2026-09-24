using Microsoft.AspNetCore.Mvc;
using TalentDesk.Models;
using TalentDesk.Repositories;

namespace TalentDesk.Controllers
{
    // Demais itens do menu lateral
    public class PainelController : Controller
    {
        private readonly CurriculoRepository _curriculoRepository = new();

        [HttpGet]
        public IActionResult Relatorios()
        {
            var viewModel = new RelatoriosViewModel
            {
                Estatisticas = _curriculoRepository.ObterEstatisticas(new List<int>()),
                PorArea = _curriculoRepository.ContarPor("area"),
                PorStatus = _curriculoRepository.ContarPor("status"),
                PorCidade = _curriculoRepository.ContarPor("cidade"),
                PorFormacao = _curriculoRepository.ContarPor("formacao")
            };

            return View(viewModel);
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
