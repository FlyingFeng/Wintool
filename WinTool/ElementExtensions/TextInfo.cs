using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WinTool.ElementExtensions
{
    public class TextInfo
    {

        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.RegisterAttached("PlaceHolder", typeof(string), typeof(TextInfo), new PropertyMetadata
            {
                DefaultValue = string.Empty
            });


        public static void SetPlaceHolder(DependencyObject element, string val)
        {
            element.SetValue(PlaceHolderProperty, val);
        }

        public static string GetPlaceHolder(DependencyObject element)
        {
            return (string)element.GetValue(PlaceHolderProperty);
        }

    }
}
