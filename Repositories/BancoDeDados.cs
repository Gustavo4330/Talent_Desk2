using MySqlConnector;

namespace TalentDesk.Repositories
{
    // Cria as tabelas de currículos (se ainda não existirem) e cadastra os
    // currículos de exemplo na primeira vez que o sistema roda.
    public static class BancoDeDados
    {
        private static readonly object _trava = new();
        private static bool _inicializado;

        private const string SqlCriarTabelas = @"
            CREATE TABLE IF NOT EXISTS curriculos (
                id INT NOT NULL AUTO_INCREMENT,
                nome VARCHAR(150) NOT NULL,
                cargo VARCHAR(150) NOT NULL,
                area VARCHAR(100) NOT NULL,
                formacao VARCHAR(150) NOT NULL,
                formacao_detalhada VARCHAR(200) NULL,
                instituicao VARCHAR(150) NULL,
                conclusao_formacao VARCHAR(50) NULL,
                idade INT NOT NULL,
                anos_experiencia INT NOT NULL DEFAULT 0,
                foto_url VARCHAR(500) NULL,
                email VARCHAR(150) NOT NULL,
                telefone VARCHAR(30) NULL,
                cidade VARCHAR(100) NOT NULL,
                linkedin VARCHAR(200) NULL,
                resumo TEXT NULL,
                status VARCHAR(20) NOT NULL DEFAULT 'Novo',
                data_cadastro DATETIME NOT NULL,
                data_atualizacao DATETIME NULL,
                PRIMARY KEY (id),
                KEY idx_curriculos_data (data_cadastro),
                KEY idx_curriculos_status (status)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

            CREATE TABLE IF NOT EXISTS curriculo_experiencias (
                id INT NOT NULL AUTO_INCREMENT,
                curriculo_id INT NOT NULL,
                cargo VARCHAR(150) NOT NULL,
                empresa VARCHAR(150) NOT NULL,
                periodo VARCHAR(60) NULL,
                descricao TEXT NULL,
                ordem INT NOT NULL DEFAULT 0,
                PRIMARY KEY (id),
                KEY idx_experiencias_curriculo (curriculo_id),
                CONSTRAINT fk_experiencias_curriculo FOREIGN KEY (curriculo_id)
                    REFERENCES curriculos (id) ON DELETE CASCADE
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

            CREATE TABLE IF NOT EXISTS curriculo_habilidades (
                id INT NOT NULL AUTO_INCREMENT,
                curriculo_id INT NOT NULL,
                nome VARCHAR(80) NOT NULL,
                PRIMARY KEY (id),
                KEY idx_habilidades_curriculo (curriculo_id),
                CONSTRAINT fk_habilidades_curriculo FOREIGN KEY (curriculo_id)
                    REFERENCES curriculos (id) ON DELETE CASCADE
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
        ";

        // Chamado no início do programa. Se o MySQL estiver desligado, só avisa no
        // console: a próxima tela que usar o banco tenta de novo.
        public static void TentarInicializar(ILogger logger)
        {
            try
            {
                GarantirInicializado();
                logger.LogInformation("Banco de dados pronto (tabelas de currículos verificadas).");
            }
            catch (Exception ex)
            {
                logger.LogWarning("Não foi possível preparar o banco agora: {Mensagem}. " +
                                  "Verifique se o MySQL do XAMPP está ligado.", ex.Message);
            }
        }

        public static void GarantirInicializado()
        {
            if (_inicializado) return;

            lock (_trava)
            {
                if (_inicializado) return;

                using var conexao = Conexao.CriarConexao();
                conexao.Open();

                using (var comando = new MySqlCommand(SqlCriarTabelas, conexao))
                {
                    comando.ExecuteNonQuery();
                }

                using (var contar = new MySqlCommand("SELECT COUNT(*) FROM curriculos;", conexao))
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
    }
}
