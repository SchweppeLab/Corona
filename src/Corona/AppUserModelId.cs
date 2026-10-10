using System.Runtime.InteropServices;

namespace Corona
{
  /// <summary>
  /// Tells the Windows shell which taskbar identity this process has. The release root's
  /// launcher (src\Corona.Launcher, Corona.exe) declares the same Id before starting this
  /// process, so the two are one application to the shell rather than two: a pin made from
  /// the running window groups with it, and nothing shows up twice. Must be applied before
  /// the first window is created, so App.OnStartup calls it first thing.
  /// </summary>
  public static class AppUserModelId
  {
    // Must match LAUNCHER_APP_USER_MODEL_ID in src\Corona.Launcher\launcher_config.h.
    public const string Id = "SchweppeLab.Corona";

    public static void Apply()
    {
      // The return value is an HRESULT; failure here (an exotic shell) only costs taskbar
      // grouping, never the app, so it is deliberately not checked.
      SetCurrentProcessExplicitAppUserModelID(Id);
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SetCurrentProcessExplicitAppUserModelID(string appId);
  }
}
