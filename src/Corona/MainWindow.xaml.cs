using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ScottPlot.Plottables;

namespace VirtualMS
{
  public partial class MainWindow : Window
  {
    public CoronaViewModel ViewModel { get; }

    private readonly DataLogger plot;
    private volatile bool refreshSpectrum = false;

    private readonly DispatcherTimer plotRefreshTimer = new() { Interval = TimeSpan.FromMilliseconds(100) };

    public MainWindow()
    {
      InitializeComponent();

      ViewModel = new CoronaViewModel();
      DataContext = ViewModel;

      RunSpanColumn.ItemsSource = new[] { "Spectrum", "Retention Time" };

      plotTIC.UserInputProcessor.Disable();
      plot = plotTIC.Plot.Add.DataLogger();
      plot.Add(0, 0);
      plot.Add(0.000001, 1);
      plotTIC.Refresh();
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
      plotSpectrum.Refresh();

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

    private void ToolBar_Loaded(object sender, RoutedEventArgs e)
    {
      if (sender is not ToolBar toolBar) return;
      if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
        overflowGrid.Visibility = Visibility.Collapsed;
      if (toolBar.Template.FindName("MainPanelBorder", toolBar) is FrameworkElement mainPanelBorder)
        mainPanelBorder.Margin = new Thickness(0);
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
