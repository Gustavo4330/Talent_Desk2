using Microsoft.AspNetCore.Mvc;
using TalentDesk.Models;
using TalentDesk.Repositories;
using TalentDesk.Services;

namespace TalentDesk.Controllers
{
    public class CurriculoController : Controller
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly CurriculoRepository _curriculoRepository;
        private readonly FavoritoService _favoritoService;

        public CurriculoController()
        {
            _usuarioRepository = new UsuarioRepository();
            _curriculoRepository = new CurriculoRepository();
            _favoritoService = new FavoritoService();
        }

        // ============ DASHBOARD / LISTA / FAVORITOS ============

        [HttpGet]
        public IActionResult Index([FromQuery] FiltroCurriculo filtro)
        {
            // Valores inválidos na URL (ex.: ?pagina=abc) são ignorados
            if (!ModelState.IsValid)
            {
                if (ModelState[nameof(FiltroCurriculo.Pagina)]?.Errors.Count > 0) filtro.Pagina = 1;
                if (ModelState[nameof(FiltroCurriculo.Itens)]?.Errors.Count > 0) filtro.Itens = 5;
                if (ModelState[nameof(FiltroCurriculo.Selecionado)]?.Errors.Count > 0) filtro.Selecionado = null;
            }

            filtro.Normalizar();

            var favoritos = _favoritoService.ObterFavoritos(Request);
            var (curriculos, total) = _curriculoRepository.Buscar(filtro, favoritos);

            // Painel da direita: o currículo escolhido ou o primeiro da lista.
            // selecionado=0 significa painel fechado.
            Curriculo? selecionado = null;
            if (filtro.Selecionado is > 0)
            {
                selecionado = _curriculoRepository.ObterPorId(filtro.Selecionado.Value);
            }
            else if (filtro.Selecionado == null && curriculos.Count > 0)
            {
                selecionado = _curriculoRepository.ObterPorId(curriculos[0].Id);
            }

            var viewModel = new CurriculoIndexViewModel
            {
                Filtro = filtro,
                Curriculos = curriculos,
                TotalEncontrado = total,
                Selecionado = selecionado,
                Estatisticas = _curriculoRepository.ObterEstatisticas(favoritos),
                Areas = _curriculoRepository.ListarAreas(),
                Formacoes = _curriculoRepository.ListarFormacoes(),
                Cidades = _curriculoRepository.ListarCidades(),
                FavoritosIds = favoritos
            };

            return View(viewModel);
        }

        // ============ CADASTRO E EDIÇÃO ============

        [HttpGet]
        public IActionResult Cadastrar()
        {
            var form = new CurriculoFormViewModel();
            form.CompletarExperiencias();
            PreencherSugestoes(form);
            return View("Formulario", form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(CurriculoFormViewModel form)
        {
            form.Id = null;

            if (!ModelState.IsValid)
            {
                form.CompletarExperiencias();
                PreencherSugestoes(form);
                return View("Formulario", form);
            }

            var curriculo = form.ParaCurriculo();
            curriculo.DataCadastro = DateTime.Now;
            curriculo.FotoUrl = string.Empty;

            int id = _curriculoRepository.Inserir(curriculo);

            TempData["Mensagem"] = $"Currículo de {curriculo.Nome} cadastrado com sucesso.";
            return RedirectToAction(nameof(Index), new { selecionado = id });
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var curriculo = _curriculoRepository.ObterPorId(id);
            if (curriculo == null)
            {
                return NotFound();
            }

            var form = CurriculoFormViewModel.DeCurriculo(curriculo);
            PreencherSugestoes(form);
            return View("Formulario", form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, CurriculoFormViewModel form)
        {
            var existente = _curriculoRepository.ObterPorId(id);
            if (existente == null)
            {
                return NotFound();
            }

            form.Id = id;

            if (!ModelState.IsValid)
            {
                form.FotoUrl = existente.FotoUrl;
                form.CompletarExperiencias();
                PreencherSugestoes(form);
                return View("Formulario", form);
            }

            var curriculo = form.ParaCurriculo();
            curriculo.Id = id;
            curriculo.FotoUrl = existente.FotoUrl; // mantém a foto que já existia

            _curriculoRepository.Atualizar(curriculo);

            TempData["Mensagem"] = $"Currículo de {curriculo.Nome} atualizado.";
            return RedirectToAction(nameof(Index), new { selecionado = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(int id, string? returnUrl)
        {
            var curriculo = _curriculoRepository.ObterPorId(id);

            if (curriculo != null && _curriculoRepository.Excluir(id))
            {
                _favoritoService.RemoverFavorito(Request, Response, id);
                TempData["Mensagem"] = $"Currículo de {curriculo.Nome} excluído.";
            }

            return VoltarPara(returnUrl);
        }

        // ============ AÇÕES RÁPIDAS ============

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AlterarStatus(int id, string status, string? returnUrl)
        {
            if (_curriculoRepository.AlterarStatus(id, status))
            {
                TempData["Mensagem"] = $"Status alterado para \"{status}\".";
            }

            return VoltarPara(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AlternarFavorito(int id, string? returnUrl)
        {
            if (_curriculoRepository.Existe(id))
            {
                bool favoritou = _favoritoService.AlternarFavorito(Request, Response, id);
                TempData["Mensagem"] = favoritou ? "Adicionado aos favoritos." : "Removido dos favoritos.";
            }

            return VoltarPara(returnUrl);
        }

        // Endereço antigo de detalhes: abre o currículo no painel do dashboard
        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            return RedirectToAction(nameof(Index), new { selecionado = id });
        }

        // ============ TELAS JÁ EXISTENTES ============

        [HttpGet]
        public IActionResult TestDb()
        {
            try
            {
                using var conexao = Conexao.CriarConexao();
                conexao.Open();
                return Content("Conectado ao banco com sucesso!");
            }
            catch (Exception ex)
            {
                return Content("Erro ao conectar:\n" + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = _usuarioRepository.ObterPorEmail(model.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Senha, usuario.Senha))
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                return View(model);
            }

            SessaoUsuario.Entrar(HttpContext.Session, usuario.Id, usuario.Nome, _usuarioRepository.ObterPerfil(usuario.Id));

            TempData["Mensagem"] = $"Bem-vindo(a), {usuario.Nome}!";
            return RedirectToAction(nameof(Index));
        }

        // ============ AUXILIARES ============

        private IActionResult VoltarPara(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        private void PreencherSugestoes(CurriculoFormViewModel form)
        {
            form.SugestoesAreas = _curriculoRepository.ListarAreas();
            form.SugestoesFormacoes = _curriculoRepository.ListarFormacoes();
            form.SugestoesCidades = _curriculoRepository.ListarCidades();
        }
    }
}
