using System.Globalization;
using Microsoft.Data.Sqlite;

namespace TalentDesk.Repositories
{
    // Cria o arquivo do banco e as tabelas (se ainda não existirem), cadastra os
    // perfis e os currículos de exemplo na primeira vez que o sistema roda.
    public static class BancoDeDados
    {
        private static readonly object _trava = new();
        private static bool _inicializado;

        // No SQLite as datas são guardadas como texto, sempre neste formato
        private const string FormatoDataHora = "yyyy-MM-dd HH:mm:ss";
        private const string FormatoData = "yyyy-MM-dd";

        private const string SqlCriarTabelas = @"
            CREATE TABLE IF NOT EXISTS curriculos (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nome TEXT NOT NULL,
                cargo TEXT NOT NULL,
                area TEXT NOT NULL,
                formacao TEXT NOT NULL,
                formacao_detalhada TEXT NULL,
                instituicao TEXT NULL,
                conclusao_formacao TEXT NULL,
                idade INTEGER NOT NULL,
                anos_experiencia INTEGER NOT NULL DEFAULT 0,
                foto_url TEXT NULL,
                email TEXT NOT NULL,
                telefone TEXT NULL,
                cidade TEXT NOT NULL,
                linkedin TEXT NULL,
                resumo TEXT NULL,
                status TEXT NOT NULL DEFAULT 'Novo',
                data_cadastro TEXT NOT NULL,
                data_atualizacao TEXT NULL
            );

            CREATE INDEX IF NOT EXISTS idx_curriculos_data ON curriculos (data_cadastro);
            CREATE INDEX IF NOT EXISTS idx_curriculos_status ON curriculos (status);

            CREATE TABLE IF NOT EXISTS curriculo_experiencias (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                curriculo_id INTEGER NOT NULL,
                cargo TEXT NOT NULL,
                empresa TEXT NOT NULL,
                periodo TEXT NULL,
                descricao TEXT NULL,
                ordem INTEGER NOT NULL DEFAULT 0,
                CONSTRAINT fk_experiencias_curriculo FOREIGN KEY (curriculo_id)
                    REFERENCES curriculos (id) ON DELETE CASCADE
            );

            CREATE INDEX IF NOT EXISTS idx_experiencias_curriculo ON curriculo_experiencias (curriculo_id);

            CREATE TABLE IF NOT EXISTS curriculo_habilidades (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                curriculo_id INTEGER NOT NULL,
                nome TEXT NOT NULL,
                CONSTRAINT fk_habilidades_curriculo FOREIGN KEY (curriculo_id)
                    REFERENCES curriculos (id) ON DELETE CASCADE
            );

            CREATE INDEX IF NOT EXISTS idx_habilidades_curriculo ON curriculo_habilidades (curriculo_id);

            CREATE TABLE IF NOT EXISTS perfis (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nome TEXT NOT NULL UNIQUE
            );

            -- COLLATE NOCASE: o e-mail não diferencia maiúsculas de minúsculas (como era no MySQL)
            CREATE TABLE IF NOT EXISTS usuarios (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nome TEXT NOT NULL,
                email TEXT NOT NULL UNIQUE COLLATE NOCASE,
                senha_hash TEXT NOT NULL,
                whatsapp TEXT NULL,
                data_nascimento TEXT NULL,
                foto_url TEXT NULL,
                data_cadastro TEXT DEFAULT (datetime('now', 'localtime')),
                ativo INTEGER DEFAULT 1
            );

            CREATE TABLE IF NOT EXISTS usuario_perfil (
                usuario_id INTEGER NOT NULL,
                perfil_id INTEGER NOT NULL,
                PRIMARY KEY (usuario_id, perfil_id),
                CONSTRAINT fk_usuario_perfil_usuario FOREIGN KEY (usuario_id)
                    REFERENCES usuarios (id) ON DELETE CASCADE,
                CONSTRAINT fk_usuario_perfil_perfil FOREIGN KEY (perfil_id)
                    REFERENCES perfis (id) ON DELETE CASCADE
            );

            CREATE INDEX IF NOT EXISTS idx_usuario_perfil_perfil ON usuario_perfil (perfil_id);

            -- Perfis iniciais (OR IGNORE: não duplica se já existirem)
            INSERT OR IGNORE INTO perfis (id, nome) VALUES
                (1, 'Candidato'),
                (2, 'Recrutador'),
                (3, 'Administrador');
        ";

        // Chamado no início do programa. Se der algum problema, só avisa no
        // console: a próxima tela que usar o banco tenta de novo.
        public static void TentarInicializar(ILogger logger)
        {
            try
            {
                GarantirInicializado();
                logger.LogInformation("Banco de dados pronto em {Caminho}.", Conexao.CaminhoDoBanco);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Não foi possível preparar o banco agora: {Mensagem}. " +
                                  "Verifique se o sistema pode gravar o arquivo {Caminho}.",
                                  ex.Message, Conexao.CaminhoDoBanco);
            }
        }

        public static void GarantirInicializado()
        {
            if (_inicializado) return;

            lock (_trava)
            {
                if (_inicializado) return;

                // Ao abrir, o SQLite cria o arquivo .db se ele ainda não existir
                using var conexao = Conexao.CriarConexao();
                conexao.Open();

                using (var comando = new SqliteCommand(SqlCriarTabelas, conexao))
                {
                    comando.ExecuteNonQuery();
                }

                using (var contar = new SqliteCommand("SELECT COUNT(*) FROM curriculos;", conexao))
                {
                    long total = Convert.ToInt64(contar.ExecuteScalar());
                    if (total == 0)
                    {
                        var repositorio = new CurriculoRepository(inicializarBanco: false);
                        foreach (var curriculo in DadosIniciais.Curriculos())
                        {
                            repositorio.Inserir(curriculo);
                        }
                    }
                }

                _inicializado = true;
            }
        }

        // ================= DATAS =================

        // Data e hora para gravar no banco (ex.: "2026-10-11 14:30:00")
        public static string FormatarDataHora(DateTime data)
        {
            return data.ToString(FormatoDataHora, CultureInfo.InvariantCulture);
        }

        // Só a data, sem hora (ex.: "1990-01-31")
        public static string FormatarData(DateTime data)
        {
            return data.ToString(FormatoData, CultureInfo.InvariantCulture);
        }

        // Lê uma data gravada como texto. Devolve null se estiver vazia ou inválida.
        public static DateTime? LerData(object? valor)
        {
            if (valor is not string texto || string.IsNullOrWhiteSpace(texto))
            {
                return null;
            }

            string[] formatos = { FormatoDataHora, FormatoData };

            if (DateTime.TryParseExact(texto, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var data) ||
                DateTime.TryParse(texto, CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
            {
                return data;
            }

            return null;
        }
    }
}
