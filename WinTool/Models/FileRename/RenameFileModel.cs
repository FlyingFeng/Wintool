using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTool.Models.FileRename
{
    internal class RenameFileModel : NotifyBaseObject
    {

        private string _originFileName = string.Empty;
        private string _newFileName = string.Empty;

        public string ExtensionName { get; set; } = string.Empty;

        public int Index { get; set; }
        public string OriginFileName
        {
            get => _originFileName;
            set
            {
                _originFileName = value;
                RaisedPropertyChanged();
            }
        }


        public string NewFileName
        {
            get => _newFileName;
            set
            {
                _newFileName = value;
                RaisedPropertyChanged();
            }
        }



    }
}
