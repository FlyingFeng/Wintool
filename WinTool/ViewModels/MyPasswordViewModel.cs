using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WinTool.Models.MyPassword;

namespace WinTool.ViewModels
{
    internal class MyPasswordViewModel : NotifyBaseObject
    {
        public MyPasswordViewModel()
        {
            System.Diagnostics.Debug.WriteLine($"MyPasswordViewModel构造");
        }


        public ObservableCollection<MyPasswordModel> SourceData { get; set; } = new ObservableCollection<MyPasswordModel>();
        //private bool _dbExisted = true;
        //public bool DbExisted
        //{
        //    get => _dbExisted;
        //    set
        //    {
        //        _dbExisted = value;
        //        RaisedPropertyChanged();
        //    }
        //}


        private string _searchName = string.Empty;
        public string SearchName
        {
            get => _searchName;
            set
            {
                _searchName = value;
                RaisedPropertyChanged();
            }
        }


        private ICommand? _modifyCommand;
        public ICommand ModifyCommand
        {
            get => _modifyCommand ??= new NormalCommand(async(s) =>
            {
                if (s != null && int.TryParse(s.ToString(), out var id))
                {
                    ModifyPasswordWindow window = new ModifyPasswordWindow
                    {
                        OperateType = OperateEnum.Modify,
                        EntityId = id
                    };
                    window.Owner = Application.Current.MainWindow;
                    var flag = window.ShowDialog();
                    if (flag ?? false)
                    {
                        await LoadDataFromDbAsync();
                    }
                }
            });
        }

        private ICommand? _addCommand;
        public ICommand AddCommand
        {
            get => _addCommand ??= new NormalCommand(async (s) =>
            {
                ModifyPasswordWindow window = new ModifyPasswordWindow();
                window.Owner = Application.Current.MainWindow;
                var flag = window.ShowDialog();
                if (flag ?? false)
                {
                    await LoadDataFromDbAsync();
                }

            }, CanExecuted);
        }

        private ICommand? _removeCommand;
        public ICommand RemoveCommand
        {
            get => _removeCommand ??= new NormalCommand(async (s) =>
            {
                try
                {
                    var recordIds = SourceData.Where(s => s.IsChecked).Select(s => s.Id).ToList();
                    if (recordIds?.Count > 0)
                    {
                        using var dbContext = new WinDbContext();
                        await dbContext.MyPasswords.Where(s => recordIds.Contains(s.Id)).ExecuteDeleteAsync();
                        await dbContext.SaveChangesAsync();
                        await LoadDataFromDbAsync();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"RemoveCommand error, {ex.Message}\n{ex.StackTrace}");
                    MessageBox.Show("移除选项异常");
                }
            }, CanExecuted);
        }


        private ICommand? _searchCommand;
        public ICommand SearchCommand
        {
            get => _searchCommand ??= new NormalCommand(async (s) =>
            {
                try
                {
                    await LoadDataFromDbAsync(name: SearchName);
                }
                catch (Exception ex)
                {
                    Log.Error($"SearchCommand error, {ex.Message}\n{ex.StackTrace}");
                    MessageBox.Show("查找异常");
                }
            }, CanExecuted);
        }


        private ICommand? _loadedCommand;
        public ICommand LoadedCommand
        {
            get => _loadedCommand ??= new NormalCommand(async (s) =>
            {
                if (!CanExecuted())
                {
                    MessageBox.Show("数据库不存在!");
                    return;
                }
                try
                {
                    await LoadDataFromDbAsync();
                }
                catch (Exception ex)
                {
                    Log.Error($"LoadedCommand error, {ex.Message}\n{ex.StackTrace}");
                    MessageBox.Show("加载数据异常");
                }
            }, CanExecuted);
        }

        private bool CanExecuted()
        {
            if (!File.Exists(WinDbContext.DbPath))
            {
                return false;
            }
            return true;
        }

        private async Task LoadDataFromDbAsync(int pageIndex = 1, int pageCount = 50, string name = "")
        {
            SourceData.Clear();
            using var dbContext = new WinDbContext();
            var query = dbContext.MyPasswords.AsQueryable().AsNoTracking();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }
            if (pageIndex != 1 && pageCount != 50)
            {
                query = query.Skip((pageIndex - 1) * pageCount).Take(pageCount);
            }
            var records = await query.ToListAsync();

            var now = DateTime.Now;
            foreach (var item in records)
            {
                var model = new MyPasswordModel
                {
                    Id = item.Id,
                    CreatedTime = item.CreatedTime ?? now,
                    IsChecked = false,
                    Name = item.Name,
                    Password = item.Password
                };
                SourceData.Add(model);
            }

        }

    }
}
