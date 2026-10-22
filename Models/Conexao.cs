using System.Globalization;
using System.Text;
using Microsoft.Data.Sqlite;

namespace TalentDesk
{
    public class Conexao
    {
        // O banco é um arquivo só: %LOCALAPPDATA%\TalentDesk\talentdesk.db
        // (ex.: C:\Users\SeuUsuario\AppData\Local\TalentDesk\talentdesk.db).
        // Fica fora da pasta do projeto para não ser versionado nem sincronizado
        // por serviços de nuvem enquanto está em uso.
        private static readonly string pastaDoBanco = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TalentDesk");

        public static readonly string CaminhoDoBanco =
            Path.Combine(pastaDoBanco, "talentdesk.db");

        // "Foreign Keys = True" faz o SQLite executar "PRAGMA foreign_keys = ON;" toda
        // vez que a conexão é aberta. Sem isso o ON DELETE CASCADE não funciona.
        private static readonly string connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = CaminhoDoBanco,
            ForeignKeys = true
        }.ToString();

        public static SqliteConnection CriarConexao()
        {
            // Cria a pasta se não existir. O arquivo .db o próprio SQLite cria ao abrir.
            Directory.CreateDirectory(pastaDoBanco);

            var conexao = new SqliteConnection(connectionString);

            // Função usada nas buscas e ordenações: sem_acento('São Paulo') = 'sao paulo'
            conexao.CreateFunction("sem_acento", (string? texto) => SemAcento(texto));

            return conexao;
        }

        // Tira os acentos e passa para minúsculas, para a busca não diferenciar
        // "JOÃO", "joão" e "joao" (o LIKE do SQLite só ignora maiúsculas em letras sem acento)
        public static string? SemAcento(string? texto)
        {
            if (texto == null) return null;

            var resultado = new StringBuilder(texto.Length);
            foreach (char letra in texto.Normalize(NormalizationForm.FormD))
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letra) != UnicodeCategory.NonSpacingMark)
                {
                    resultado.Append(letra);
                }
            }

            return resultado.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }
    }
}
