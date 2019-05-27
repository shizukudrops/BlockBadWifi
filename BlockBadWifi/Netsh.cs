using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BlockBadWifi
{
    public class Netsh
    {
        private readonly int codePage = 65001;
        private List<NetworkModel> userBlockNetworks = new List<NetworkModel>();
        private readonly StringBuilder outputAndErrorLog = new StringBuilder();

        public IEnumerable<NetworkModel> UserBlockNetworks => userBlockNetworks;

        public string OutputAndErrorLog => outputAndErrorLog.ToString();

        public Netsh() { }

        /// <summary>
        /// コマンドを発行しWindowsが保有するフィルタのリストを取得する。
        /// </summary>
        /// <returns>成功ならture。</returns>
        public NetshellErrors FetchFilters()
        {
            var (output, error) = Execute($@"/c chcp {codePage} & netsh wlan show filters");

            try
            {
                userBlockNetworks = StdOutputParser.ParseUserBlockList(output.ToString()).ToList();
                return NetshellErrors.Success;
            }
            catch (Exception e)
            {
                outputAndErrorLog.AppendLine("[Exception]フィルタの標準出力のパースに失敗しました");
                outputAndErrorLog.AppendLine(e.ToString());
                return NetshellErrors.FailedToFetchFilters;
            }
        }

        public NetshellErrors BlockOrUnblockNetworks(NetworkModel network, bool block)
        {
            string addOrDelete;

            if (block) addOrDelete = "add";
            else addOrDelete = "delete";

            var (output, error) = Execute($@"/c chcp {codePage} & netsh wlan {addOrDelete} filter permission=block ssid={network.Ssid} networktype={network.NetworkType}", true);

            var result = ValidateOutputOfManagingFilters(output);

            switch (result)
            {
                case NetshellErrors.Success:
                    return FetchFilters();

                default:
                    return result;
            }
        }

        private NetshellErrors ValidateOutputOfManagingFilters(string output)
        {
            if(Regex.IsMatch(output, "The filter is (added on|removed from) the system successfully"))
            {
                return NetshellErrors.Success;
            }
            else if(output.Contains("One or more parameters for the command are not correct or missing"))
            {
                return NetshellErrors.ParametersIncorrectOrMissing;
            }
            else
            {
                return NetshellErrors.UndefinedErrorsOnManagingFilters;
            }
        }

        private (string output, string error) Execute(string args, bool runas = false)
        {
            var output = new StringBuilder();
            var error = new StringBuilder();

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
                    p.OutputDataReceived += (s, e) => output.AppendLine(e.Data);
                    p.ErrorDataReceived += (s, e) => error.AppendLine(e.Data);

                    p.Start();

                    //非同期で出力の読み取りを開始
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();

                    p.WaitForExit();

                    outputAndErrorLog.Append(output);
                    outputAndErrorLog.Append(error);

                    return (output.ToString(), error.ToString());
                }
            }
            catch (Exception e)
            {
                outputAndErrorLog.AppendLine("[Exception]NetShellコマンドの実行で例外が発生しました");
                outputAndErrorLog.AppendLine(e.ToString());
                return (output.ToString(), error.ToString());
            }
        }
    }
}
