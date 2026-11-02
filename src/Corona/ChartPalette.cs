using System.Windows;
using ScottPlot;

namespace Corona
{
  /// <summary>
  /// The current theme's colors as ScottPlot colors, read from the dictionary App.ApplyTheme
  /// merged into Application.Resources, so the plots track the theme. Read fresh on every
  /// call: the theme can change at any time and the plots restyle on App.ThemeChanged.
  /// </summary>
  public static class ChartPalette
  {
    public static Color Background => Resolve("BackgroundColor", "#ffffff");
    public static Color Surface => Resolve("SurfaceColor", "#ffffff");
    public static Color TextPrimary => Resolve("TextPrimaryColor", "#202020");
    public static Color TextSecondary => Resolve("TextSecondaryColor", "#606060");
    public static Color Accent => Resolve("AccentColor", "#1f77b4");
    public static Color Primary => Resolve("PrimaryColor", "#1f77b4");

    // The fallbacks are a plain light look for a plot built before the theme is merged.
    private static Color Resolve(string resourceKey, string fallbackHex)
    {
      if (Application.Current?.Resources[resourceKey] is System.Windows.Media.Color wpfColor)
      {
        return new Color($"#{wpfColor.R:X2}{wpfColor.G:X2}{wpfColor.B:X2}");
      }
      return new Color(fallbackHex);
    }
  }
}
