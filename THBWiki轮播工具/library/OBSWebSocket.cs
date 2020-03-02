using OBS.WebSocket.NET;
using OBS.WebSocket.NET.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace THBWiki轮播工具.library
{
    class OBSWebSocket
    {
        private const string connectTo = "ws://localhost:4444/";
        private const string connectPassword = "";

        private static ObsWebSocket _obs;
        private static bool _running = false;
        private static bool _connecting = false;
        private static bool _ctip = false;
        private static bool _tip = false;
        public static bool Connected { get; private set; }

        private static Thread _thread = null;

        public static void Open()
        {
            if (_obs != null || _running) return;

            _running = true;

            _obs = new ObsWebSocket();

            _obs.StreamStatus += OnStreamData;

            try
            {
                _connecting = true;
                _tip = false;
                MainWindow.Form.AddInfo("尝试连接OBS中...");
                _ctip = true;
                _obs.Connect(connectTo, connectPassword);
                _thread = new Thread(CheckConnection);
                _thread.Start();
            }
            catch (AuthFailureException)
            {
                MainWindow.Form.AddInfo(InfoType.ERROR, "身份验证失败，请确认OBS Studio的WebSocket设置密码为空");
            }
            catch (ErrorResponseException ex)
            {
                MainWindow.Form.AddInfo(InfoType.ERROR, "尝试连接OBS时发生错误: " + ex.Message);
            }
            finally
            {
                _connecting = false;
            }
        }

        public static void Close()
        {
            if (_obs == null || !_running) return;

            try
            {
                _thread?.Abort();
            }
            catch
            { }
            _thread = null;
            try
            {
                _obs?.Disconnect();
            }
            catch
            { }
            _obs = null;
            _running = false;
        }

        private static void CheckConnection()
        {
            while (!MainWindow._isClosing)
            {
                Thread.Sleep(1000);
                if (MainWindow._isClosing)
                    break;
                if (_obs != null && _running)
                {
                    if (!_obs.IsConnected && !_connecting)
                    {
                        Connected = false;
                        _tip = false;
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            MainWindow.Form.ObsState_Connect.Content = "OBS未连接";
                        });
                        try
                        {
                            _connecting = true;
                            if (!_ctip)
                            {
                                MainWindow.Form.AddInfo("尝试连接OBS中...");
                                _ctip = true;
                            }
                            _obs.Connect(connectTo, connectPassword);
                        }
                        catch (AuthFailureException)
                        {
                            MainWindow.Form.AddInfo(InfoType.ERROR, "身份验证失败，请确认OBS Studio的WebSocket设置密码为空");
                        }
                        catch (ErrorResponseException ex)
                        {
                            MainWindow.Form.AddInfo(InfoType.ERROR, "尝试连接OBS时发生错误: " + ex.Message);
                        }
                        finally
                        {
                            _connecting = false;
                        }
                    }
                    else
                    {
                        Connected = true;
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            MainWindow.Form.ObsState_Connect.Content = "OBS已连接";
                        });
                        if (!_tip)
                            MainWindow.Form.AddInfo("已成功连接至OBS");
                        _tip = true;
                    }
                }
                else
                {
                    Connected = false;
                    _tip = false;
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        MainWindow.Form.ObsState_Connect.Content = "OBS未连接";
                    });
                }
            }
        }

        public static void StartStream()
        {
            if (!Connected) return;

            try
            {
                _obs.Api.StartStreaming();
            }
            catch (Exception ex)
            {
                MainWindow.Form.AddInfo(InfoType.ERROR, ex.Message);
            }
        }

        public static void StopStream()
        {
            if (!Connected) return;

            try
            {
                _obs.Api.StopStreaming();
            }
            catch (Exception ex)
            {
                MainWindow.Form.AddInfo(InfoType.ERROR, ex.Message);
            }
        }

        private static void OnStreamData(ObsWebSocket sender, StreamStatus data)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                MainWindow.Form.ObsState_TotalStream.Content = data.TotalStreamTime.ToString() + " sec";
                MainWindow.Form.ObsState_FPS.Content = data.FPS.ToString() + " FPS";
                MainWindow.Form.ObsState_Strain.Content = (data.Strain * 100).ToString() + " %";
                MainWindow.Form.ObsState_DropFrames.Content = data.DroppedFrames.ToString();
                MainWindow.Form.ObsState_TotalFrames.Content = data.TotalFrames.ToString();
            });
        }
    }
}
