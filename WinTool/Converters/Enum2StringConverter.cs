using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WinTool.ViewModels;

namespace WinTool.Converters
{
    internal class EnumRenameState2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RenameState rs)
            {
                if (rs == RenameState.NotStart)
                {
                    return "未开始";
                }
                else if (rs == RenameState.importFiles)
                {
                    return "已导入文件";
                }
                else if (rs == RenameState.ApplyTemplate)
                {
                    return "应用模板";
                }
                else if (rs == RenameState.Working)
                {
                    return "处理中";
                }
                else if (rs == RenameState.Finished)
                {
                    return "已完成";
                }
                else if (rs == RenameState.UndoTemplate)
                {
                    return "撤销模板";
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return RenameState.NotStart;
        }
    }
}
