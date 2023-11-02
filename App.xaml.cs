using HandyControl.Controls;
using RPA_Window.model;
using RPA_Window.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Shapes;
using 链友融RPA.dialog;
using Path = System.IO.Path;

namespace RPA_Window
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private string uiRobotPath = "C:\\Program Files (x86)\\UiPath\\Studio\\UiRobot.exe";
        public static bool Flag=false;
        public ObservableCollection<FileAttribute> FileAttributes { get; set; } = new ObservableCollection<FileAttribute>();
        public ObservableCollection<FileAttribute> ExecuteLists { get; set; } = new ObservableCollection<FileAttribute>();
        public ObservableCollection<String>  Folder{ get; set; } = new ObservableCollection<String>();
        public ObservableCollection<FolderAttribute> RemoveProjectList{ get; set; } = new ObservableCollection<FolderAttribute>();
        public static SqliteHelper sqlite=new SqliteHelper();
        public App()
        {
            ExecuteLists.CollectionChanged += CollectionChangedHandler;
            Thread thread = new Thread(ExecuteTask);
        }

        private void CollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                if (!Flag)
                {
                    //ExecuteTask();
                }
            }else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                //FileAttribute attribute = e.OldItems[0] as FileAttribute;
                /*if (attribute.Processing != null)
                {
                    attribute.Processing.Kill();
                }*/

            }
        }

        public void  ExecuteTask()
        {
            int count = ExecuteLists.Count;
            Flag = true;
            for (int index = 0; index < count; index++)
            {
                Console.WriteLine(index);
                if (ExecuteLists[index].IsExecute)
                {
                    continue;
                }
                FileAttribute attribute = ExecuteLists[index];
                attribute.Status = "正在执行...";
                attribute.Flag = false;
                attribute.IsCurrent = true;
                attribute.FontColor = "#0078d4";
                string path = "";
                string[] files = Directory.GetFiles(attribute.FilePath);
                foreach (string file in files)
                {
                    if (file.EndsWith("\\Main.xaml"))
                    {
                        path = file;
                    }
                }

                if (path.Equals(""))
                {
                    attribute.IsCurrent = false;
                    attribute.IsExecute = true;
                    attribute.FontColor = "red";
                    attribute.Status = "执行异常[未找执行入口]";
                    continue;
                }

                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = uiRobotPath,
                    Arguments = $"-file \"{path}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                attribute.Processing= new Process();

                attribute.Processing.StartInfo = processStartInfo;

                attribute.Processing.ErrorDataReceived += (sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        attribute.FontColor = "red";
                        attribute.Status = "执行异常";
                        attribute.IsExecute = true;
                    }

                };

                attribute.Processing.OutputDataReceived += (sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        //处理输出信息
                    }
                };

                attribute.Processing.Start();
                //attribute.Processing.BeginErrorReadLine();
                //attribute.Processing.BeginOutputReadLine();
                attribute.Processing.WaitForExit();
                string error = attribute.Processing.StandardError.ReadToEnd();
                Console.WriteLine(error);
                if (!error.Equals(""))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageDialog dialog = new MessageDialog();
                        dialog.title.Text = "异常提醒";
                        dialog.message.Text = error;
                        dialog.ok.Content = "确定";
                        dialog.ShowDialog();
                    });
                }
                
                int code = attribute.Processing.ExitCode;

                if (code == 0)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        attribute.Processing = null;
                        ExecuteLists.Remove(attribute);
                        index--;
                    });


                }
                else
                {
                    attribute.FontColor = "red";
                    attribute.Status = "执行异常";
                    attribute.IsExecute = true;
                    attribute.Processing = null;
                }
                attribute.IsCurrent = false;
                count = ExecuteLists.Count;
                Task.Delay(1000);
            }
            Flag = false;
        }
        public void RefreshList()
        {
            FileAttributes.Clear();
            Folder.Clear();
            string sql = "select * from folders";
            DataTable folderTable= sqlite.QueryData(sql);
            sql= "select * from remove_projects";
            DataTable folderRemoveTable = sqlite.QueryData(sql);
            if ( folderTable.Rows.Count>0 )
            {
                //获取移除项目的列表
                foreach (DataRow item in folderRemoveTable.Rows)
                {
                    
                    RemoveProjectList.Add(
                        new FolderAttribute
                        {
                            FolderPath = item["folder_path"].ToString(),
                            FolderName = Path.GetFileName(item["folder_path"].ToString())
                        }) ;
                    
                }
                //获取项目列表
                foreach (DataRow row in folderTable.Rows)
                {
                    
                    string path = SimpleEncryption.Decrypt(row["folder_path"].ToString(), "lyrrpa");
                    Folder.Add(path);
                    string[] files = Directory.GetDirectories(path);
                    foreach (string file in files)
                    {
                        bool exists = false;
                        foreach (FolderAttribute attribute in RemoveProjectList)
                        {
                            if (attribute.FolderPath.Equals(file))
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (exists)
                        {
                            continue;
                        }
                        DateTime dateTime = Directory.GetLastWriteTime(file);
                        string date = dateTime.ToString("yyyy/MM").Equals(DateTime.Now.ToString("yyyy/MM"))
                            ? dateTime.ToString("yyyy/MM/dd").Equals(DateTime.Now.ToString("yyyy/MM/dd"))
                            ? "今天" : (DateTime.Now - dateTime).Days.ToString() + " 天前" : dateTime.ToString("yyyy/MM/dd");
                        FileAttributes.Add(new FileAttribute()
                        {
                            ID = Guid.NewGuid().ToString(),
                            SerialNumber = FileAttributes.Count + 1,
                            IsCurrent = false,
                            Flag = true,
                            IsExecute = false,
                            FileName = Path.GetFileName(file),
                            UpdateTime = date,
                            FilePath = file,
                            FontColor = "#000000",
                            Status = "待添加执行"
                        }); ;
                        
                    }
                }
            }
            else
            {
                
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\UiPath";
                if (!Directory.Exists(path))
                {
                    return;
                }
                string[] files = Directory.GetDirectories(path);
                foreach (string file in files)
                {
                    DateTime dateTime = Directory.GetLastWriteTime(file);
                    string date = dateTime.ToString("yyyy/MM").Equals(DateTime.Now.ToString("yyyy/MM"))
                        ? dateTime.ToString("yyyy/MM/dd").Equals(DateTime.Now.ToString("yyyy/MM/dd"))
                        ? "今天" : (DateTime.Now - dateTime).Days.ToString() + " 天前" : dateTime.ToString("yyyy/MM/dd");
                    FileAttributes.Add(new FileAttribute()
                    {
                        ID = Guid.NewGuid().ToString(),
                        SerialNumber = FileAttributes.Count + 1,
                        IsCurrent = false,
                        Flag = true,
                        IsExecute = false,
                        FileName = Path.GetFileName(file),
                        UpdateTime = date,
                        FilePath = file,
                        FontColor = "#000000",
                        Status = "待添加执行"
                    }); ;
                    
                }
            }
            
        }
    }
}
