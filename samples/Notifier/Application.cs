namespace NetCoreKit.Samples.Notifier
{
  public interface IApplication
  {
    void Run();
  }

  public class Application : IApplication
  {
    public void Run()
    {
      throw new System.NotImplementedException();
    }
  }
}
