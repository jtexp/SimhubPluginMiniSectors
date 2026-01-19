using SimHub.Plugins.Styles;
using System.Windows.Controls;

namespace User.PluginMiniSectors
{
    /// <summary>
    /// Logique d'interaction pour SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public DataPluginMiniSectors Plugin { get; }

        public SettingsControl()
        {
            InitializeComponent();
        }

        public SettingsControl(DataPluginMiniSectors plugin) : this()
        {
            this.Plugin = plugin;
        }

        private async void StyledMessageBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var res = await SHMessageBox.Show("Message box", "Hello", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Question);

            await SHMessageBox.Show(res.ToString());
        }

        private void MiniSectorsWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var window = new MiniSectorsWindow();

            window.Show();
        }

        private async void MiniSectorsDialogWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialogWindow = new MiniSectorsDialogWindow();

            var res = await dialogWindow.ShowDialogWindowAsync(this);

            await SHMessageBox.Show(res.ToString());
        }
    }
}