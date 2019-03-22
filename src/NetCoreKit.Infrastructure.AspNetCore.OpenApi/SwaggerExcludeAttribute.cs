using System;

namespace NetCoreKit.Infrastructure.AspNetCore.OpenApi
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SwaggerExcludeAttribute : Attribute
    {
    }
}
