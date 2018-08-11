using System;

namespace NetCoreKit.Utils.Helpers
{
  public static class DateTimeHelper
  {
    public static DateTime GenerateDateTime()
    {
      return DateTimeOffset.Now.UtcDateTime;
    }
  }
}
