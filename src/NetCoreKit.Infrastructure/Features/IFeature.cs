namespace NetCoreKit.Infrastructure.Features
{
    /// <summary>
    ///     Reference at https://github.com/anuraj/AspNetCoreSamples/tree/master/FeatureToggle
    /// </summary>
    public interface IFeature
    {
        bool IsEnabled(string feature);
    }
}
