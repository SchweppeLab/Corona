using System.Windows;

namespace Corona
{
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      AppUserModelId.Apply(); // before any window exists; see the class comment
      base.OnStartup(e);
    }
  }
}
