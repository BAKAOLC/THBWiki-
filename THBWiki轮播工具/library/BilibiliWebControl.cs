using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace THBWiki轮播工具.library
{
    class BilibiliWebControl
    {
        private static ChromeDriver driver;
        private static bool _isRunning = false;

        public static bool Launched = false;

        public static BilibiliVideoInfo VideoInfo = new BilibiliVideoInfo();

        private static Thread _thread = null;

        private const string TargetPage = "https://www.bilibili.com/medialist/play/ml853928275";

        public BilibiliWebControl()
        {
        }

        public static async void Launch()
        {
            if (_isRunning) return;

            _isRunning = true;

            await Task.Run(() =>
            {
                MainWindow.Form.AddInfo(InfoType.INFO, "启动浏览器中……");
                ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;

                try
                {
                    driver = new ChromeDriver(driverService);
                    Goto(TargetPage);
                    Launched = true;
                    MainWindow.Form.AddInfo(InfoType.INFO, "浏览器启动完毕，现在开始可以激活轮播系统");
                    MainWindow.Form.AddInfo(InfoType.INFO, "如果视频自动播放，请手动暂停");
                    MainWindow.Form.AddInfo(InfoType.INFO, "如提示浏览器限制了自动播放，请手动播放一段后暂停");
                    _thread = new Thread(AutoUpdateVideoInfo);
                    _thread.Start();
                }
                catch
                {
                    Shutdown();
                    MainWindow.Form.AddInfo(InfoType.ERROR, "Chrome启动失败，请确认安装了Chrome浏览器");
                }
            });
        }

        public static void Goto(string url)
        {
            if (!_isRunning) return;
            driver.Navigate().GoToUrl(url);
        }

        public static async void Shutdown()
        {
            if (!_isRunning) return;

            try
            {
                _thread?.Abort();
            }
            catch
            { }
            _thread = null;

            MainWindow.Form.AddInfo(InfoType.INFO, "关闭浏览器中……");
            await Task.Run(() =>
            {
                try
                {
                    driver?.Quit();
                }
                catch
                {
                }
                finally
                {
                    Launched = false;
                    MainWindow.Form.AddInfo(InfoType.INFO, "已关闭浏览器");
                    _isRunning = false;
                }
            });
        }

        public static void PausePlayer()
        {
            if (!_isRunning) return;

            MainWindow.Form.AddInfo(InfoType.INFO, "向浏览器发送播放器暂停指令");
            SendJS("player.pause()");
        }

        public static void ResumePlayer()
        {
            if (!_isRunning) return;

            MainWindow.Form.AddInfo(InfoType.INFO, "向浏览器发送播放器播放指令");
            SendJS("player.play()");
        }

        public static void Refresh()
        {
            if (!_isRunning) return;

            try
            {
                MainWindow.Form.AddInfo(InfoType.INFO, "向浏览器发送页面刷新指令");
                driver.Navigate().Refresh();
                MainWindow.Form.AddInfo(InfoType.INFO, "刷新完毕");
            }
            catch (Exception ex)
            {
                MainWindow.Form.AddInfo(InfoType.ERROR, ex.Message);
            }
        }

        public static void NextVideo()
        {
            if (!_isRunning) return;

            MainWindow.Form.AddInfo(InfoType.INFO, "向浏览器发送切换下一视频请求");
            SendJS("player.next()");
        }

        public static void SetPart(int part)
        {
            if (!_isRunning) return;
            MainWindow.Form.AddInfo(InfoType.INFO, "向浏览器发送Part跳跃指令");
            Goto($"{TargetPage}/p{part}");
        }

        public static void SendJS(string js)
        {
            if (!_isRunning) return;

            try
            {
                ((IJavaScriptExecutor)driver).ExecuteScript(js);
            }
            catch (Exception ex)
            {
                MainWindow.Form.AddInfo(InfoType.ERROR, ex.Message);
            }
        }

        private static void AutoUpdateVideoInfo()
        {
            while (true)
            {
                if (_isRunning)
                    UpdateVideoInfo();
                Thread.Sleep(5000);
            }
        }

        private static readonly Regex BiliAVMatch = new Regex("(?<=av)[^/]+");
        public static void UpdateVideoInfo()
        {
            if (!_isRunning) return;

            try
            {
                string url = driver.FindElementByClassName("play-title-location").GetAttribute("href");
                string av = BiliAVMatch.Match(url).Value;
                VideoInfo.UpdateInfo(av);
            }
            catch (Exception ex)
            {
                MainWindow.Form.AddInfo(InfoType.ERROR, "更新视频信息时发生异常: " + ex.Message);
            }
        }
    }

    class BilibiliVideoInfo
    {
        public string AV { get; private set; }
        public string Name { get; private set; }
        public string Pic { get; private set; }
        public string Url { get; private set; }
        public string Uploader { get; private set; }
        public string UploaderId { get; private set; }
        public string UploaderUrl { get; private set; }
        public string Tname { get; private set; }
        public string View { get; private set; }
        public string Danmaku { get; private set; }
        public string Reply { get; private set; }
        public string Favorite { get; private set; }
        public string Coin { get; private set; }
        public string Share { get; private set; }
        public string Like { get; private set; }

        public BilibiliVideoInfo()
        {
            SetToDefault();
        }

        private void SetToDefault()
        {
            Pic = null;
            Name = "N/A";
            AV = "N/A";
            Url = "N/A";
            Uploader = "N/A";
            UploaderId = "N/A";
            UploaderUrl = "N/A";
            Tname = "N/A";
            View = "N/A";
            Danmaku = "N/A";
            Reply = "N/A";
            Favorite = "N/A";
            Coin = "N/A";
            Share = "N/A";
            Like = "N/A";
        }

        private const string BiliVideoInfoApi = "http://api.bilibili.com/x/web-interface/view?aid=";
        public async void UpdateInfo(string av)
        {
            await Task.Run(() =>
            {
                try
                {
                    string result = HttpGet(BiliVideoInfoApi + av);
                    if (result != "")
                    {
                        var info = JObject.Parse(result);
                        AV = av;
                        Name = (string)info["data"]["title"];
                        Pic = (string)info["data"]["pic"];
                        Url = $"https://www.bilibili.com/video/av{av}";
                        Uploader = (string)info["data"]["owner"]["name"];
                        UploaderId = (string)info["data"]["owner"]["mid"];
                        UploaderUrl = $"https://space.bilibili.com/{UploaderId}";
                        Tname = (string)info["data"]["tname"];
                        View = (string)info["data"]["stat"]["view"];
                        Danmaku = (string)info["data"]["stat"]["danmaku"];
                        Reply = (string)info["data"]["stat"]["reply"];
                        Favorite = (string)info["data"]["stat"]["favorite"];
                        Coin = (string)info["data"]["stat"]["coin"];
                        Share = (string)info["data"]["stat"]["share"];
                        Like = (string)info["data"]["stat"]["like"];
                    }
                }
                catch (Exception ex)
                {
                    SetToDefault();
                    MainWindow.Form.AddInfo(InfoType.ERROR, "更新视频信息时发生异常: " + ex.Message);
                }
            });
        }

        public void UpdateInfo(int av) => UpdateInfo(av.ToString());

        private static string HttpGet(string Url, string postDataStr = "", long timeout = 5000,
            string cookie = "")
        {
            HttpWebRequest request = null;
            try
            {
                if (Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) =>
                    {
                        return true;
                    });
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = (int)timeout;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";
                if (cookie != "")
                    request.Headers.Add("cookie", cookie);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8";
                }
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding));

                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myStreamReader.Dispose();
                myResponseStream.Close();
                myResponseStream.Dispose();
                request.Abort();
                return retString;
            }
            catch
            {
                request?.Abort();
            }
            return "";
        }

        public override string ToString()
        {
            return $@"{Name} av{AV}({Url})
分区: {Tname} UP: {Uploader}({UploaderUrl})
播放量: {View} 弹幕: {Danmaku} 评论：{Reply} 收藏: {Favorite} 投币: {Coin} 分享: {Share} 点赞: {Like}";
        }
    }
}
