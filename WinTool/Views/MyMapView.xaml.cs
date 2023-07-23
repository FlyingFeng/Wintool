using GMap.NET;
using GMap.NET.WindowsPresentation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WinTool.Internal;
using WinTool.Models.MyMap;
using WinTool.MyMapLogic;

namespace WinTool.Views
{


    internal enum MapOperation
    {
        /// <summary>
        /// 可以拖拽地图
        /// </summary>
        Normal,

        /// <summary>
        /// 可以拖拽地图，在上面画线
        /// </summary>
        MeasureDistance,

        /// <summary>
        /// 可以拖拽地图，在上面画多边形
        /// </summary>
        MeasureAreaPolygon,

        /// <summary>
        /// 可以拖拽地图，在上面画圆形
        /// </summary>
        MeasureAreaCircle,

        /// <summary>
        /// 可以拖拽地图，在上面画矩形
        /// </summary>
        MeasureAreaRectangle
    }

    /// <summary>
    /// MyMapView.xaml 的交互逻辑
    /// </summary>
    public partial class MyMapView : UserControl
    {
        const string Region = "Region";
        const string Popup = "Popup";

        List<string> Excludes = new List<string> { Region, Popup };

        public MyMapView()
        {
            InitializeComponent();

            var path = System.IO.Path.Combine(AppContext.BaseDirectory, "Cache");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            myMap.CacheLocation = path;
            myMap.MapProvider = AMapHybridMapProvider.Instance;
            myMap.Position = new PointLatLng(22.54, 114.06);
            myMap.MinZoom = 3;
            myMap.MaxZoom = 20;
            myMap.Zoom = 6;
            myMap.ShowCenter = false;
            myMap.DragButton = MouseButton.Right;

            myMap.MouseMove += MyMap_MouseMove;
            myMap.OnMapZoomChanged += MyMap_OnMapZoomChanged;
            myMap.MouseDoubleClick += MyMap_MouseDoubleClick;
            myMap.MouseLeftButtonDown += MyMap_MouseLeftButtonDown;
            myMap.MouseEnter += MyMap_MouseEnter;

            if (Caller.ChinaRegion?.Province != null &&
                Caller.ChinaRegion.Province.Count > 0)
            {
                var firstProvinceData = Caller.ChinaRegion.Province[0].Points;
                if (firstProvinceData != null)
                {
                    _region = new MyMapPolygon(firstProvinceData);
                    _region.Tag = "Region";
                    myMap.Markers.Add(_region);
                    var area = China.CalculateArea(firstProvinceData.Select(x => x.Lng).ToList(),
                           firstProvinceData.Select(x => x.Lat).ToList());
                    tbArea.Text = $"{Math.Round(area / 1000000.0, 2)}";
                }
            }

            InitTip();

        }



        private MyMapPolygon? _region;
        private MapOperation _mapOperation = MapOperation.Normal;
        private MyMapMarker? _tipMarker;
        private TextBlock? _tipInfo;
        private PointLatLng SouthPole = new PointLatLng(-80, 48.5642);
        private MyMapPolygon? _currentPolygon;
        private Point _leftMouseDownPoint;
        private MyMapRoute? _currentRoute;

        private void InitTip()
        {
            var pos = SouthPole; //南极
            _tipMarker = new MyMapMarker(pos);
            _tipMarker.Tag = Popup;
            _tipInfo = new TextBlock();
            _tipInfo.Foreground = Brushes.Black;
            _tipInfo.FontSize = 14;
            _tipInfo.TextWrapping = TextWrapping.Wrap;
            _tipInfo.Width = 88;
            //_tipInfo.Text = "testing...";
            _tipInfo.Background = Brushes.Blue;
            _tipInfo.Visibility = Visibility.Collapsed;
            _tipMarker.ZIndex = 99999;
            _tipMarker.Shape = _tipInfo;
            _tipMarker.Offset = new Point(-33, 15);
            myMap.Markers.Add(_tipMarker);
        }

        private void HideTipMarker()
        {
            if (_tipMarker != null && _tipInfo != null)
            {
                _tipMarker.Position = SouthPole;
                _tipInfo.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowTipMarker(PointLatLng pos, string info = "")
        {
            if (_tipMarker != null && _tipInfo != null)
            {
                _tipMarker.Position = pos;
                _tipInfo.Text = info;
                _tipInfo.Visibility = Visibility.Visible;
            }
        }

        private void UpdateTipMarker(PointLatLng pos, string info = "")
        {
            if (_tipMarker != null && _tipInfo != null)
            {
                _tipMarker.Position = pos;
                if (!string.IsNullOrEmpty(info))
                {
                    _tipInfo.Text = info;
                }
            }
        }


        private void MyMap_OnMapZoomChanged()
        {
            tbZoom.Text = myMap.Zoom.ToString();
        }

        private void MyMap_MouseEnter(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(myMap);
            var afterPos = myMap.FromLocalToLatLng((int)pos.X, (int)pos.Y);
            if (_mapOperation == MapOperation.MeasureAreaPolygon)
            {
                ShowTipMarker(afterPos, "单击鼠标左键添加顶点；双击鼠标右键结束");
            }
            else if (_mapOperation == MapOperation.MeasureAreaRectangle)
            {
                ShowTipMarker(afterPos, "单击鼠标左键添加顶点；双击鼠标右键结束");
            }
            else if (_mapOperation == MapOperation.MeasureDistance)
            {
                ShowTipMarker(afterPos, "单击鼠标左键添加端点；双击鼠标右键结束");
            }
        }

        private void MyMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var localPos = e.GetPosition(myMap);
            _leftMouseDownPoint = localPos;
            var afterPos = myMap.FromLocalToLatLng((int)localPos.X, (int)localPos.Y);
            if (_mapOperation == MapOperation.MeasureAreaPolygon)
            {
                if (_currentPolygon == null)
                {
                    //ShowTipMarker(afterPos, "单击鼠标左键添加顶点；双击鼠标右键结束");
                    _currentPolygon = new MyMapPolygon(new List<PointLatLng>() { afterPos, afterPos });
                    _currentPolygon.Tag = Guid.NewGuid().ToString();
                    myMap.Markers.Add(_currentPolygon);
                }
                else
                {
                    var pointCount = _currentPolygon.Points.Count;
                    _currentPolygon.Points[pointCount - 1] = afterPos;
                    _currentPolygon.Points.Add(afterPos);
                }
            }
            else if (_mapOperation == MapOperation.MeasureAreaRectangle)
            {
                if (_currentPolygon == null)
                {
                    _currentPolygon = new MyMapPolygon(new List<PointLatLng>()
                    {
                        afterPos,afterPos,afterPos,afterPos
                    });
                    _currentPolygon.Tag = Guid.NewGuid().ToString();
                    myMap.Markers.Add(_currentPolygon);
                }
            }
            else if (_mapOperation == MapOperation.MeasureDistance)
            {
                if (_currentRoute == null)
                {
                    _currentRoute = new MyMapRoute(new List<PointLatLng>() { afterPos, afterPos });
                    _currentRoute.Tag = Guid.NewGuid().ToString();
                    myMap.Markers.Add(_currentRoute);
                }
                else
                {
                    var pointCount = _currentRoute.Points.Count;
                    _currentRoute.Points[pointCount - 1] = afterPos;
                    _currentRoute.Points.Add(afterPos);
                }
            }
        }

        private void MyMap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_mapOperation == MapOperation.Normal)
            {
                if (e.ChangedButton == MouseButton.Left && myMap.Zoom < myMap.MaxZoom)
                {
                    myMap.Zoom++;
                }
            }
            else if (_mapOperation == MapOperation.MeasureAreaPolygon &&
                _currentPolygon != null)
            {
                var position = e.GetPosition(myMap);
                var lngLat = myMap.FromLocalToLatLng((int)position.X, (int)position.Y);
                _currentPolygon.Points.RemoveAt(_currentPolygon.Points.Count - 1);
                var count = _currentPolygon.Points.Count;
                _currentPolygon.Points[count - 1] = lngLat;
                myMap.RegenerateShape(_currentPolygon);
                _currentPolygon.ShowArea();
                _currentPolygon = null;
                _mapOperation = MapOperation.Normal;
                HideTipMarker();
            }
            else if (_mapOperation == MapOperation.MeasureDistance &&
                _currentRoute != null)
            {
                var position = e.GetPosition(myMap);
                var lngLat = myMap.FromLocalToLatLng((int)position.X, (int)position.Y);
                _currentRoute.Points.RemoveAt(_currentRoute.Points.Count - 1);
                var count = _currentRoute.Points.Count;
                _currentRoute.Points[count - 1] = lngLat;
                myMap.RegenerateShape(_currentRoute);
                _currentRoute.ShowDistance();
                _currentRoute = null;
                _mapOperation = MapOperation.Normal;
                HideTipMarker();
            }
            else if (_mapOperation == MapOperation.MeasureAreaRectangle &&
                _currentPolygon != null)
            {
                _currentPolygon.ShowArea();
                _currentPolygon = null;
                _mapOperation = MapOperation.Normal;
                HideTipMarker();
                myMap.CanDragMap = true;
            }
        }

        private void MyMap_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(myMap);
            var lngLat = myMap.FromLocalToLatLng((int)position.X, (int)position.Y);
            tbLng.Text = lngLat.Lng.ToString("F6");
            tbLat.Text = lngLat.Lat.ToString("F6");

            UpdateTipMarker(lngLat);

            if (_mapOperation == MapOperation.MeasureAreaPolygon &&
                _currentPolygon != null)
            {
                var count = _currentPolygon.Points.Count;
                _currentPolygon.Points[count - 1] = lngLat;
                myMap.RegenerateShape(_currentPolygon);
            }
            else if (_mapOperation == MapOperation.MeasureDistance &&
                _currentRoute != null)
            {
                var count = _currentRoute.Points.Count;
                _currentRoute.Points[count - 1] = lngLat;
                myMap.RegenerateShape(_currentRoute);
                UpdateTipMarker(lngLat, $"长度：\n{Math.Round(_currentRoute.CalculateCurrentDistance(), 2)} 米");
            }
            else if (_mapOperation == MapOperation.MeasureAreaRectangle &&
                _currentPolygon != null)
            {
                var pointStart = _currentPolygon.Points[0];
                var width = position.X - _leftMouseDownPoint.X;
                var height = position.Y - _leftMouseDownPoint.Y;
                var point1 = myMap.FromLocalToLatLng((int)(_leftMouseDownPoint.X + width), (int)(_leftMouseDownPoint.Y));
                var point2 = myMap.FromLocalToLatLng((int)(_leftMouseDownPoint.X + width), (int)(_leftMouseDownPoint.Y + height));
                var point3 = myMap.FromLocalToLatLng((int)(_leftMouseDownPoint.X), (int)(_leftMouseDownPoint.Y + height));

                _currentPolygon.Points[1] = point1;
                _currentPolygon.Points[2] = point2;
                _currentPolygon.Points[3] = point3;
                myMap.RegenerateShape(_currentPolygon);
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is MapNodeModel model)
            {
                var points = Caller.ChinaRegion!.FindRegion(model.ProvinceName, model.CityName, model.AreaName)?.Points;
                if (points != null)
                {
                    _region!.Points = points;
                    myMap.RegenerateShape(_region);
                    var area = model.Area;
                    tbArea.Text = $"{Math.Round(area / 1000000.0, 2)}";
                    tbRegion.Text = model.NodeName;
                    myMap.Position = points[0];
                }
            }
        }

        private void cboMapProvider_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cb)
            {
                if (cb.SelectedIndex == 0)
                {
                    myMap.MapProvider = AMapHybridMapProvider.Instance;
                }
                else if (cb.SelectedIndex == 1)
                {
                    myMap.MapProvider = AMapSatelliteMapProvider.Instance;
                }
                else if (cb.SelectedIndex == 2)
                {
                    myMap.MapProvider = AMapProvider.Instance;
                }
            }
        }

        private void MeasureDistance_Click(object sender, RoutedEventArgs e)
        {
            _mapOperation = MapOperation.MeasureDistance;
            myMap.CanDragMap = true;
        }

        private void MeasureAreaCircle_Click(object sender, RoutedEventArgs e)
        {
            _mapOperation = MapOperation.MeasureAreaCircle;
            myMap.CanDragMap = true;
        }

        private void MeasureAreaPolygon_Click(object sender, RoutedEventArgs e)
        {
            _mapOperation = MapOperation.MeasureAreaPolygon;
            myMap.CanDragMap = true;
        }

        private void MeasureAreaRectangle_Click(object sender, RoutedEventArgs e)
        {
            _mapOperation = MapOperation.MeasureAreaRectangle;
            myMap.CanDragMap = false;
        }

        private void ClearShape_Click(object sender, RoutedEventArgs e)
        {
            var removes = myMap.Markers.Where(s => !Excludes.Contains(s.Tag?.ToString() ?? "")).ToList();
            for (int i = 0; i < removes.Count; i++)
            {
                myMap.Markers.Remove(removes[i]);
            }
        }
    }
}
