using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
using THBWiki轮播工具.library;

namespace THBWiki轮播工具
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Form = null;
        public static bool _isClosing = false;

        private const int MaxInfoLine = 80;

        private const string BotServer = "ws://botdatabase.ritsukage.com:20020";
        private static WebSocketClient _botServer;

        private static bool BrowserOpen = false;
        private static bool SystemWorking = false;

        private static bool THBPlaying = false;
        private static bool Playing = false;

        public MainWindow()
        {
            InitializeComponent();
            _botServer = new WebSocketClient();
            _botServer.Opened += BotServerConnected;
            _botServer.MessageReceived += BotServerMessageReceived;
            _botServer.Error += BotServerError;
            _botServer.Closed += BotServerDisonnected;
            Form = this;
            AddInfo("初始化成功");
            AddInfo(InfoType.WARN, "左侧的信息只是闲的蛋疼做的，实际上大概没啥用，谁知道呢");
            AddInfo("启用方法：");
            AddInfo("1.激活浏览器");
            AddInfo("2.等到页面加载完毕，OBS这些配置也准备好了，激活轮播系统");
        }

        public void AddInfo(string msg)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                InfoList.Items.Add(new InfoLine(msg));
            });
        }

        public void AddInfo(InfoType type, string msg)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                InfoList.Items.Add(new InfoLine(type, msg));
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OBSWebSocket.Open();
            _botServer.Start(BotServer);
        }

        private void InfoLoad(object sender, DataGridRowEventArgs e)
        {
            switch ((e.Row.Item as InfoLine).Type)
            {
                case InfoType.WARN:
                    e.Row.Foreground = new SolidColorBrush(Colors.Orange);
                    break;
                case InfoType.ERROR:
                    e.Row.Foreground = new SolidColorBrush(Colors.Red);
                    break;
                default:
                    e.Row.Foreground = new SolidColorBrush(Colors.Black);
                    break;
            }
            if (InfoList.Items.Count > MaxInfoLine)
            {
                InfoList.Items.RemoveAt(0);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _isClosing = true;
            try
            {
                _botServer?.Dispose();
            }
            catch
            {
            }
            try
            {
                OBSWebSocket.Close();
            }
            catch
            {
            }
            if (BrowserOpen)
            {
                try
                {
                    BilibiliWebControl.Shutdown();
                }
                catch
                {
                }
            }
        }

        private static void BotServerConnected()
        {
            Form?.AddInfo("已连接至Bot Server");
        }

        private static void BotServerMessageReceived(string msg)
        {
            if (msg == "start")
            {
                Form?.AddInfo("接收到开始轮播请求");
                if (SystemWorking && !THBPlaying)
                {
                    Playing = true;
                    OBSWebSocket.StartStream();
                    BilibiliWebControl.ResumePlayer();
                }
                THBPlaying = true;
            }
            else if (msg == "stop")
            {
                Form?.AddInfo("接收到停止轮播请求");
                if (SystemWorking && THBPlaying)
                {
                    Playing = false;
                    BilibiliWebControl.PausePlayer();
                    OBSWebSocket.StopStream();
                }
                THBPlaying = false;
            }
        }

        private static void BotServerError(Exception ex)
        {
            Form?.AddInfo(InfoType.ERROR, "Bot服务器发生错误: " + ex.Message);
        }

        private static void BotServerDisonnected()
        {
            Form?.AddInfo("Bot Server已断开");
        }

        private void OpenBrowser(object sender, RoutedEventArgs e)
        {
            if (BrowserOpen) return;

            AddInfo("开启浏览器");
            BrowserOpen = true;
            BilibiliWebControl.Launch();
        }

        private void CloseBrowser(object sender, RoutedEventArgs e)
        {
            if (!BrowserOpen) return;

            if (SystemWorking)
            {
                AddInfo(InfoType.WARN, "轮播系统还未停止，请先结束轮播系统");
                return;
            }

            AddInfo("关闭浏览器");
            BrowserOpen = false;
            BilibiliWebControl.Shutdown();
        }

        private void StartSystem(object sender, RoutedEventArgs e)
        {
            if (SystemWorking) return;

            if (!BrowserOpen)
            {
                AddInfo(InfoType.WARN, "浏览器还未载入，不允许激活轮播系统");
                return;
            }

            AddInfo("激活轮播系统");

            if (THBPlaying)
            {
                Playing = true;
                OBSWebSocket.StartStream();
                BilibiliWebControl.ResumePlayer();
            }
            SystemWorking = true;
        }

        private void StopSystem(object sender, RoutedEventArgs e)
        {
            if (!SystemWorking) return;

            AddInfo("轮播系统停止");

            if (Playing)
            {
                Playing = false;
                BilibiliWebControl.PausePlayer();
                OBSWebSocket.StopStream();
            }
            SystemWorking = false;
        }
    }

    public class InfoLine
    {
        public string Time { get; private set; }

        public InfoType Type { get; private set; }

        public string Message { get; private set; }

        public InfoLine(string message)
        {
            Time = DateTime.Now.ToString();
            Type = InfoType.INFO;
            Message = message;
        }

        public InfoLine(InfoType type, string message)
        {
            Time = DateTime.Now.ToString();
            Type = type;
            Message = message;
        }
    }

    public enum InfoType
    {
        INFO,
        WARN,
        ERROR,
    }
}
