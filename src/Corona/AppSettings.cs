using System.IO;
using System.Text.Json;

namespace Corona
{
  /// <summary>
  /// The user's last theme choice, kept per user under %AppData%\Corona. A missing or
  /// unreadable file means defaults; this is a convenience, not data worth an error dialog.
  /// </summary>
  public sealed class AppSettings
  {
    private static readonly string FilePath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Corona", "settings.json");

    public string ThemeName { get; set; } = "Fjord";
    public string ThemeMode { get; set; } = "Light";

    public static AppSettings Load()
    {
      try
      {
        return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath)) ?? new AppSettings();
      }
      catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
      {
        return new AppSettings();
      }
    }

    public void Save()
    {
      try
      {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
      }
      catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
      {
        // Losing the preference costs one re-selection next start; not worth interrupting a run.
      }
    }
  }
}
