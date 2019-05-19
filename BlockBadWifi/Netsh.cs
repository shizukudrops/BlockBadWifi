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

        public NetshellErrors BlockOrUnblockNetworks(NetworkModel network, bool block)
        {
            var addOrDelete = "";

            if (block) addOrDelete = "add";
            else addOrDelete = "delete";

            if (Execute($@"/c chcp {codePage} & netsh wlan {addOrDelete} filter permission=block ssid={network.Ssid} networktype={network.NetworkType}", true))
            {
                switch (ValidateOutput())
                {
                    case NetshellErrors.SuccessOrUndefinedError:
                        FetchFilters();
                        return NetshellErrors.SuccessOrUndefinedError;

                    case NetshellErrors.ParametersIncorrectOrMissing:
                        return NetshellErrors.ParametersIncorrectOrMissing;
                }
            }

            return NetshellErrors.Undefined;
        }

        private NetshellErrors ValidateOutput()
        {
            var target = output.ToString();

            if(target.Contains("One or more parameters for the command are not correct or missing"))
            {
                return NetshellErrors.ParametersIncorrectOrMissing;
            }
            else
            {
                return NetshellErrors.SuccessOrUndefinedError;
            }
        }

        private bool Execute(string args, bool runas = false)
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

                    return true;
                }
            }
            catch (Exception)
            {
                outputAndErrorLog.AppendLine("[Exception]NetShellコマンドの実行で例外が発生しました");
                return false;
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
