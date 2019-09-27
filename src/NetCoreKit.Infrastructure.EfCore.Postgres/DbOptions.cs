namespace NetCoreKit.Infrastructure.EfCore.Postgres
{
    public class DbOptions
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string ConnString { get; set; } = "Host={0};Port={1};User ID={2};Password={3};Database={4}";
        public string FQDN { get; set; }
    }
}
