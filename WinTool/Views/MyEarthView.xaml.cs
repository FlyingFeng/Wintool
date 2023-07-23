using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinTool.Views
{
    /// <summary>
    /// MyEarthView.xaml 的交互逻辑
    /// </summary>
    public partial class MyEarthView : UserControl
    {
        public MyEarthView()
        {
            InitializeComponent();


        }
        //private bool _isLoaded = false;
        //private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (!_isLoaded)
        //    {
        //        await webView2.EnsureCoreWebView2Async();
             
        //        if (webView2 != null && webView2.CoreWebView2 != null)
        //        {
        //            webView2.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
        //            var source = System.IO.Path.Combine(AppContext.BaseDirectory, "RawHtml", "index.html");
        //            webView2.CoreWebView2.Navigate($"file:///{source}");
        //            _isLoaded = true;
        //        }
        //    }
        //}

        //private void CoreWebView2_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        //{
        //    if (e.Request != null)
        //    {
        //        //e.Request.Headers.SetHeader("host", "t0.tianditu.com");
        //        //e.Request.Headers.SetHeader("referer", "http://localhost");
        //        //e.Request.Headers.SetHeader("origin", "http://localhost");
        //    }
        //}

    }
}
