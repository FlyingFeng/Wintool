using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WinTool.Models.MyMap;

namespace WinTool.MyMapLogic
{
    internal class MyMapPolygon : GMapPolygon
    {
        private Path? _path;
        private double _area = 0;
        private MyMapMarker? _marker;

        public MyMapPolygon(IEnumerable<PointLatLng> points) : base(points)
        {
        }

        public double Area
        {
            get
            {
                if (_path != null &&
                    Points.Count >= 3)
                {
                    if (_area == 0)
                    {
                        _area = China.CalculateArea(Points.Select(x => x.Lng).ToList(), Points.Select(x => x.Lat).ToList());
                    }
                }

                return _area;
            }
        }


        public void ShowArea()
        {
            if (_path == null ||
                Points.Count < 3)
            {
                return;
            }

            TextBlock _tipInfo = new TextBlock();
            _tipInfo.Foreground = Brushes.Black;
            _tipInfo.FontSize = 15;

            if (Area < 10000)
            {
                _tipInfo.Text = $"{Math.Round(Area, 2)}平方米";
            }
            else if (Area >= 10000 && Area < 1000000)
            {
                _tipInfo.Text = $"{Math.Round(Area / 10000.0, 2)}万平方米";
            }
            else
            {
                _tipInfo.Text = $"{Math.Round(Area / 1000000.0, 2)}平方千米";
            }
            _marker = new MyMapMarker(Points[0]);
            _marker.Shape = _tipInfo;
            this.Map.Markers.Add(_marker);

        }



        public override Path CreatePath(List<Point> localPath, bool addBlurEffect)
        {
            var p = base.CreatePath(localPath, addBlurEffect);
            if (_path == null)
            {
                _path = p;
                _path.Stroke = Brushes.MidnightBlue;
                _path.Fill = Brushes.AliceBlue;
                _path.StrokeThickness = 5;
            }
            else
            {
                _path.Data = p.Data;
            }

            return _path;
        }
    }
}
