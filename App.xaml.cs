using HandyControl.Controls;
using RPA_Window.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RPA_Window
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private string uiRobotPath = "C:\\Program Files (x86)\\UiPath\\Studio\\UiRobot.exe";
        public int index = 0;
        public ObservableCollection<FileAttribute> FileAttributes { get; set; } = new ObservableCollection<FileAttribute>();
        public ObservableCollection<FileAttribute> ExecuteLists { get; set; } = new ObservableCollection<FileAttribute>();
        public App()
        {
            //Thread thread = new Thread(ExecuteThread);
            //thread.Start();
        }

        private async void  ExecuteTask()
        {
            await Task.Run(() =>
            {
                for (index=index; index < ExecuteLists.Count; index++)
                {
                    FileAttribute attribute = ExecuteLists[index];
                    attribute.Status = "正在执行中...";
                }
            });
            
        }

        private void ExecuteThread()
        {
            while (true)
            {
                
                int count = ExecuteLists.Count;
                for (int i=0;i< ExecuteLists.Count && !ExecuteLists[i].IsExecute; i++)
                {
                    if (count>ExecuteLists.Count)
                    {
                        i = i - (count - ExecuteLists.Count);
                    }
                    FileAttribute attribute = ExecuteLists[i];
                    attribute.Status = "正在执行 . . .";
                    attribute.IsCurrent = true;
                    attribute.IsExecute = true;
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
                        Console.WriteLine("执行异常[未找执行入口]");
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
                    Process process = new Process();
                    process.StartInfo = processStartInfo;

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!String.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine(e.Data);
                            Dialog.Show(e.Data);
                        }
                        
                    };
                    
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!String.IsNullOrEmpty(e.Data))
                        {
                            attribute.Status = "执行异常";
                            Console.WriteLine(e.Data);
                        }                   
                    };

                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();

                    process.WaitForExit();

                    int code = process.ExitCode;
                    
                    if (code == 0)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            i=ExecuteLists.Remove(attribute)?i-1:i;
                        });

                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            attribute.Status = "执行异常";
                            Dialog.Show("执行出现异常");
                        });
                    }
                    attribute.IsCurrent = false;
                    Thread.Sleep(1000);
                }
            }
        }

        public void RefreshList()
        {
            FileAttributes.Clear();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\UiPath";
            if (!Directory.Exists(path))
            {
                return;
            }
            string[] files = Directory.GetDirectories(path);
            foreach (string file in files)
            {
                DateTime dateTime = Directory.GetLastWriteTime(file);
                string date = dateTime.ToString("yyyy/MM/dd HH:mm:ss");
                date = dateTime.ToString("yyyy/MM").Equals(DateTime.Now.ToString("yyyy/MM"))
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
                Task.Delay(100);
            }
        }
    }
}
