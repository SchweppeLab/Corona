using System.Windows;
using System.Windows.Threading;

namespace Corona
{
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      AppUserModelId.Apply(); // before any window exists; see the class comment
      DispatcherUnhandledException += OnDispatcherUnhandledException;
      base.OnStartup(e);
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
