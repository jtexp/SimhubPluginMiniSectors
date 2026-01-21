using MahApps.Metro.IconPacks;
using SimHub.Plugins.Styles;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace User.PluginMiniSectors
{
    public partial class SettingsControl : UserControl
    {
        private readonly DataPluginMiniSectors _plugin;
        private readonly UpdateChecker _updateChecker;
        private UpdateCheckResult _lastCheckResult;

        // Records tab state
        private bool _recordsLoaded = false;
        private bool _allRecordsLoaded = false;

        public DataPluginMiniSectors Plugin => _plugin;

        public SettingsControl()
        {
            InitializeComponent();
            _updateChecker = new UpdateChecker();
            Loaded += SettingsControl_Loaded;
        }

        public SettingsControl(DataPluginMiniSectors plugin) : this()
        {
            _plugin = plugin;
        }

        private void SettingsControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Display current version info
            CurrentVersionText.Text = UpdateChecker.GetCurrentVersion();

            var commit = UpdateChecker.GetCurrentCommit();
            if (commit == "unknown")
            {
                CurrentCommitText.Text = "(development build)";
            }
            else if (commit.Length > 7)
            {
                CurrentCommitText.Text = commit.Substring(0, 7);
            }
            else
            {
                CurrentCommitText.Text = commit;
            }
        }

        private async void CheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Disable button and show checking state
            CheckUpdateButton.IsEnabled = false;
            CheckUpdateButton.Content = "Checking...";
            UpdateButton.Visibility = Visibility.Collapsed;
            UpdateDetailsPanel.Visibility = Visibility.Collapsed;

            try
            {
                _lastCheckResult = await _updateChecker.CheckForUpdateAsync();

                StatusBorder.Visibility = Visibility.Visible;

                if (!string.IsNullOrEmpty(_lastCheckResult.ErrorMessage))
                {
                    ShowStatus(_lastCheckResult.ErrorMessage, PackIconMaterialKind.AlertCircle, Brushes.Orange);
                }
                else if (_lastCheckResult.UpdateAvailable)
                {
                    ShowStatus("Update available!", PackIconMaterialKind.Download, Brushes.LightGreen);

                    // Show update details
                    UpdateDetailsPanel.Visibility = Visibility.Visible;
                    NewCommitText.Text = _lastCheckResult.LatestCommit?.Length > 7
                        ? _lastCheckResult.LatestCommit.Substring(0, 7)
                        : _lastCheckResult.LatestCommit ?? "unknown";
                    BuildDateText.Text = _lastCheckResult.ReleaseDate ?? "unknown";

                    // Show update button
                    UpdateButton.Visibility = Visibility.Visible;
                }
                else
                {
                    ShowStatus("You're up to date!", PackIconMaterialKind.CheckCircle, Brushes.LightGreen);
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Error: {ex.Message}", PackIconMaterialKind.AlertCircle, Brushes.Red);
            }
            finally
            {
                CheckUpdateButton.IsEnabled = true;
                CheckUpdateButton.Content = "Check for Updates";
            }
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            if (_lastCheckResult == null || !_lastCheckResult.UpdateAvailable)
            {
                return;
            }

            // Disable buttons
            CheckUpdateButton.IsEnabled = false;
            UpdateButton.IsEnabled = false;
            UpdateButton.Content = "Downloading...";

            // Show progress
            ProgressPanel.Visibility = Visibility.Visible;
            DownloadProgress.Value = 0;

            try
            {
                // Download the update
                var newDllPath = await _updateChecker.DownloadUpdateAsync(
                    _lastCheckResult.DownloadUrl,
                    progress =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            DownloadProgress.Value = progress;
                            ProgressText.Text = $"Downloading... {progress}%";
                        });
                    });

                ProgressText.Text = "Starting updater...";

                // Compute hash of downloaded DLL for updater verification
                var dllHash = UpdateChecker.ComputeSHA256(newDllPath);

                // Launch updater
                _updateChecker.LaunchUpdater(newDllPath, dllHash);

                // Request SimHub shutdown
                ShowStatus("Update starting... SimHub will restart automatically.", PackIconMaterialKind.Restart, Brushes.LightBlue);

                // Give user a moment to see the message, then shutdown
                await System.Threading.Tasks.Task.Delay(1500);

                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                ShowStatus($"Update failed: {ex.Message}", PackIconMaterialKind.AlertCircle, Brushes.Red);
                SimHub.Logging.Current.Error($"Update failed: {ex}");

                ProgressPanel.Visibility = Visibility.Collapsed;
                UpdateButton.IsEnabled = true;
                UpdateButton.Content = "Update Now";
                CheckUpdateButton.IsEnabled = true;
            }
        }

        private void GitHubLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/jtexp/User.PluginMiniSectors",
                UseShellExecute = true
            });
        }

        private void ShowStatus(string message, PackIconMaterialKind icon, Brush color)
        {
            _lastStatusMessage = message;
            StatusBorder.Visibility = Visibility.Visible;
            StatusText.Text = message;
            StatusIcon.Kind = icon;
            StatusIcon.Foreground = color;
        }

        private string _lastStatusMessage;

        private void StatusBorder_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(_lastStatusMessage))
            {
                try
                {
                    Clipboard.SetText(_lastStatusMessage);
                    // Brief visual feedback - flash the background
                    var originalBackground = StatusBorder.Background;
                    StatusBorder.Background = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x55));
                    var timer = new System.Windows.Threading.DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(150)
                    };
                    timer.Tick += (s, args) =>
                    {
                        StatusBorder.Background = originalBackground;
                        timer.Stop();
                    };
                    timer.Start();
                }
                catch
                {
                    // Clipboard access can fail, ignore
                }
            }
        }

        // ----------------------------------------------------------------
        // Records Tab
        // ----------------------------------------------------------------

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Only handle our own tab control's selection changes
            if (e.Source != MainTabControl)
                return;

            // Auto-load when Records tab is selected for the first time
            if (MainTabControl.SelectedItem == RecordsTab && !_recordsLoaded)
            {
                LoadRecords(limit: 500);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadRecords(_allRecordsLoaded ? -1 : 500);
        }

        private void LoadAll_Click(object sender, RoutedEventArgs e)
        {
            _allRecordsLoaded = true;
            LoadRecords(limit: -1);
            LoadAllButton.IsEnabled = false;
        }

        private void LoadRecords(int limit)
        {
            ErrorBorder.Visibility = Visibility.Collapsed;

            // Check if plugin and repository are available
            if (_plugin?.Repository == null)
            {
                ErrorBorder.Visibility = Visibility.Visible;
                ErrorText.Text = "Plugin not initialized. Please restart SimHub.";
                return;
            }

            try
            {
                var records = limit > 0
                    ? _plugin.Repository.GetRecentRecords(limit)
                    : _plugin.Repository.GetAllRecords();

                RecordsGrid.ItemsSource = records;
                _recordsLoaded = true;

                var countText = limit > 0 && records.Count >= limit
                    ? $"Showing {records.Count} most recent records"
                    : $"{records.Count} records";
                RecordCountText.Text = countText;
            }
            catch (Exception ex)
            {
                ErrorBorder.Visibility = Visibility.Visible;
                ErrorText.Text = $"Unable to load records: {ex.Message}";
                SimHub.Logging.Current.Error($"Failed to load records in settings UI: {ex}");
            }
        }
    }
}
