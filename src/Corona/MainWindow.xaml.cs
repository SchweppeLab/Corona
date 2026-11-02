using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using LandmineUI.Core.Theming;
using ScottPlot.Plottables;

namespace Corona
{
  public partial class MainWindow : Window
  {
    public CoronaViewModel ViewModel { get; }

    private readonly DataLogger plot;
    private volatile bool refreshSpectrum = false;

    // Read on the UI thread in StylePlots and used on the replay thread in OnSpectrumReady,
    // which must not touch Application.Resources.
    private ScottPlot.Color spectrumColor = ChartPalette.Primary;

    private readonly DispatcherTimer plotRefreshTimer = new() { Interval = TimeSpan.FromMilliseconds(100) };

    public MainWindow()
    {
      InitializeComponent();
      Title = $"Corona v{AppVersion.Display}";

      ViewModel = new CoronaViewModel();
      DataContext = ViewModel;

      RunSpanColumn.ItemsSource = new[] { "Spectrum", "Retention Time" };

      plotTIC.UserInputProcessor.Disable();
      plot = plotTIC.Plot.Add.DataLogger();
      plot.Add(0, 0);
      plot.Add(0.000001, 1);
      plotTIC.Plot.Title("TIC");
      plotTIC.Plot.XLabel("Retention Time (min)");
      plotTIC.Plot.YLabel("TIC");
      plotTIC.Plot.HideGrid();

      ScottPlot.Coordinates[] co = { new(1, 1), new(2, 2) };
      var initialSpec = plotSpectrum.Plot.Add.Scatter(co);
      initialSpec.MarkerSize = 1;
      plotSpectrum.Plot.Axes.SetupMultiplierNotation(plotSpectrum.Plot.Axes.Left);
      plotSpectrum.Plot.YLabel("Intensity");
      plotSpectrum.Plot.HideGrid();

      StylePlots();
      // This window lives for the whole process, so a plain += never leaks.
      App.ThemeChanged += StylePlots;

      ThemeCombo.ItemsSource = App.AvailableThemes.Select(t => t.Name).ToList();
      ThemeCombo.SelectedItem = App.CurrentTheme.Name;
      DarkCheck.IsChecked = App.CurrentThemeMode == ThemeMode.Dark;

      ViewModel.TicPointArrived += OnTicPointArrived;
      ViewModel.SpectrumReady += OnSpectrumReady;
      ViewModel.TicResetRequested += OnTicResetRequested;
      ViewModel.AcquisitionTicResetRequested += OnAcquisitionTicResetRequested;
      ViewModel.PropertyChanged += (s, e) =>
      {
        if (e.PropertyName == nameof(CoronaViewModel.LogText)) LogTextBox.ScrollToEnd();
      };

      plotRefreshTimer.Tick += (s, e) =>
      {
        if (plot.HasNewData) plotTIC.Refresh();
        if (refreshSpectrum)
        {
          plotSpectrum.Refresh();
          refreshSpectrum = false;
        }
        ViewModel.RefreshStatsIfDirty();
      };
      plotRefreshTimer.Start();
    }

    // ScottPlot paints its own surface, so the theme reaches it here rather than through
    // DynamicResource. Runs at construction and again on every theme change.
    private void StylePlots()
    {
      spectrumColor = ChartPalette.Primary;
      foreach (var control in new[] { plotTIC, plotSpectrum })
      {
        lock (control.Plot.Sync)
        {
          control.Plot.FigureBackground.Color = ChartPalette.Background;
          control.Plot.DataBackground.Color = ChartPalette.Surface;
          control.Plot.Axes.Color(ChartPalette.TextSecondary);
          control.Plot.Axes.Title.Label.ForeColor = ChartPalette.TextPrimary;
          foreach (var scatter in control.Plot.GetPlottables<Scatter>()) scatter.Color = spectrumColor;
        }
        control.Refresh();
      }
      lock (plotTIC.Plot.Sync) plot.Color = ChartPalette.Accent;
      plotTIC.Refresh();
    }

    private void ThemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (ThemeCombo.SelectedItem is not string name) return;
      var theme = Array.Find(App.AvailableThemes, t => t.Name == name);
      if (theme is null || theme == App.CurrentTheme) return;
      App.ApplyTheme(theme, App.CurrentThemeMode);
      SaveThemeChoice();
    }

    private void DarkCheck_Click(object sender, RoutedEventArgs e)
    {
      App.ApplyTheme(App.CurrentTheme, DarkCheck.IsChecked == true ? ThemeMode.Dark : ThemeMode.Light);
      SaveThemeChoice();
    }

    private static void SaveThemeChoice()
    {
      new AppSettings { ThemeName = App.CurrentTheme.Name, ThemeMode = App.CurrentThemeMode.ToString() }.Save();
    }

    // Fired directly from the background thread SimRunner raises MsScanArrived on - mirrors the
    // original's un-marshaled, Plot.Sync-locked ScottPlot appends. Do not dispatch this.
    private void OnTicPointArrived(double rt, double tic)
    {
      lock (plotTIC.Plot.Sync)
      {
        plot.Add(rt, tic);
        plotTIC.Plot.Axes.SetupMultiplierNotation(plotTIC.Plot.Axes.Left);
      }
    }

    private void OnSpectrumReady(SpectrumUpdate update)
    {
      lock (plotSpectrum.Plot.Sync)
      {
        plotSpectrum.Plot.Clear();
        var scat = plotSpectrum.Plot.Add.Scatter(update.X, update.Y);
        scat.MarkerSize = 1;
        scat.Color = spectrumColor;
        plotSpectrum.Plot.Axes.SetLimitsX(update.StartMz, update.EndMz);
        plotSpectrum.Plot.Axes.SetLimitsY(0, 100);
      }
      refreshSpectrum = true;
    }

    private void OnTicResetRequested()
    {
      lock (plotTIC.Plot.Sync)
      {
        plot.Clear();
        plot.Add(0, 0);
      }
      plotTIC.Refresh();
    }

    private void OnAcquisitionTicResetRequested()
    {
      lock (plotTIC.Plot.Sync)
      {
        plot.Clear();
        plot.Add(0, 0);
        plot.Add(0.000001, 1);
        plotTIC.Plot.Axes.SetLimitsY(0, 1);
      }
      plotTIC.Refresh();
    }

    private void QueueListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ViewModel.QueueSelectionCount = QueueListBox.SelectedItems.Count;
      ViewModel.UpdateQueueButtonState();
    }

    private void RunParamsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ViewModel.FileListSelectionCount = RunParamsGrid.SelectedItems.Count;
      ViewModel.UpdateFileListButtonStates();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
      List<FileRow> rows = RunParamsGrid.SelectedItems.Cast<FileRow>()
          .OrderBy(r => ViewModel.FileList.IndexOf(r))
          .ToList();
      ViewModel.AddFilesToQueue(rows);
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
      ViewModel.DeleteFromFileList(RunParamsGrid.SelectedItems.Cast<FileRow>().ToList());
    }

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
    {
      List<int> indices = QueueListBox.SelectedItems.Cast<FileStats>()
          .Select(item => ViewModel.FileQueue.IndexOf(item))
          .ToList();
      ViewModel.RemoveFromQueue(indices);
    }

    private void RunParamsGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
    {
      string? header = e.Column.Header as string;
      if (header != "Start" && header != "End") return;
      if (e.EditingElement is not TextBox tb) return;
      tb.PreviewTextInput -= NumericPreviewTextInput;
      tb.PreviewTextInput += NumericPreviewTextInput;
    }

    private static void NumericPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      TextBox tb = (TextBox)sender;
      foreach (char c in e.Text)
      {
        if (!char.IsDigit(c) && c != '.')
        {
          e.Handled = true;
          return;
        }
        if (c == '.' && tb.Text.Contains('.'))
        {
          e.Handled = true;
          return;
        }
      }
    }
  }
}
