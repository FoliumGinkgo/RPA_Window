using System;
using System.Windows;
using System.Windows.Input;
using 链友融RPA.pages;

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
            folderList.ItemsSource = app.Folder;
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
            //SettingWindow settingWindow = new SettingWindow();
            //settingWindow.ShowDialog();
            LeftDrawer.IsOpen = false;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Click_Message(object sender, RoutedEventArgs e)
        {
            
        }

        private void Folder_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                folderDialog.Description = "请选择一个文件夹";
                folderDialog.RootFolder = Environment.SpecialFolder.Desktop;

                // 显示对话框并获取用户的选择
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // 用户选择的文件夹路径
                    string selectedFolder = folderDialog.SelectedPath;
                    folderText.Text = selectedFolder;
                    app.Folder.Add(selectedFolder);
                }
            }
        }
    }
}
