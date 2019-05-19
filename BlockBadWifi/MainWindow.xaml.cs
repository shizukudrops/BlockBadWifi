using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BlockBadWifi
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Netsh netsh = new Netsh();
        private NativeWifiManager manager = new NativeWifiManager();
        private ObservableCollection<NetworkViewModel> networks;
        private ObservableCollection<NetworkViewModel> userBlockNetworks;

        public MainWindow()
        {
            InitializeComponent();

#if (DEBUG)
            menuitem_debug.Visibility = Visibility.Visible;
#endif

            networks = new ObservableCollection<NetworkViewModel>(manager.NetworkModels.Select(n => new NetworkViewModel(n)));
            networkList.ItemsSource = networks;
            userBlockNetworks = new ObservableCollection<NetworkViewModel>(netsh.UserBlockNetworks.Select(n => new NetworkViewModel(n)));
            userBlockList.ItemsSource = userBlockNetworks;

            Loaded += async (s, e) => {
                await ScanAndRefreshNetworkList();
                RefreshFilterList();
            };
        }

        #region MyMethods

        /// <summary>
        /// モデルのブロックされているネットワークのリストをビューに反映させる
        /// </summary>
        public void CopyFilterList()
        {
            userBlockNetworks.Replace(netsh.UserBlockNetworks.Select(n => new NetworkViewModel(n)));
        }

        private void RefreshFilterList()
        {
            netsh.FetchFilters();
            CopyFilterList();
        }

        public void RefreshNetworkList()
        {
            manager.RefreshNetworkInfomations();
            networks.Replace(manager.NetworkModels.Select(n => new NetworkViewModel(n)));
        }

        private async Task ScanAndRefreshNetworkList()
        {
            await manager.ScanNetworks();
            RefreshNetworkList();
        }

        public void BlockOrUnblockNetwork(NetworkModel network, bool block)
        {
            switch (netsh.BlockOrUnblockNetworks(network, block))
            {
                case NetshellErrors.SuccessOrUndefinedError:
                    CopyFilterList();
                    RefreshNetworkList();
                    break;

                case NetshellErrors.ParametersIncorrectOrMissing:
                    MessageBox.Show(Properties.Resources.Error_InvalidSsidOrNetworkType, Properties.Resources.Error);
                    break;
            }
        }

        #endregion

        private void Button_Block_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = networkList.SelectedItem as NetworkViewModel;

            if (selectedItem == null)
            {
                MessageBox.Show(Properties.Resources.Error_ChooseBlockNetwork, Properties.Resources.Error);
                return;
            }
            else if (string.IsNullOrWhiteSpace(selectedItem.Ssid))
            {
                MessageBox.Show(Properties.Resources.Error_InvalidSsid, Properties.Resources.Error);
                return;
            }
            else if (Regex.IsMatch(selectedItem.Ssid, "\0+"))
            {
                MessageBox.Show(Properties.Resources.Error_InvalidSsid, Properties.Resources.Error);
                return;
            }

            BlockOrUnblockNetwork(selectedItem, true);
        }

        private void Button_Unblock_Click(object sender, RoutedEventArgs e)
        {
            if(userBlockList.SelectedItem == null)
            {
                MessageBox.Show(Properties.Resources.Error_ChosseUnblockNetwork, Properties.Resources.Error);
                return;
            }

            BlockOrUnblockNetwork((NetworkViewModel)userBlockList.SelectedItem, false);
        }

        private async void MenuItem_Refresh_Click(object sender, RoutedEventArgs e)
        {
            await ScanAndRefreshNetworkList();
            RefreshFilterList();
        }

        private void MenuItem_ManageFilter_Click(object sender, RoutedEventArgs e)
        {
            var window = new ManualFilterWindow
            {
                MainWindow = this
            };
            window.Show();
        }

        private void MenuItem_Property_Click(object sender, RoutedEventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var title = assembly.GetName();
            var ver = title.Version;
            var copyright = (AssemblyCopyrightAttribute)assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));

            MessageBox.Show($@"{title.Name}
{ver}
{copyright.Copyright}
Released under MIT Lisence", Properties.Resources.Property);
        }

        private void MenuItem_Debug_Click(object sender, RoutedEventArgs e)
        {
            var window = new DebugWindow
            {
                Netsh = netsh
            };
            window.Show();
        }
    }
}
