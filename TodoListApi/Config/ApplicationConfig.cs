#nullable disable
namespace TodoListApi.Config
{
    public class ApplicationConfig
    {
        public JwtConfig JwtConfig{ get; set; } = new JwtConfig();
        public DbConfig DbConfig{ get; set; } = new DbConfig();
        public AesEncryptionConfig AesEncryptionConfig{ get; set; } = new AesEncryptionConfig();
    }

    public class AesEncryptionConfig
    {
        public string Key { get; set; }
        public string Iv { get; set; }
    }

    public class JwtConfig
    {
        public string Key { get; set; }
        public string Scheme { get; set; }
        public int ExpiresIn { get; set; }
    }

    public class DbConfig
    {
        public string Server { get; set;}
        public string Port { get; set;}
        public string User { get; set;}
        public string Password { get; set;}
        public string Database { get; set;}

        public string getConnectionString(){
            return $"server={Server};port={Port};user={User};password={Password};database={Database};";
        }
    }
}