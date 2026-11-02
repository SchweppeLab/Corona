using System.Windows;
using System.Windows.Threading;
using LandmineUI.Core.Theming;
using LandmineUI.Themes;
using LandmineUI.Wpf.Theming;

namespace Corona
{
  public partial class App : Application
  {
    private static ResourceDictionary? themeDictionary;

    /// <summary>
    /// Every theme LandmineUI ships, in the order the theme picker offers them.
    /// </summary>
    public static readonly ThemeTokens[] AvailableThemes =
    {
      FjordTokens.Value,
      MossPlumTokens.Value,
      NocturneTokens.Value,
      NightshadeTokens.Value,
      NightshadeHCTokens.Value,
      // The mid-tone themes LandmineUI added in 1.3.0, in the order it lists them.
      AmberDuskTokens.Value,
      AbyssalTealTokens.Value,
      UnderstoryTokens.Value,
      GoldenHourTokens.Value,
      KilnTokens.Value,
      CitronTokens.Value,
      StormSandTokens.Value,
    };

    public static ThemeTokens CurrentTheme { get; private set; } = FjordTokens.Value;
    public static ThemeMode CurrentThemeMode { get; private set; } = ThemeMode.Light;

    /// <summary>
    /// Raised after ApplyTheme so the plots, which ScottPlot paints itself, can restyle.
    /// Everything else follows the theme through DynamicResource on its own.
    /// </summary>
    public static event Action? ThemeChanged;

    protected override void OnStartup(StartupEventArgs e)
    {
      AppUserModelId.Apply(); // before any window exists; see the class comment
      DispatcherUnhandledException += OnDispatcherUnhandledException;
      base.OnStartup(e);

      AppSettings settings = AppSettings.Load();
      ThemeTokens theme = Array.Find(AvailableThemes, t => t.Name == settings.ThemeName) ?? FjordTokens.Value;
      ThemeMode mode = Enum.TryParse(settings.ThemeMode, out ThemeMode parsed) ? parsed : ThemeMode.Light;
      ApplyTheme(theme, mode);

      // Only the stock controls Corona uses are restyled. A control missing here keeps WPF's
      // own look, which in a dark theme means dark text on a dark surface.
      foreach (string control in new[]
      {
        "Window", "ToolBar", "Button", "CheckBox", "ComboBox", "TextBox", "ListBox", "DataGrid",
        "TabControl", "GroupBox", "Slider", "GridSplitter", "ScrollBar", "ScrollViewer", "ToolTip",
        "Separator", "Label", "Menu", "ContextMenu",
      })
      {
        Resources.MergedDictionaries.Add(new ResourceDictionary
        {
          Source = new Uri($"pack://application:,,,/LandmineUI.Wpf;component/Controls/{control}.xaml"),
        });
      }

      MainWindow = new MainWindow();
      MainWindow.Show();
    }

    /// <summary>
    /// Swaps the merged theme dictionary; every DynamicResource reference repaints at once.
    /// </summary>
    public static void ApplyTheme(ThemeTokens tokens, ThemeMode mode)
    {
      ResourceDictionary next = ThemeResources.Build(tokens, mode);
      var merged = Current.Resources.MergedDictionaries;
      if (themeDictionary is not null) merged.Remove(themeDictionary);
      merged.Insert(0, next);
      themeDictionary = next;
      CurrentTheme = tokens;
      CurrentThemeMode = mode;
      ThemeChanged?.Invoke();
    }

    // An exception on the UI thread would otherwise end the process, and with it the pipe
    // server every connected client depends on. Log it, show it, and carry on.
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      (MainWindow as MainWindow)?.ViewModel.ReportUnhandledException(e.Exception);
      MessageBox.Show(e.Exception.Message, "Corona", MessageBoxButton.OK, MessageBoxImage.Error);
      e.Handled = true;
    }
  }
}
