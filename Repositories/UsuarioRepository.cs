using System.Data;
using Microsoft.Data.Sqlite;
using TalentDesk.Models;
using BCrypt.Net;

namespace TalentDesk.Repositories
{
    public class UsuarioRepository
    {
        // Abre a conexão garantindo antes que as tabelas existem
        private static SqliteConnection AbrirConexao()
        {
            BancoDeDados.GarantirInicializado();

            var conexao = Conexao.CriarConexao();
            conexao.Open();
            return conexao;
        }

        public bool EmailExiste(string email)
        {
            using var conexao = AbrirConexao();

            string sql = @"
                SELECT COUNT(*)
                FROM usuarios
                WHERE email = @email;
            ";

            using var comando = new SqliteCommand(sql, conexao);

            comando.Parameters.AddWithValue("@email", email);

            int quantidade = Convert.ToInt32(comando.ExecuteScalar());

            return quantidade > 0;
        }

        public Usuario? ObterPorEmail(string email)
        {
            using var conexao = AbrirConexao();

            string sql = @"
                SELECT id, nome, email, senha_hash, whatsapp, data_nascimento, foto_url
                FROM usuarios
                WHERE email = @email;
            ";

            using var comando = new SqliteCommand(sql, conexao);
            comando.Parameters.AddWithValue("@email", email);

            using var leitor = comando.ExecuteReader();
            if (!leitor.Read())
            {
                return null;
            }

            return new Usuario
            {
                Id = leitor.GetInt32("id"),
                Nome = leitor.GetString("nome"),
                Email = leitor.GetString("email"),
                Senha = leitor["senha_hash"] as string ?? string.Empty,
                Whatsapp = leitor["whatsapp"] as string,
                // A data vem como texto (ou nula) e é convertida com segurança
                DataNascimento = BancoDeDados.LerData(leitor["data_nascimento"]),
                FotoUrl = leitor["foto_url"] as string
            };
        }

        // Nome do perfil do usuário (Candidato, Recrutador ou Administrador)
        public string ObterPerfil(int usuarioId)
        {
            using var conexao = AbrirConexao();

            string sql = @"
                SELECT p.nome
                FROM usuario_perfil up
                INNER JOIN perfis p ON p.id = up.perfil_id
                WHERE up.usuario_id = @usuario_id
                ORDER BY p.id DESC
                LIMIT 1;
            ";

            using var comando = new SqliteCommand(sql, conexao);
            comando.Parameters.AddWithValue("@usuario_id", usuarioId);

            return comando.ExecuteScalar() as string ?? "Usuário";
        }

        public int Cadastrar(Usuario usuario)
        {
            using var conexao = AbrirConexao();

            using var transacao = conexao.BeginTransaction();

            try
            {
                // Cria o hash da senha
                string senhaHash =
                    BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

                // Cadastra o usuário
                string sqlUsuario = @"
                    INSERT INTO usuarios
                    (
                        nome,
                        email,
                        senha_hash,
                        whatsapp,
                        data_nascimento,
                        foto_url
                    )
                    VALUES
                    (
                        @nome,
                        @email,
                        @senha_hash,
                        @whatsapp,
                        @data_nascimento,
                        @foto_url
                    );

                    SELECT last_insert_rowid();
                ";

                using var comandoUsuario =
                    new SqliteCommand(sqlUsuario, conexao, transacao);

                comandoUsuario.Parameters.AddWithValue(
                    "@nome",
                    usuario.Nome
                );

                comandoUsuario.Parameters.AddWithValue(
                    "@email",
                    usuario.Email
                );

                comandoUsuario.Parameters.AddWithValue(
                    "@senha_hash",
                    senhaHash
                );

                comandoUsuario.Parameters.AddWithValue(
                    "@whatsapp",
                    (object?)usuario.Whatsapp ?? DBNull.Value
                );

                // Data de nascimento gravada como texto (ex.: "1990-01-31")
                comandoUsuario.Parameters.AddWithValue(
                    "@data_nascimento",
                    usuario.DataNascimento.HasValue
                        ? (object)BancoDeDados.FormatarData(usuario.DataNascimento.Value)
                        : DBNull.Value
                );

                comandoUsuario.Parameters.AddWithValue(
                    "@foto_url",
                    (object?)usuario.FotoUrl ?? DBNull.Value
                );

                // last_insert_rowid() devolve long; o id cabe em int
                int usuarioId =
                    Convert.ToInt32(comandoUsuario.ExecuteScalar());

                // Busca o perfil Candidato
                string sqlPerfil = @"
                    SELECT id
                    FROM perfis
                    WHERE nome = 'Candidato';
                ";

                using var comandoPerfil =
                    new SqliteCommand(sqlPerfil, conexao, transacao);

                object? resultadoPerfil =
                    comandoPerfil.ExecuteScalar();

                if (resultadoPerfil == null)
                {
                    throw new Exception(
                        "O perfil Candidato não foi encontrado."
                    );
                }

                int perfilId =
                    Convert.ToInt32(resultadoPerfil);

                // Relaciona o usuário ao perfil
                string sqlUsuarioPerfil = @"
                    INSERT INTO usuario_perfil
                    (
                        usuario_id,
                        perfil_id
                    )
                    VALUES
                    (
                        @usuario_id,
                        @perfil_id
                    );
                ";

                using var comandoUsuarioPerfil =
                    new SqliteCommand(
                        sqlUsuarioPerfil,
                        conexao,
                        transacao
                    );

                comandoUsuarioPerfil.Parameters.AddWithValue(
                    "@usuario_id",
                    usuarioId
                );

                comandoUsuarioPerfil.Parameters.AddWithValue(
                    "@perfil_id",
                    perfilId
                );

                comandoUsuarioPerfil.ExecuteNonQuery();

                // Confirma todas as operações
                transacao.Commit();

                return usuarioId;
            }
            catch
            {
                // Desfaz tudo caso alguma etapa dê erro
                transacao.Rollback();

                throw;
            }
        }
    }
}
