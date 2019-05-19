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
        public MainWindow MainWindow { get; set; }

        public ManualFilterWindow()
        {
            InitializeComponent();
        }

        #region MyMethods

        private void BlockOrUnblockNetwork(bool block)
        {
            if (string.IsNullOrWhiteSpace(ssidTextBox.Text) || networktypeComboBox.Text == "")
            {
                MessageBox.Show(Properties.Resources.Error_FillAll, Properties.Resources.Error);
                return;
            }

            var networkType = (NetworkType)networktypeComboBox.SelectedIndex + 1;

            MainWindow.BlockOrUnblockNetwork(new NetworkModel { Ssid = ssidTextBox.Text, NetworkType = networkType }, block);
        }

        #endregion

        private void Button_Block_Click(object sender, RoutedEventArgs e)
        {
            BlockOrUnblockNetwork(true);

            Close();
        }

        private void Button_Unblock_Click(object sender, RoutedEventArgs e)
        {
            BlockOrUnblockNetwork(false);

            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
