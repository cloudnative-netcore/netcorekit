namespace NetCoreKit.Infrastructure.EfCore.MySql
{
    public class DbOptions
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DbInfo { get; set; } = "5.7.14-mysql";
        public string ConnString { get; set; } = "server={0};port={1};uid={2};pwd={3};database={4}";
        public string FQDN { get; set; }
    }
}
