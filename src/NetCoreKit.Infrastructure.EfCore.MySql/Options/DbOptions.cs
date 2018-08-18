namespace NetCoreKit.Infrastructure.EfCore.MySql.Options
{
  public class DbOptions
  {
    public string Host { get; set; }
    public string Port { get; set; }
    public string Database { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int Major { get; set; } = 8;
    public int Minor { get; set; } = 0;
    public int Build { get; set; } = 12;
    public int DbType { get; set; } = 0; // mysql by default, 1 for mariadb
  }
}
