using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTool.Models.MyPassword
{
    internal class MyPasswordModel : NotifyBaseObject
    {

        public int Id { get; set; }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisedPropertyChanged();
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                RaisedPropertyChanged();
            }
        }

        private byte[]? _password;
        public byte[]? Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisedPropertyChanged();
            }
        }

        private string _maskPassword = "****************";
        public string MaskPassword
        {
            get => _maskPassword;
            set
            {
                _maskPassword = value;
                RaisedPropertyChanged();
            }
        }

        private DateTime _createdTime;
        public DateTime CreatedTime
        {
            get => _createdTime;
            set
            {
                _createdTime = value;
                RaisedPropertyChanged();
            }
        }

    }
}
