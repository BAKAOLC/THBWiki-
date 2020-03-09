using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace THBWiki轮播工具.library
{
    class BilibiliWebControl
    {
        private static ChromeDriver driver;
        private static bool _isRunning = false;

        public static bool Launched = false;

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
                    Goto("https://www.bilibili.com/medialist/play/ml853928275");
                    Launched = true;
                    MainWindow.Form.AddInfo(InfoType.INFO, "浏览器启动完毕，现在开始可以激活轮播系统");
                    MainWindow.Form.AddInfo(InfoType.INFO, "如果视频自动播放，请手动暂停");
                    MainWindow.Form.AddInfo(InfoType.INFO, "如提示浏览器限制了自动播放，请手动播放一段后暂停");
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
            MainWindow.Form.AddInfo(InfoType.INFO, "向浏览器发送播放器暂停指令");
            SendJS("player.pause()");
        }

        public static void ResumePlayer()
        {
            MainWindow.Form.AddInfo(InfoType.INFO, "向浏览器发送播放器播放指令");
            SendJS("player.play()");
        }

        public static void SendJS(string js)
        {
            if (!_isRunning) return;

            ((IJavaScriptExecutor)driver).ExecuteScript(js);
        }
    }
}
