using Microsoft.Extensions.Configuration;

namespace NetCoreKit.Infrastructure.Features
{
  public class Feature : IFeature
  {
    private readonly IConfiguration _config;

    public Feature(IConfiguration config)
    {
      _config = config;
    }

    public bool IsEnabled(string feature)
    {
      var featureValue = _config.GetValue($"Features:{feature}:Enabled", false);
      return featureValue;
    }
  }
}
