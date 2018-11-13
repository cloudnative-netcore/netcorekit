namespace NetCoreKit.Infrastructure.EfCore.MySql.Options
{
  public class DbOptions
  {
    public string Host { get; set; }
    public string Port { get; set; }
    public string Database { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string DbInfo { get; set; } = "5.7.14-mysql";
  }
}
