using MySqlConnector;
using TalentDesk.Models;
using BCrypt.Net;

namespace TalentDesk.Repositories
{
    public class UsuarioRepository
    {
        public bool EmailExiste(string email)
        {
            using var conexao = Conexao.CriarConexao();

            conexao.Open();

            string sql = @"
                SELECT COUNT(*)
                FROM usuarios
                WHERE email = @email;
            ";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@email", email);

            int quantidade = Convert.ToInt32(comando.ExecuteScalar());

            return quantidade > 0;
        }

        public Usuario? ObterPorEmail(string email)
        {
            using var conexao = Conexao.CriarConexao();
            conexao.Open();

            string sql = @"
                SELECT id, nome, email, senha_hash, whatsapp, data_nascimento, foto_url
                FROM usuarios
                WHERE email = @email;
            ";

            using var comando = new MySqlCommand(sql, conexao);
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
                DataNascimento = leitor["data_nascimento"] == DBNull.Value ? null : leitor.GetDateTime("data_nascimento"),
                FotoUrl = leitor["foto_url"] as string
            };
        }

        // Nome do perfil do usuário (Candidato, Recrutador ou Administrador)
        public string ObterPerfil(int usuarioId)
        {
            using var conexao = Conexao.CriarConexao();
            conexao.Open();

            string sql = @"
                SELECT p.nome
                FROM usuario_perfil up
                INNER JOIN perfis p ON p.id = up.perfil_id
                WHERE up.usuario_id = @usuario_id
                ORDER BY p.id DESC
                LIMIT 1;
            ";

            using var comando = new MySqlCommand(sql, conexao);
            comando.Parameters.AddWithValue("@usuario_id", usuarioId);

            return comando.ExecuteScalar() as string ?? "Usuário";
        }

        public int Cadastrar(Usuario usuario)
        {
            using var conexao = Conexao.CriarConexao();

            conexao.Open();

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

                    SELECT LAST_INSERT_ID();
                ";

                using var comandoUsuario =
                    new MySqlCommand(sqlUsuario, conexao, transacao);

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

                comandoUsuario.Parameters.AddWithValue(
                    "@data_nascimento",
                    (object?)usuario.DataNascimento ?? DBNull.Value
                );

                comandoUsuario.Parameters.AddWithValue(
                    "@foto_url",
                    (object?)usuario.FotoUrl ?? DBNull.Value
                );

                int usuarioId =
                    Convert.ToInt32(comandoUsuario.ExecuteScalar());

                // Busca o perfil Candidato
                string sqlPerfil = @"
                    SELECT id
                    FROM perfis
                    WHERE nome = 'Candidato';
                ";

                using var comandoPerfil =
                    new MySqlCommand(sqlPerfil, conexao, transacao);

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
                    new MySqlCommand(
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