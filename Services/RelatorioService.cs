using System.Globalization;
using TalentDesk.Models;
using TalentDesk.Repositories;

namespace TalentDesk.Services
{
    // Calcula os números da tela de relatórios a partir da lista de currículos
    public class RelatorioService
    {
        private static readonly CultureInfo PtBr = new("pt-BR");

        private readonly CurriculoRepository _curriculoRepository;

        public RelatorioService(CurriculoRepository curriculoRepository)
        {
            _curriculoRepository = curriculoRepository;
        }

        public RelatoriosViewModel Montar(FiltroRelatorio filtro)
        {
            filtro.Normalizar();

            var agora = DateTime.Now;
            var todos = _curriculoRepository.ListarParaRelatorio(filtro.Area);

            // Currículos dentro do período escolhido
            var curriculos = filtro.Dias == 0
                ? todos
                : todos.Where(c => c.DataCadastro >= agora.AddDays(-filtro.Dias)).ToList();

            var viewModel = new RelatoriosViewModel
            {
                Filtro = filtro,
                Areas = _curriculoRepository.ListarAreas(),
                Total = curriculos.Count,
                Crescimento = CalcularVariacao(todos, filtro.Dias == 0 ? 30 : filtro.Dias, agora),
                RotuloCrescimento = filtro.Dias == 0
                    ? "últimos 30 dias x 30 dias anteriores"
                    : "vs. período anterior",
                EmProcesso = curriculos.Count(c => StatusCurriculo.EmProcesso.Contains(c.Status)),
                Aprovados = curriculos.Count(c => c.Status == StatusCurriculo.Aprovado),
                Reprovados = curriculos.Count(c => c.Status == StatusCurriculo.Reprovado),
                MediaExperiencia = curriculos.Count == 0 ? 0 : curriculos.Average(c => c.AnosExperiencia),
                MediaIdade = curriculos.Count == 0 ? 0 : curriculos.Average(c => c.Idade),
                Funil = StatusCurriculo.Todos
                    .Select(status => new ItemContagem { Nome = status, Quantidade = curriculos.Count(c => c.Status == status) })
                    .ToList(),
                PorArea = ContarPor(curriculos, c => c.Area),
                PorFormacao = ContarPor(curriculos, c => c.Formacao)
            };

            MontarEvolucao(viewModel, curriculos, filtro.Dias, agora);
            MontarMapa(viewModel, curriculos);

            return viewModel;
        }

        // Compara com o período anterior de mesmo tamanho (ex.: últimos 30 dias x 30 dias antes)
        private static int? CalcularVariacao(List<Curriculo> todos, int dias, DateTime agora)
        {
            int atual = todos.Count(c => c.DataCadastro >= agora.AddDays(-dias));
            int anterior = todos.Count(c => c.DataCadastro >= agora.AddDays(-2 * dias) && c.DataCadastro < agora.AddDays(-dias));

            if (anterior == 0) return null;
            return (int)Math.Round((atual - anterior) * 100.0 / anterior);
        }

        private static List<ItemContagem> ContarPor(List<Curriculo> curriculos, Func<Curriculo, string> campo, int limite = 6)
        {
            return curriculos
                .GroupBy(campo)
                .Select(grupo => new ItemContagem { Nome = grupo.Key, Quantidade = grupo.Count() })
                .OrderByDescending(item => item.Quantidade)
                .ThenBy(item => item.Nome, StringComparer.Create(PtBr, true))
                .Take(limite)
                .ToList();
        }

        // Até 30 dias: uma barra por dia. 90 dias: por semana. Mais que isso: por mês (12 meses).
        private static void MontarEvolucao(RelatoriosViewModel viewModel, List<Curriculo> curriculos, int dias, DateTime agora)
        {
            var hoje = agora.Date;

            if (dias is > 0 and <= 30)
            {
                viewModel.TituloEvolucao = "Cadastros por dia";
                for (int i = dias - 1; i >= 0; i--)
                {
                    var dia = hoje.AddDays(-i);
                    int quantidade = curriculos.Count(c => c.DataCadastro.Date == dia);
                    viewModel.Evolucao.Add(new BarraEvolucao(dia.ToString("dd/MM"), $"{dia:dd/MM}: {Texto(quantidade)}", quantidade));
                }
            }
            else if (dias == 90)
            {
                viewModel.TituloEvolucao = "Cadastros por semana";
                for (int i = 12; i >= 0; i--)
                {
                    var fim = hoje.AddDays(-7 * i);
                    var inicio = fim.AddDays(-6);
                    int quantidade = curriculos.Count(c => c.DataCadastro.Date >= inicio && c.DataCadastro.Date <= fim);
                    viewModel.Evolucao.Add(new BarraEvolucao(inicio.ToString("dd/MM"), $"{inicio:dd/MM} a {fim:dd/MM}: {Texto(quantidade)}", quantidade));
                }
            }
            else
            {
                viewModel.TituloEvolucao = "Cadastros por mês";
                var mesAtual = new DateTime(hoje.Year, hoje.Month, 1);
                for (int i = 11; i >= 0; i--)
                {
                    var mes = mesAtual.AddMonths(-i);
                    int quantidade = curriculos.Count(c => c.DataCadastro.Year == mes.Year && c.DataCadastro.Month == mes.Month);
                    string nome = mes.ToString("MMM", PtBr).TrimEnd('.');
                    viewModel.Evolucao.Add(new BarraEvolucao(nome, $"{mes.ToString("MMMM 'de' yyyy", PtBr)}: {Texto(quantidade)}", quantidade));
                }
            }

            // Crescimento de cada barra em relação à anterior (ex.: outubro x setembro)
            for (int i = 1; i < viewModel.Evolucao.Count; i++)
            {
                int anterior = viewModel.Evolucao[i - 1].Quantidade;
                if (anterior == 0) continue;

                var barra = viewModel.Evolucao[i];
                int variacao = (int)Math.Round((barra.Quantidade - anterior) * 100.0 / anterior);
                viewModel.Evolucao[i] = barra with
                {
                    Variacao = variacao,
                    Dica = $"{barra.Dica} ({Semaforo.TextoVariacao(variacao)} vs. anterior)"
                };
            }
        }

        private static void MontarMapa(RelatoriosViewModel viewModel, List<Curriculo> curriculos)
        {
            var porRegiao = MapaBrasil.Regioes.ToDictionary(regiao => regiao, _ => 0);

            foreach (var grupo in curriculos.GroupBy(c => c.Cidade))
            {
                var local = MapaBrasil.SepararCidade(grupo.Key);
                if (local == null)
                {
                    viewModel.SemLocalizacao += grupo.Count();
                    continue;
                }

                var (x, y) = MapaBrasil.Posicao(local.Value.Cidade, local.Value.Uf);
                viewModel.Pontos.Add(new PontoMapa(grupo.Key, local.Value.Uf, x, y, grupo.Count()));
                porRegiao[MapaBrasil.RegiaoDaUf(local.Value.Uf)] += grupo.Count();
            }

            // Bolinhas maiores primeiro, para as menores ficarem por cima e continuarem clicáveis
            viewModel.Pontos = viewModel.Pontos.OrderByDescending(p => p.Quantidade).ToList();
            viewModel.PorRegiao = porRegiao
                .Select(par => new ItemContagem { Nome = par.Key, Quantidade = par.Value })
                .ToList();
        }

        private static string Texto(int quantidade) => quantidade == 1 ? "1 currículo" : $"{quantidade} currículos";
    }
}
