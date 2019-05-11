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
using System.Windows.Shapes;

namespace BlockBadWifi
{
    /// <summary>
    /// ManualFilterWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ManualFilterWindow : Window
    {
        public Netsh Netsh { get; set; }
        public MainWindow MainWindow { get; set; }

        public ManualFilterWindow()
        {
            InitializeComponent();
        }

        private void Button_Block_Click(object sender, RoutedEventArgs e)
        {
            if(ssidTextBox.Text == "" || networktypeComboBox.Text == "")
            {
                MessageBox.Show("すべての項目を入力してください", "エラー");
                return;
            }
            var networkType = (NetworkType)networktypeComboBox.SelectedIndex;
            Netsh.BlockNetwork(new NetworkModel { Ssid = ssidTextBox.Text, NetworkType = networkType.ToString() });
            MainWindow.CopyFilterList();
            MainWindow.RefreshNetworkList();
            Close();
        }

        private void Button_Unblock_Click(object sender, RoutedEventArgs e)
        {
            if (ssidTextBox.Text == "" || networktypeComboBox.Text == "")
            {
                MessageBox.Show("すべての項目を入力してください", "エラー");
                return;
            }
            var networkType = (NetworkType)networktypeComboBox.SelectedIndex;
            Netsh.UnblockNetwork(new NetworkModel { Ssid = ssidTextBox.Text, NetworkType = networkType.ToString() });
            MainWindow.CopyFilterList();
            MainWindow.RefreshNetworkList();
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
