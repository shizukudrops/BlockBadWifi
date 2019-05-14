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

        private void Button_Block_Click(object sender, RoutedEventArgs e)
        {
            if(networkList.SelectedItem == null)
            {
                MessageBox.Show(Properties.Resources.Error_ChooseBlockNetwork, Properties.Resources.Error);
                return;
            }
            netsh.BlockNetwork((NetworkViewModel)networkList.SelectedItem);
            CopyFilterList();
            RefreshNetworkList();
        }

        private void Button_Unblock_Click(object sender, RoutedEventArgs e)
        {
            if(userBlockList.SelectedItem == null)
            {
                MessageBox.Show(Properties.Resources.Error_ChosseUnblockNetwork, Properties.Resources.Error);
                return;
            }
            netsh.UnblockNetwork((NetworkViewModel)userBlockList.SelectedItem);
            CopyFilterList();
            RefreshNetworkList();
        }

        public void CopyFilterList()
        { 
            userBlockNetworks.Clear();
            foreach (var n in netsh.UserBlockNetworks) userBlockNetworks.Add(new NetworkViewModel(n));
        }

        private void RefreshFilterList()
        {
            netsh.RefreshFilters();
            CopyFilterList();
        }

        public void RefreshNetworkList()
        {
            networks.Clear();
            manager.RefreshNetworkInfomations();
            foreach (var n in manager.NetworkModels) networks.Add(new NetworkViewModel(n));
        }

        private async Task ScanAndRefreshNetworkList()
        {
            await manager.ScanNetworks();
            RefreshNetworkList();
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
                Netsh = netsh,
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
