namespace NetCoreKit.Infrastructure.EfCore.SqlServer.Options
{
  public class DbOptions
  {
    public string Host { get; set; }
    public string Port { get; set; }
    public string Database { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
  }
}
