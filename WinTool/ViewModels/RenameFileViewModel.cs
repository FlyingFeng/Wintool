using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using form = System.Windows.Forms;
using System.Windows.Input;
using WinTool.Models.FileRename;
using Serilog;
using WinTool.Parsers;
using WinTool.Internal;
using System.Windows.Threading;
using Newtonsoft.Json;
using WinTool.Models.MyMap;

namespace WinTool.ViewModels
{
    internal enum RenameState
    {
        NotStart,
        importFiles,
        ApplyTemplate,
        Working,
        Finished,
        UndoTemplate
    }


    internal class RenameFileViewModel : NotifyBaseObject
    {
        public ObservableCollection<RenameFileModel> RenameFiles { get; set; } = new();
        private ICommand? _openFiles = null;
        private ICommand? _openFolder = null;
        private ICommand? _applyTemplate = null;
        private ICommand? _run = null;
        private ICommand? _undoTemplate = null;
        private bool _createNewFolder = false;
        private string _templateString = string.Empty;
        private StringTemplateParser? _parser = null;
        private string _currentFolder = string.Empty;
        private RenameState _renameState = RenameState.NotStart;
        private int _total;
        private int _currentCount;

        public RenameFileViewModel()
        {

            System.Diagnostics.Debug.WriteLine($"RenameFileViewModel构造");

        }


        public int CurrentCount
        {
            get => _currentCount;
            set
            {
                _currentCount = value;
                RaisedPropertyChanged();
            }
        }

        public int Total
        {
            get => _total;
            set
            {
                _total = value;
                RaisedPropertyChanged();
            }
        }

        public RenameState RenameState
        {
            get => _renameState;
            set
            {
                _renameState = value;
                RaisedPropertyChanged();
            }
        }

        public string TemplateString
        {
            get => _templateString;
            set
            {
                _templateString = value;
                RaisedPropertyChanged();
            }
        }


        public bool CreateNewFolder
        {
            get => _createNewFolder;
            set
            {
                _createNewFolder = value;
                RaisedPropertyChanged();
            }
        }

        public ICommand RunCommand
        {
            get => _run ??= new NormalCommand(async (s) =>
            {
                if (RenameState != RenameState.ApplyTemplate)
                {
                    MessageBox.Show("请先应用模板");
                    return;
                }

                try
                {
                    RenameState = RenameState.Working;
                    Caller.ProgressBar.SetTotal(Total);
                    Caller.ProgressBar.Show();
                    CurrentCount = 0;
                    await Task.Run(() =>
                    {
                        var destFolder = _currentFolder;
                        if (CreateNewFolder)
                        {
                            destFolder = Path.Combine(destFolder, "New");
                            if (!Directory.Exists(destFolder))
                            {
                                Directory.CreateDirectory(destFolder);
                            }
                        }
                        foreach (var item in RenameFiles)
                        {
                            //await Task.Delay(1);
                            try
                            {
                                var dest = Path.Combine(destFolder, item.NewFileName);
                                var source = Path.Combine(_currentFolder, item.OriginFileName);
                                if (CreateNewFolder)
                                {
                                    File.Copy(source, dest, true);
                                }
                                else
                                {
                                    File.Move(source, dest, true);
                                }

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    CurrentCount++;
                                    Caller.ProgressBar.SetCurrent(CurrentCount);
                                }, DispatcherPriority.Normal);

                            }
                            catch (Exception ex)
                            {
                                Log.Error($"RunCommand error, {ex.Message}\n{ex.StackTrace}");
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Log.Error($"RunCommand error, {ex.Message}\n{ex.StackTrace}");
                }
                finally
                {
                    RenameState = RenameState.Finished;
                    Caller.ProgressBar.Hide();
                }
            });
        }

        public ICommand UndoTemplateCommand
        {
            get => _undoTemplate ??= new NormalCommand(async (s) =>
            {
                if (RenameState == RenameState.Finished)
                {
                    MessageBox.Show("已经处理完成，请导入新文件");
                    return;
                }
                if (RenameState != RenameState.ApplyTemplate)
                {
                    MessageBox.Show("还没有应用模板");
                    return;
                }

                try
                {
                    Caller.WaitingMask.Show();
                    CurrentCount = 0;
                    RenameState = RenameState.UndoTemplate;
                    await Task.Run(() =>
                    {
                        foreach (var item in RenameFiles)
                        {
                            //await Task.Delay(1);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                CurrentCount++;
                                item.NewFileName = item.OriginFileName;
                            }, DispatcherPriority.Normal);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Log.Error($"UndoTemplateCommand error, {ex.Message}\n{ex.StackTrace}");
                }
                finally
                {
                    Caller.WaitingMask.Hide();
                }
            });
        }

        public ICommand ApplyTemplateCommand
        {
            get => _applyTemplate ??= new NormalCommand(async (s) =>
            {
                if (RenameState == RenameState.Finished)
                {
                    MessageBox.Show("已经处理完成，请导入新文件");
                    return;
                }
                if (string.IsNullOrEmpty(TemplateString))
                {
                    MessageBox.Show("请先输入模板");
                    return;
                }
                if (TemplateString.Length > 200)
                {
                    MessageBox.Show("输入模板过长，不能大于200个字符");
                    return;
                }
                if (RenameFiles.Count == 0)
                {
                    MessageBox.Show("请先导入文件或文件夹");
                    return;
                }

                string errMsg = string.Empty;
                try
                {
                    _parser = new(TemplateString);
                    _parser.Parse();

                    Caller.WaitingMask.Show();
                    RenameState = RenameState.ApplyTemplate;
                    CurrentCount = 0;
                    await Task.Run(() =>
                    {
                        var list = _parser.Generate(RenameFiles.Count);
                        for (int i = 0; i < list.Count; i++)
                        {
                            //await Task.Delay(1);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                CurrentCount++;
                                RenameFiles[i].NewFileName = $"{list[i]}{RenameFiles[i].ExtensionName}";
                            }, DispatcherPriority.Normal);
                        }
                    });
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                    Log.Error($"ApplyTemplateCommand error, {ex.Message}\n{ex.StackTrace}");
                }
                finally
                {
                    Caller.WaitingMask.Hide();
                }
                if (!string.IsNullOrEmpty(errMsg))
                {
                    MessageBox.Show(errMsg);
                }
            });
        }



        public ICommand OpenFilesCommand
        {
            get => _openFiles ??= new NormalCommand(async (s) =>
            {
                form.OpenFileDialog dialog = new form.OpenFileDialog();
                dialog.Multiselect = true;
                var result = dialog.ShowDialog();
                if (result == form.DialogResult.OK ||
                 result == form.DialogResult.Yes)
                {
                    _currentFolder = string.Empty;
                    RenameFiles.Clear();
                    await HandleOpenFilesAsync(dialog.FileNames);
                }

            });
        }



        public ICommand OpenFolderCommand
        {
            get => _openFolder ??= new NormalCommand(async (s) =>
            {
                form.FolderBrowserDialog dialog = new form.FolderBrowserDialog();
                var result = dialog.ShowDialog();
                if (result == form.DialogResult.OK ||
                 result == form.DialogResult.Yes)
                {
                    _currentFolder = string.Empty;
                    RenameFiles.Clear();
                    await HandleOpenFolderAsync(dialog.SelectedPath);
                }
            });
        }


        private async Task HandleOpenFolderAsync(string folderName)
        {

            try
            {
                var fileNames = Directory.GetFiles(folderName);
                await HandleOpenFilesAsync(fileNames);
            }
            catch (Exception ex)
            {
                Log.Error($"HandleOpenFolderAsync error, {ex.Message}\n{ex.StackTrace}");
            }

        }

        private async Task HandleOpenFilesAsync(IEnumerable<string> fileNames)
        {
            CurrentCount = 0;
            Total = fileNames.Count();
            RenameState = RenameState.importFiles;
            Caller.WaitingMask.Show();

            await Task.Run(() =>
            {
                try
                {
                    int index = 1;

                    foreach (var fileName in fileNames)
                    {
                        //await Task.Delay(1);
                        var fileInfo = new FileInfo(fileName);
                        if (string.IsNullOrEmpty(_currentFolder))
                        {
                            _currentFolder = fileInfo.DirectoryName ?? "";
                        }
                        var each = new RenameFileModel
                        {
                            Index = index,
                            OriginFileName = fileInfo.Name,
                            NewFileName = fileInfo.Name,
                            ExtensionName = fileInfo.Extension
                        };
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            CurrentCount++;
                            RenameFiles.Add(each);
                        }, DispatcherPriority.Normal);
                        index++;
                    }

                }
                catch (Exception ex)
                {
                    Log.Error($"HandleOpenFilesAsync error, {ex.Message}\n{ex.StackTrace}");
                }
                finally
                {
                    //System.Diagnostics.Debug.WriteLine("+++++++done++++");
                }
            });

            Caller.WaitingMask.Hide();
        }
    }
}
