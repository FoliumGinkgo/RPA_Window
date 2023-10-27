using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RPA_Window.views
{
    /// <summary>
    /// HomeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HomeWindow : Window
    {
        App app = Application.Current as App;
        public HomeWindow()
        {
            InitializeComponent();
        }

        private void Click_Min(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
            // 终止整个应用程序
            Application.Current.Shutdown();
        }
        private void Click_Setting(object sender, RoutedEventArgs e)
        {
            
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Click_Message(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
