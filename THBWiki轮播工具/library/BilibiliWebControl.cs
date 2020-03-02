using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace THBWiki轮播工具.library
{
    class BilibiliWebControl
    {
        private static ChromeDriver driver;
        private static bool _isRunning = false;

        public BilibiliWebControl()
        {
        }

        public static void Launch()
        {
            _isRunning = true;
            
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;

            try
            {
                driver = new ChromeDriver(driverService);
            }
            catch
            {
                MessageBox.Show("Chrome启动失败，请确认安装了Chrome浏览器");
                _isRunning = false;
                return;
            }
            Goto("https://www.bilibili.com/medialist/play/ml853928275");
        }

        public static void Goto(string url)
        {
            if (!_isRunning) return;

            driver.Navigate().GoToUrl(url);
        }

        public static void Shutdown()
        {
            if (!_isRunning) return;

            try
            {
                driver?.Quit();
            }
            catch
            {
            }
        }

        public static void PausePlayer()
        {
            SendJS("player.pause()");
        }

        public static void ResumePlayer()
        {
            SendJS("player.play()");
        }

        public static void SendJS(string js)
        {
            if (!_isRunning) return;

            ((IJavaScriptExecutor)driver).ExecuteScript(js);
        }
    }
}
