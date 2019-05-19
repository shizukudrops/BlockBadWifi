using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BlockBadWifi
{
    public class Netsh
    {
        private readonly int codePage = 65001;
        private StringBuilder outputAndErrorLog = new StringBuilder();
        private StringBuilder output = new StringBuilder();
        private StringBuilder error = new StringBuilder();
        private List<NetworkModel> userBlockNetworks = new List<NetworkModel>();

        public IEnumerable<NetworkModel> UserBlockNetworks
        {
            get => userBlockNetworks;
        }

        public string OutputAndErrorLog
        {
            get => outputAndErrorLog.ToString();
        }

        public Netsh() { }

        /// <summary>
        /// コマンドを発行しWindowsが保有するフィルタのリストを取得する
        /// </summary>
        public void FetchFilters()
        {
            Execute($@"/c chcp {codePage} & netsh wlan show filters");
            try
            {
                userBlockNetworks = StdOutputParser.UserBlockList(output.ToString()).ToList();
            }
            catch (Exception)
            {
                outputAndErrorLog.AppendLine("[Exception]フィルタの標準出力のパースに失敗しました");
            }
        }

        public void BlockNetwork(NetworkModel network)
        {
            Execute($@"/c chcp {codePage} & netsh wlan add filter permission=block ssid={network.Ssid} networktype={network.NetworkType}", true);
            FetchFilters();
        }

        public void UnblockNetwork(NetworkModel network)
        {
            Execute($@"/c chcp {codePage} & netsh wlan delete filter permission=block ssid={network.Ssid} networktype={network.NetworkType}", true);
            FetchFilters();
        }

        void Execute(string args, bool runas = false)
        {
            output.Clear();
            error.Clear();

            try
            {
                using (var p = new Process())
                {
                    if (runas)
                    {
                        p.StartInfo.Verb = "RunAs";
                    }

                    //出力をストリームに書き込むようにする
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;

                    //これがないと日本語が文字化けする
                    p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                    p.StartInfo.StandardErrorEncoding = Encoding.UTF8;

                    p.StartInfo.FileName = Environment.GetEnvironmentVariable("ComSpec");
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.Arguments = args;
                    
                    //OutputDataReceivedイベントハンドラを追加
                    p.OutputDataReceived += OutputDataReceived;
                    p.ErrorDataReceived += ErrorDataReceived;

                    p.Start();

                    //非同期で出力の読み取りを開始
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();

                    p.WaitForExit();

                    outputAndErrorLog.Append(output);
                    outputAndErrorLog.Append(error);
                }
            }
            catch (Exception)
            {
                outputAndErrorLog.AppendLine("[Exception]NetShellコマンドの実行で例外が発生しました");
            }   
        }

        void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            output.AppendLine(e.Data);
        }

        void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            error.AppendLine(e.Data);
        }
    }
}
