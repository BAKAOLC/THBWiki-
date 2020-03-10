using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Net;
using Image = System.Drawing.Image;
using System.IO;
using System.Drawing.Imaging;

namespace THBWiki轮播工具.library
{
    /// <summary>
    /// InfoViewer.xaml 的交互逻辑
    /// </summary>
    public partial class InfoViewer : Window
    {
        private Thread _thread = null;

        public InfoViewer()
        {
            InitializeComponent();
            _thread = new Thread(UpdateInfo);
            _thread.Start();
        }

        private void UpdateInfo()
        {
            string lastPicUrl = null;
            while (true)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        VideoName.Content = BilibiliWebControl.VideoInfo.Name;
                        VideoUrl.Content = BilibiliWebControl.VideoInfo.Url;
                        VideoTname.Content = BilibiliWebControl.VideoInfo.Tname;
                        VideoView.Content = BilibiliWebControl.VideoInfo.View;
                        VideoDanmaku.Content = BilibiliWebControl.VideoInfo.Danmaku;
                        VideoReply.Content = BilibiliWebControl.VideoInfo.Reply;
                        VideoLike.Content = BilibiliWebControl.VideoInfo.Like;
                        VideoCoin.Content = BilibiliWebControl.VideoInfo.Coin;
                        VideoFavorite.Content = BilibiliWebControl.VideoInfo.Favorite;
                        VideoShare.Content = BilibiliWebControl.VideoInfo.Share;
                        VideoUploader.Content = BilibiliWebControl.VideoInfo.Uploader;
                        VideoUploaderUrl.Content = BilibiliWebControl.VideoInfo.UploaderUrl;
                        if (BilibiliWebControl.VideoInfo.Pic != null && lastPicUrl != BilibiliWebControl.VideoInfo.Pic)
                        {
                            lastPicUrl = BilibiliWebControl.VideoInfo.Pic;
                            SetImage(lastPicUrl);
                        }
                    }
                    catch
                    {
                    }
                });
                Thread.Sleep(1000);
            }
        }

        private async void SetImage(string url)
        {
            Bitmap image = null;
            await Task.Run(() =>
            {
                image = (Bitmap)url.GetImageFromNet().Clone();
            });
            App.Current.Dispatcher.Invoke(() =>
            {
                VideoImage.Source = BitmapToBitmapImage(image);
            });
        }

        private static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                _thread?.Abort();
            }
            catch
            {
            }
            _thread = null;
        }
    }

    public static class ImageExtensions
    {
        public static Image GetImageFromNet(this string url, Action<WebRequest> requestAction = null, Func<WebResponse, Image> responseFunc = null)
        {
            return new Uri(url).GetImageFromNet(requestAction, responseFunc);
        }

        public static Image GetImageFromNet(this Uri url, Action<WebRequest> requestAction = null, Func<WebResponse, Image> responseFunc = null)
        {
            Image img;
            try
            {
                WebRequest request = WebRequest.Create(url);
                if (requestAction != null)
                {
                    requestAction(request);
                }
                using (WebResponse response = request.GetResponse())
                {
                    if (responseFunc != null)
                    {
                        img = responseFunc(response);
                    }
                    else
                    {
                        img = Image.FromStream(response.GetResponseStream());
                    }
                }
            }
            catch
            {
                img = null;
            }
            return img;
        }
    }
}
