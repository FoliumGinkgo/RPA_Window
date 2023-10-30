using RPA_Window.model;
using System;
using System.IO;
using System.Threading;
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
            FileAttribute attribute=new FileAttribute();
            attribute.Status = "正在等待执行";
            attribute.IsCurrent = data.IsCurrent;
            attribute.SerialNumber =data.SerialNumber;
            attribute.FileName = data.FileName;
            attribute.FilePath = data.FilePath;
            attribute.Flag = data.Flag;
            attribute.IsExecute = data.IsExecute;
            app.ExecuteLists.Add(attribute);
            if (!App.Flag)
            {
                Thread theead = new Thread(app.ExecuteTask);
                theead.Start();
            }
            
            
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
