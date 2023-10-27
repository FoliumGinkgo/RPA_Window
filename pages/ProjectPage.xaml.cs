using RPA_Window.model;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RPA_Window.pages
{
    /// <summary>
    /// ProjectPage.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectPage : Page
    {
        private App app =Application.Current as App;
        public ProjectPage()
        {
            InitializeComponent();
            dataGrid.ItemsSource = app.FileAttributes;
            
        }
        private void Click_Execute(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            FileAttribute data = btn.DataContext as FileAttribute;
            data.Status = "正在等待执行";
            app.ExecuteLists.Add(data);
            Console.WriteLine(data.ID);
            app.ExecuteTask();
        }

        private void Click_Refresh(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.IsEnabled = false;
            app.RefreshList();
            btn.IsEnabled = true;
        }
    }
}
