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
        private List<NetworkModel> userBlockNetworks = new List<NetworkModel>();
        private readonly StringBuilder outputAndErrorLog = new StringBuilder();

        public IEnumerable<NetworkModel> UserBlockNetworks => userBlockNetworks;

        public string OutputAndErrorLog => outputAndErrorLog.ToString();

        public Netsh() { }

        /// <summary>
        /// コマンドを発行しWindowsが保有するフィルタのリストを取得する
        /// </summary>
        public void FetchFilters()
        {
            var result = Execute($@"/c chcp {codePage} & netsh wlan show filters");

            if (result.success)
            {
                try
                {
                    userBlockNetworks = StdOutputParser.UserBlockList(result.output.ToString()).ToList();
                }
                catch (Exception)
                {
                    outputAndErrorLog.AppendLine("[Exception]フィルタの標準出力のパースに失敗しました");
                }
            }
        }

        public NetshellErrors BlockOrUnblockNetworks(NetworkModel network, bool block)
        {
            string addOrDelete;

            if (block) addOrDelete = "add";
            else addOrDelete = "delete";

            var result = Execute($@"/c chcp {codePage} & netsh wlan {addOrDelete} filter permission=block ssid={network.Ssid} networktype={network.NetworkType}", true);

            if (result.success)
            {
                switch (ValidateOutput(result.output))
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

        private NetshellErrors ValidateOutput(string output)
        {
            if(output.Contains("One or more parameters for the command are not correct or missing"))
            {
                return NetshellErrors.ParametersIncorrectOrMissing;
            }
            else
            {
                return NetshellErrors.SuccessOrUndefinedError;
            }
        }

        private (string output, string error, bool success) Execute(string args, bool runas = false)
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

                    return (output.ToString(), error.ToString(), true);
                }
            }
            catch (Exception)
            {
                outputAndErrorLog.AppendLine("[Exception]NetShellコマンドの実行で例外が発生しました");
                return (output.ToString(), error.ToString(), false);
            }   
        }
    }
}
