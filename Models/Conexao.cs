using MySqlConnector;

namespace TalentDesk
{
    public class Conexao
    {
        private const string connectionString =
            "Server=localhost;Port=3306;Database=talent_desk;User ID=root;Password=;";

        public static MySqlConnection CriarConexao()
        {
            return new MySqlConnection(connectionString);
        }
    }
}