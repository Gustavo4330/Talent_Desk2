using MySqlConnector;
using TalentDesk.Models;

namespace TalentDesk.Repositories
{
    public class ItemContagem
    {
        public string Nome { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }

    public class CurriculoRepository
    {
        private const string ColunasCurriculo = @"
            c.id, c.nome, c.cargo, c.area, c.formacao, c.formacao_detalhada, c.instituicao,
            c.conclusao_formacao, c.idade, c.anos_experiencia, c.foto_url, c.email, c.telefone,
            c.cidade, c.linkedin, c.resumo, c.status, c.data_cadastro";

        private readonly bool _verificarBanco;

        public CurriculoRepository() : this(inicializarBanco: true)
        {
        }

        public CurriculoRepository(bool inicializarBanco)
        {
            _verificarBanco = inicializarBanco;
        }

        // Abre a conexão garantindo antes que as tabelas existem
        private MySqlConnection AbrirConexao()
        {
            if (_verificarBanco)
            {
                BancoDeDados.GarantirInicializado();
            }

            var conexao = Conexao.CriarConexao();
            conexao.Open();
            return conexao;
        }

        // ================= CONSULTAS =================

        // Busca com filtros e paginação. Retorna a página pedida e o total encontrado.
        public (List<Curriculo> Itens, int Total) Buscar(FiltroCurriculo filtro, ICollection<int> favoritos)
        {
            // Tela de favoritos sem nenhum favorito: nem precisa ir ao banco
            if (filtro.EhFavoritos && favoritos.Count == 0)
            {
                return (new List<Curriculo>(), 0);
            }

            using var conexao = AbrirConexao();

            using var comandoTotal = new MySqlCommand { Connection = conexao };
            string where = MontarWhere(filtro, favoritos, comandoTotal);

            comandoTotal.CommandText = $"SELECT COUNT(*) FROM curriculos c {where};";
            int total = Convert.ToInt32(comandoTotal.ExecuteScalar());

            // Se a página pedida não existe mais (ex.: após filtrar), vai para a última
            int totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)filtro.Itens));
            if (filtro.Pagina > totalPaginas)
            {
                filtro.Pagina = totalPaginas;
            }

            using var comando = new MySqlCommand { Connection = conexao };
            where = MontarWhere(filtro, favoritos, comando);

            comando.CommandText = $@"
                SELECT {ColunasCurriculo}
                FROM curriculos c
                {where}
                ORDER BY c.data_cadastro DESC, c.id DESC
                LIMIT @limite OFFSET @deslocamento;";

            comando.Parameters.AddWithValue("@limite", filtro.Itens);
            comando.Parameters.AddWithValue("@deslocamento", (filtro.Pagina - 1) * filtro.Itens);

            var itens = new List<Curriculo>();
            using (var leitor = comando.ExecuteReader())
            {
                while (leitor.Read())
                {
                    itens.Add(LerCurriculo(leitor));
                }
            }

            return (itens, total);
        }

        // Monta o WHERE com parâmetros (nunca concatenando o texto digitado)
        private static string MontarWhere(FiltroCurriculo filtro, ICollection<int> favoritos, MySqlCommand comando)
        {
            var condicoes = new List<string>();

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                condicoes.Add(@"(c.nome LIKE @busca OR c.cargo LIKE @busca OR c.area LIKE @busca
                    OR EXISTS (SELECT 1 FROM curriculo_habilidades h
                               WHERE h.curriculo_id = c.id AND h.nome LIKE @busca))");
                comando.Parameters.AddWithValue("@busca", $"%{filtro.Busca.Trim()}%");
            }

            if (!string.IsNullOrWhiteSpace(filtro.Area))
            {
                condicoes.Add("c.area = @area");
                comando.Parameters.AddWithValue("@area", filtro.Area);
            }

            if (!string.IsNullOrWhiteSpace(filtro.Formacao))
            {
                condicoes.Add("c.formacao = @formacao");
                comando.Parameters.AddWithValue("@formacao", filtro.Formacao);
            }

            if (!string.IsNullOrWhiteSpace(filtro.Localizacao))
            {
                condicoes.Add("c.cidade = @cidade");
                comando.Parameters.AddWithValue("@cidade", filtro.Localizacao);
            }

            var faixa = FaixaExperiencia.ObterPorValor(filtro.Experiencia);
            if (faixa != null)
            {
                condicoes.Add("c.anos_experiencia BETWEEN @expMin AND @expMax");
                comando.Parameters.AddWithValue("@expMin", faixa.Minimo);
                comando.Parameters.AddWithValue("@expMax", faixa.Maximo);
            }

            if (filtro.Idade.HasValue)
            {
                condicoes.Add("c.idade = @idade");
                comando.Parameters.AddWithValue("@idade", filtro.Idade.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtro.Status))
            {
                condicoes.Add("c.status = @status");
                comando.Parameters.AddWithValue("@status", filtro.Status);
            }

            if (filtro.EhFavoritos)
            {
                condicoes.Add($"c.id IN ({CriarParametrosIn(favoritos, comando, "fav")})");
            }

            return condicoes.Count == 0 ? string.Empty : "WHERE " + string.Join(" AND ", condicoes);
        }

        // Cria @fav0, @fav1, ... para usar em "IN (...)"
        private static string CriarParametrosIn(IEnumerable<int> ids, MySqlCommand comando, string prefixo)
        {
            var nomes = new List<string>();
            int i = 0;
            foreach (int id in ids)
            {
                string nome = $"@{prefixo}{i++}";
                nomes.Add(nome);
                comando.Parameters.AddWithValue(nome, id);
            }
            return nomes.Count == 0 ? "NULL" : string.Join(", ", nomes);
        }

        public Curriculo? ObterPorId(int id)
        {
            using var conexao = AbrirConexao();

            Curriculo? curriculo;

            using (var comando = new MySqlCommand($"SELECT {ColunasCurriculo} FROM curriculos c WHERE c.id = @id;", conexao))
            {
                comando.Parameters.AddWithValue("@id", id);
                using var leitor = comando.ExecuteReader();
                curriculo = leitor.Read() ? LerCurriculo(leitor) : null;
            }

            if (curriculo == null)
            {
                return null;
            }

            using (var comando = new MySqlCommand(@"
                SELECT cargo, empresa, periodo, descricao
                FROM curriculo_experiencias
                WHERE curriculo_id = @id
                ORDER BY ordem, id;", conexao))
            {
                comando.Parameters.AddWithValue("@id", id);
                using var leitor = comando.ExecuteReader();
                while (leitor.Read())
                {
                    curriculo.Experiencias.Add(new ExperienciaProfissional
                    {
                        Cargo = leitor.GetString("cargo"),
                        Empresa = leitor.GetString("empresa"),
                        Periodo = TextoOuVazio(leitor, "periodo"),
                        Descricao = TextoOuVazio(leitor, "descricao")
                    });
                }
            }

            using (var comando = new MySqlCommand(@"
                SELECT nome FROM curriculo_habilidades
                WHERE curriculo_id = @id
                ORDER BY id;", conexao))
            {
                comando.Parameters.AddWithValue("@id", id);
                using var leitor = comando.ExecuteReader();
                while (leitor.Read())
                {
                    curriculo.Habilidades.Add(leitor.GetString("nome"));
                }
            }

            return curriculo;
        }

        public bool Existe(int id)
        {
            using var conexao = AbrirConexao();

            using var comando = new MySqlCommand("SELECT COUNT(*) FROM curriculos WHERE id = @id;", conexao);
            comando.Parameters.AddWithValue("@id", id);
            return Convert.ToInt32(comando.ExecuteScalar()) > 0;
        }

        // Valores distintos de uma coluna, para montar as opções dos filtros
        public List<string> ListarAreas() => ListarDistintos("area");
        public List<string> ListarFormacoes() => ListarDistintos("formacao");
        public List<string> ListarCidades() => ListarDistintos("cidade");

        private List<string> ListarDistintos(string coluna)
        {
            // A coluna vem só dos três métodos acima, nunca do usuário
            using var conexao = AbrirConexao();

            using var comando = new MySqlCommand(
                $"SELECT DISTINCT {coluna} FROM curriculos WHERE {coluna} <> '' ORDER BY {coluna};", conexao);

            var valores = new List<string>();
            using var leitor = comando.ExecuteReader();
            while (leitor.Read())
            {
                valores.Add(leitor.GetString(0));
            }
            return valores;
        }

        // Currículos com status "Novo" cadastrados nos últimos dias (notificações do sino)
        public List<Curriculo> ListarNovos(int dias, int limite)
        {
            using var conexao = AbrirConexao();

            using var comando = new MySqlCommand($@"
                SELECT {ColunasCurriculo}
                FROM curriculos c
                WHERE c.status = @status AND c.data_cadastro >= @desde
                ORDER BY c.data_cadastro DESC
                LIMIT @limite;", conexao);

            comando.Parameters.AddWithValue("@status", StatusCurriculo.Novo);
            comando.Parameters.AddWithValue("@desde", DateTime.Now.AddDays(-dias));
            comando.Parameters.AddWithValue("@limite", limite);

            var itens = new List<Curriculo>();
            using var leitor = comando.ExecuteReader();
            while (leitor.Read())
            {
                itens.Add(LerCurriculo(leitor));
            }
            return itens;
        }

        public EstatisticasDashboard ObterEstatisticas(ICollection<int> favoritos)
        {
            using var conexao = AbrirConexao();

            var agora = DateTime.Now;
            var inicioDoMes = new DateTime(agora.Year, agora.Month, 1);

            using var comando = new MySqlCommand(@"
                SELECT
                    COUNT(*) AS total,
                    COALESCE(SUM(data_cadastro < @inicioMes), 0) AS total_inicio_mes,
                    COALESCE(SUM(data_cadastro >= @seteDias), 0) AS novos_semana,
                    COALESCE(SUM(data_cadastro >= @quatorzeDias AND data_cadastro < @seteDias), 0) AS novos_semana_anterior,
                    COALESCE(SUM(status IN (@statusAnalise, @statusEntrevista)), 0) AS em_processo
                FROM curriculos;", conexao);

            comando.Parameters.AddWithValue("@inicioMes", inicioDoMes);
            comando.Parameters.AddWithValue("@seteDias", agora.AddDays(-7));
            comando.Parameters.AddWithValue("@quatorzeDias", agora.AddDays(-14));
            comando.Parameters.AddWithValue("@statusAnalise", StatusCurriculo.EmAnalise);
            comando.Parameters.AddWithValue("@statusEntrevista", StatusCurriculo.Entrevista);

            var estatisticas = new EstatisticasDashboard();

            using (var leitor = comando.ExecuteReader())
            {
                if (leitor.Read())
                {
                    estatisticas.TotalCurriculos = Convert.ToInt32(leitor["total"]);
                    estatisticas.TotalNoInicioDoMes = Convert.ToInt32(leitor["total_inicio_mes"]);
                    estatisticas.NovosEstaSemana = Convert.ToInt32(leitor["novos_semana"]);
                    estatisticas.NovosSemanaAnterior = Convert.ToInt32(leitor["novos_semana_anterior"]);
                    estatisticas.EmProcessoSeletivo = Convert.ToInt32(leitor["em_processo"]);
                }
            }

            // Conta só os favoritos que ainda existem no banco
            if (favoritos.Count > 0)
            {
                using var comandoFav = new MySqlCommand { Connection = conexao };
                string parametros = CriarParametrosIn(favoritos, comandoFav, "fav");
                comandoFav.CommandText = $"SELECT COUNT(*) FROM curriculos WHERE id IN ({parametros});";
                estatisticas.Favoritos = Convert.ToInt32(comandoFav.ExecuteScalar());
            }

            return estatisticas;
        }

        // Quantidade de currículos agrupados por uma coluna (tela de relatórios)
        public List<ItemContagem> ContarPor(string coluna)
        {
            // Só aceita colunas conhecidas, para evitar SQL injection
            string[] permitidas = { "area", "status", "cidade", "formacao" };
            if (!permitidas.Contains(coluna))
            {
                throw new ArgumentException("Coluna inválida para relatório.", nameof(coluna));
            }

            using var conexao = AbrirConexao();

            using var comando = new MySqlCommand($@"
                SELECT {coluna} AS nome, COUNT(*) AS quantidade
                FROM curriculos
                GROUP BY {coluna}
                ORDER BY quantidade DESC, nome;", conexao);

            var itens = new List<ItemContagem>();
            using var leitor = comando.ExecuteReader();
            while (leitor.Read())
            {
                itens.Add(new ItemContagem
                {
                    Nome = leitor.GetString("nome"),
                    Quantidade = Convert.ToInt32(leitor["quantidade"])
                });
            }
            return itens;
        }

        // ================= GRAVAÇÃO =================

        public int Inserir(Curriculo curriculo)
        {
            using var conexao = AbrirConexao();
            using var transacao = conexao.BeginTransaction();

            try
            {
                using var comando = new MySqlCommand(@"
                    INSERT INTO curriculos
                        (nome, cargo, area, formacao, formacao_detalhada, instituicao, conclusao_formacao,
                         idade, anos_experiencia, foto_url, email, telefone, cidade, linkedin, resumo,
                         status, data_cadastro)
                    VALUES
                        (@nome, @cargo, @area, @formacao, @formacao_detalhada, @instituicao, @conclusao_formacao,
                         @idade, @anos_experiencia, @foto_url, @email, @telefone, @cidade, @linkedin, @resumo,
                         @status, @data_cadastro);
                    SELECT LAST_INSERT_ID();", conexao, transacao);

                AdicionarParametros(comando, curriculo);
                comando.Parameters.AddWithValue("@data_cadastro", curriculo.DataCadastro);

                int id = Convert.ToInt32(comando.ExecuteScalar());

                GravarFilhos(conexao, transacao, id, curriculo);

                transacao.Commit();
                curriculo.Id = id;
                return id;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
        }

        public bool Atualizar(Curriculo curriculo)
        {
            using var conexao = AbrirConexao();
            using var transacao = conexao.BeginTransaction();

            try
            {
                using var comando = new MySqlCommand(@"
                    UPDATE curriculos SET
                        nome = @nome, cargo = @cargo, area = @area, formacao = @formacao,
                        formacao_detalhada = @formacao_detalhada, instituicao = @instituicao,
                        conclusao_formacao = @conclusao_formacao, idade = @idade,
                        anos_experiencia = @anos_experiencia, foto_url = @foto_url, email = @email,
                        telefone = @telefone, cidade = @cidade, linkedin = @linkedin, resumo = @resumo,
                        status = @status, data_atualizacao = @data_atualizacao
                    WHERE id = @id;", conexao, transacao);

                AdicionarParametros(comando, curriculo);
                comando.Parameters.AddWithValue("@data_atualizacao", DateTime.Now);
                comando.Parameters.AddWithValue("@id", curriculo.Id);

                if (comando.ExecuteNonQuery() == 0)
                {
                    transacao.Rollback();
                    return false;
                }

                // Apaga experiências e competências antigas e grava as novas
                using (var apagar = new MySqlCommand(@"
                    DELETE FROM curriculo_experiencias WHERE curriculo_id = @id;
                    DELETE FROM curriculo_habilidades WHERE curriculo_id = @id;", conexao, transacao))
                {
                    apagar.Parameters.AddWithValue("@id", curriculo.Id);
                    apagar.ExecuteNonQuery();
                }

                GravarFilhos(conexao, transacao, curriculo.Id, curriculo);

                transacao.Commit();
                return true;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
        }

        public bool AlterarStatus(int id, string status)
        {
            if (!StatusCurriculo.EhValido(status)) return false;

            using var conexao = AbrirConexao();

            using var comando = new MySqlCommand(@"
                UPDATE curriculos SET status = @status, data_atualizacao = @agora WHERE id = @id;", conexao);
            comando.Parameters.AddWithValue("@status", status);
            comando.Parameters.AddWithValue("@agora", DateTime.Now);
            comando.Parameters.AddWithValue("@id", id);
            return comando.ExecuteNonQuery() > 0;
        }

        public bool Excluir(int id)
        {
            using var conexao = AbrirConexao();

            // Experiências e competências são apagadas junto (ON DELETE CASCADE)
            using var comando = new MySqlCommand("DELETE FROM curriculos WHERE id = @id;", conexao);
            comando.Parameters.AddWithValue("@id", id);
            return comando.ExecuteNonQuery() > 0;
        }

        // ================= AUXILIARES =================

        private static void AdicionarParametros(MySqlCommand comando, Curriculo c)
        {
            comando.Parameters.AddWithValue("@nome", c.Nome);
            comando.Parameters.AddWithValue("@cargo", c.Cargo);
            comando.Parameters.AddWithValue("@area", c.Area);
            comando.Parameters.AddWithValue("@formacao", c.Formacao);
            comando.Parameters.AddWithValue("@formacao_detalhada", c.FormacaoDetalhada);
            comando.Parameters.AddWithValue("@instituicao", c.Instituicao);
            comando.Parameters.AddWithValue("@conclusao_formacao", c.ConclusaoFormacao);
            comando.Parameters.AddWithValue("@idade", c.Idade);
            comando.Parameters.AddWithValue("@anos_experiencia", c.AnosExperiencia);
            comando.Parameters.AddWithValue("@foto_url", c.FotoUrl);
            comando.Parameters.AddWithValue("@email", c.Email);
            comando.Parameters.AddWithValue("@telefone", c.Telefone);
            comando.Parameters.AddWithValue("@cidade", c.Cidade);
            comando.Parameters.AddWithValue("@linkedin", c.Linkedin);
            comando.Parameters.AddWithValue("@resumo", c.Resumo);
            comando.Parameters.AddWithValue("@status", c.Status);
        }

        private static void GravarFilhos(MySqlConnection conexao, MySqlTransaction transacao, int curriculoId, Curriculo c)
        {
            int ordem = 0;
            foreach (var experiencia in c.Experiencias)
            {
                using var comando = new MySqlCommand(@"
                    INSERT INTO curriculo_experiencias (curriculo_id, cargo, empresa, periodo, descricao, ordem)
                    VALUES (@curriculo_id, @cargo, @empresa, @periodo, @descricao, @ordem);", conexao, transacao);

                comando.Parameters.AddWithValue("@curriculo_id", curriculoId);
                comando.Parameters.AddWithValue("@cargo", experiencia.Cargo);
                comando.Parameters.AddWithValue("@empresa", experiencia.Empresa);
                comando.Parameters.AddWithValue("@periodo", experiencia.Periodo);
                comando.Parameters.AddWithValue("@descricao", experiencia.Descricao);
                comando.Parameters.AddWithValue("@ordem", ordem++);
                comando.ExecuteNonQuery();
            }

            foreach (string habilidade in c.Habilidades)
            {
                using var comando = new MySqlCommand(@"
                    INSERT INTO curriculo_habilidades (curriculo_id, nome)
                    VALUES (@curriculo_id, @nome);", conexao, transacao);

                comando.Parameters.AddWithValue("@curriculo_id", curriculoId);
                comando.Parameters.AddWithValue("@nome", habilidade.Length > 80 ? habilidade[..80] : habilidade);
                comando.ExecuteNonQuery();
            }
        }

        private static Curriculo LerCurriculo(MySqlDataReader leitor)
        {
            return new Curriculo
            {
                Id = leitor.GetInt32("id"),
                Nome = leitor.GetString("nome"),
                Cargo = leitor.GetString("cargo"),
                Area = leitor.GetString("area"),
                Formacao = leitor.GetString("formacao"),
                FormacaoDetalhada = TextoOuVazio(leitor, "formacao_detalhada"),
                Instituicao = TextoOuVazio(leitor, "instituicao"),
                ConclusaoFormacao = TextoOuVazio(leitor, "conclusao_formacao"),
                Idade = leitor.GetInt32("idade"),
                AnosExperiencia = leitor.GetInt32("anos_experiencia"),
                FotoUrl = TextoOuVazio(leitor, "foto_url"),
                Email = leitor.GetString("email"),
                Telefone = TextoOuVazio(leitor, "telefone"),
                Cidade = leitor.GetString("cidade"),
                Linkedin = TextoOuVazio(leitor, "linkedin"),
                Resumo = TextoOuVazio(leitor, "resumo"),
                Status = leitor.GetString("status"),
                DataCadastro = leitor.GetDateTime("data_cadastro")
            };
        }

        private static string TextoOuVazio(MySqlDataReader leitor, string coluna)
        {
            return leitor[coluna] as string ?? string.Empty;
        }
    }
}
