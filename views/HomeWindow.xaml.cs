
using RPA_Window.model;
using RPA_Window.utils;
using System;
using System.Windows;
using System.Windows.Controls;
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
        SqliteHelper sqlite=new SqliteHelper();
        public HomeWindow()
        {
            InitializeComponent();
            folderList.ItemsSource = app.Folder;
            removeProjectList.ItemsSource = app.RemoveProjectList;
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
             //Notification.Show(new AppNotification(), ShowAnimation.Fade, true)
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
                    if(!app.Folder.Contains(selectedFolder))
                    {
                        string sql = $"insert into folders(folder_path) values('{SimpleEncryption.Encrypt(selectedFolder,"lyrrpa")}')";
                        Console.WriteLine(sql);
                        if (sqlite.InsertData(sql)>0)
                        {
                            app.Folder.Add(selectedFolder);
                            app.RefreshList();
                        }
                       
                    }
                    
                }
            }
        }

        private void folderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Console.WriteLine(sender)
            if(folderList.SelectedItem != null){ 
                folderText.Text=folderList.SelectedItem.ToString();
            }
        }

        private void Click_RemoveFolder(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            String folder= btn.DataContext.ToString();
            string sql = $"delete from folders where folder_path='{SimpleEncryption.Encrypt(folder,"lyrrpa")}'";
            if (sqlite.DeleteData(sql) > 0)
            {
                app.Folder.Remove(folder);
                app.RefreshList();
            }
        }
        private void Click_RemoveFolderName(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            FolderAttribute folder= btn.DataContext as FolderAttribute;
            string sql = $"delete from remove_projects where folder_path='{folder.FolderPath}'";
            if (sqlite.DeleteData(sql) > 0)
            {
                app.RemoveProjectList.Remove(folder);
                app.RefreshList();
            }
        }
    }
}
