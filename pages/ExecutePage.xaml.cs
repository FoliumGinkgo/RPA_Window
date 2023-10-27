
using RPA_Window.model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace RPA_Window.pages
{
    /// <summary>
    /// ExecutePage.xaml 的交互逻辑
    /// </summary>
    public partial class ExecutePage : Page
    {
        App app = Application.Current as App;
        public ExecutePage()
        {
            InitializeComponent();
            executeDataGrid.ItemsSource=app.ExecuteLists;
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            FileAttribute data = btn.DataContext as FileAttribute;
            int index = app.ExecuteLists.IndexOf(data);
            
            if (app.ExecuteLists.Remove(data))
            {
                if (index <= App.index)
                {
                    App.index--;
                    if (index == App.index)
                    {
                        App.CancelFlag = true;
                    }
                }
                
            }
        }

        private void Click_Pinned(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            FileAttribute data = btn.DataContext as FileAttribute;
            app.ExecuteLists.Move(app.ExecuteLists.IndexOf(data),App.index+1);
        }
    }
}
