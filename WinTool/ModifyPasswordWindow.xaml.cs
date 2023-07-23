using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinTool.Converters;
using WinTool.Entities;

namespace WinTool
{


    /// <summary>
    /// ModifyPasswordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyPasswordWindow : Window
    {
        public ModifyPasswordWindow()
        {
            InitializeComponent();
        }

        internal OperateEnum OperateType { get; set; } = OperateEnum.Add;

        internal int EntityId { get; set; }


        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(pwdName.Text))
            {
                MessageBox.Show("请输入密码名称");
                return false;
            }
            if (string.IsNullOrEmpty(pwdOne.Password))
            {
                MessageBox.Show("请输入密码");
                return false;
            }
            if (string.IsNullOrEmpty(pwdTwo.Password))
            {
                MessageBox.Show("请再次输入密码");
                return false;
            }

            if (pwdOne.Password != pwdTwo.Password)
            {
                MessageBox.Show("两次输入的密码不一样");
                return false;
            }

            return true;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private async void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
            {
                return;
            }

            //var list = new List<byte>();
            //var pwdBytes = Encoding.UTF8.GetBytes(pwdOne.Password);
            //list.AddRange(BitConverter.GetBytes(pwdBytes.Length));

            //foreach (var item in pwdBytes)
            //{
            //    list.Add(item);
            //    list.Add((byte)Random.Shared.Next(255));
            //}
            //int need = 1024 - list.Count;
            //if (need > 0)
            //{
            //    for (int i = 0; i < need; i++)
            //    {
            //        list.Add((byte)Random.Shared.Next(255));
            //    }
            //}
            var list = Password.Encode(pwdOne.Password);

            using var dbContext = new WinDbContext();

            if (OperateType == OperateEnum.Add)
            {
                var entity = new MyPasswordEntity
                {
                    CreatedTime = DateTime.Now,
                    Name = pwdName.Text.Trim(),
                    Password = list.ToArray(),
                    LoginName = txtLoginName.Text.Trim(),
                    Remark = txtRemark.Text.Trim(),
                    Url = txtUrl.Text.Trim()
                };
                dbContext.MyPasswords.Add(entity);
            }
            else if (OperateType == OperateEnum.Modify)
            {
                var entity = dbContext.MyPasswords.Find(EntityId);
                if (entity != null)
                {
                    entity.Url = txtUrl.Text.Trim();
                    entity.Remark = txtRemark.Text.Trim();
                    entity.Name = pwdName.Text.Trim();
                    entity.LoginName = txtLoginName.Text.Trim();
                    entity.Password = list.ToArray();
                }
            }
            await dbContext.SaveChangesAsync();

            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (OperateType == OperateEnum.Modify)
            {
                using var dbContext = new WinDbContext();
                var entity = dbContext.MyPasswords.Find(EntityId);
                if (entity != null)
                {
                    pwdName.Text = entity.Name;
                    txtLoginName.Text = entity.LoginName;
                    pwdOne.Password = Password.Parse(entity.Password);
                    pwdTwo.Password = Password.Parse(entity.Password);
                    txtUrl.Text = entity.Url;
                    txtRemark.Text = entity.Remark;
                }

            }
        }
    }
}
