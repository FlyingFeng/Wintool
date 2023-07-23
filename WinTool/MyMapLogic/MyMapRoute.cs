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
    internal class MyMapRoute : GMapRoute
    {

        private Path? _path;

        public MyMapRoute(IEnumerable<PointLatLng> points) : base(points)
        {
        }

        public double CalculateCurrentDistance()
        {
            if (Points.Count < 2)
            {
                return 0;
            }

            var count = Points.Count;
            var start = Points[count - 2];
            var end = Points[count - 1];
            var distance = China.GetDistance(start.Lng, start.Lat, end.Lng, end.Lat);

            return distance;
        }


        public void ShowDistance()
        {
            if (Points.Count < 2)
            {
                return;
            }

            for (int i = 0; i < Points.Count - 1; i++)
            {
                var start = Points[i];
                var end = Points[i + 1];
                var distance = China.GetDistance(start.Lng, start.Lat, end.Lng, end.Lat);
                var marker = new MyMapMarker(end);
                TextBlock t = new TextBlock();
                if (distance < 10000)
                {
                    t.Text = $"{Math.Round(distance, 2)} 米";
                }
                else
                {
                    t.Text = $"{Math.Round(distance / 1000, 2)} 千米";
                }
                t.Foreground = Brushes.Black;
                t.FontSize = 14;
                marker.Shape = t;

                this.Map.Markers.Add(marker);
            }
        }



        public override Path CreatePath(List<Point> localPath, bool addBlurEffect)
        {
            var p = base.CreatePath(localPath, addBlurEffect);
            if (_path == null)
            {
                _path = p;
                _path.Stroke = Brushes.MidnightBlue;
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
