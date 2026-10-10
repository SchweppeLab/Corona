using System.Reflection;

namespace Corona
{
  /// <summary>
  /// The version Corona shows in its title bar and log: the repo version from
  /// Directory.Build.props, plus the -dev.N suffix CI adds to development builds.
  /// </summary>
  public static class AppVersion
  {
    public static string Display { get; } = Compute();

    private static string Compute()
    {
      Assembly assembly = typeof(AppVersion).Assembly;
      string? informational = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
      if (string.IsNullOrEmpty(informational))
      {
        return assembly.GetName().Version?.ToString(3) ?? "unknown";
      }
      // The SDK appends "+<commit>" to the informational version; that is noise in a title bar.
      int plus = informational.IndexOf('+');
      return plus > 0 ? informational[..plus] : informational;
    }
  }
}
