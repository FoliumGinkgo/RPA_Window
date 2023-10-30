
using Microsoft.Win32;
using RPA_Window.model;
using RPA_Window.views;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace RPA_Window
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private int count = 0;
        private string text;
        private App app = Application.Current as App;
        private HomeWindow homeWindow = new HomeWindow();
        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(0.35);
            timer.Tick += Timer_task;
            timer.Start();
            text = "正在初始化数据 ";
            app.RefreshList();
            text = "正在检测安装环境 ";
            Detect_Install();
        }


        private void Timer_task(object sender, EventArgs e)
        {
            loadText.Text = text + new string('.',count);
            count = (count+1) % 5;
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Click_Close(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            this.Close();
            // 终止整个应用程序
            Application.Current.Shutdown();
        }


        private void Detect_Install()
        {
            string uipathRegistryKey = @"SOFTWARE\UiPath";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(uipathRegistryKey))
            {
                if (key != null)
                {
                    //Task.Delay(3000);
                    Close();
                    timer.Stop();
                    homeWindow.Show();
                    //MessageBox.Show("UiPath 已安装。");
                }
                else
                {
                    string uipathInstallDir = @"C:\Program Files (x86)\UiPath"; // 默认安装目录
                    if (Directory.Exists(uipathInstallDir))
                    {
                        this.Close();
                        timer.Stop();
                        homeWindow.Show();
                        //MessageBox.Show("UiPath 已安装。");
                    }
                    else
                    {
                        //MessageBox.Show("UiPath 未安装。");
                    }
                    
                }
            }
        }
    }
}
