using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinTool.Internal;

namespace WinTool.Models.MyMap
{
    internal class MapNodeModel
    {
        public NodeEnum NodeType { get; set; }

        public string NodeName { get; set; } = string.Empty;

        public string ProvinceName { get; set; } = string.Empty;

        public string CityName { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;

        public List<MapNodeModel> Children { get; set; } = new List<MapNodeModel>();

        private double _area = 0;
        public double Area
        {
            get
            {
                if (Points == null ||
                    Points.Count == 0)
                {
                    return 0;
                }


                if (_area == 0)
                {
                    var lngs = new List<double>();
                    var lats = new List<double>();
                    foreach (var point in Points)
                    {
                        lngs.Add(point.Lng);
                        lats.Add(point.Lat);
                    }
                    _area = China.CalculateArea(lngs, lats);
                }

                return _area;
            }
        }




        public List<PointLatLng>? Points
        {
            get
            {
                if (Caller.ChinaRegion != null)
                {
                    var region = Caller.ChinaRegion.FindRegion(this.ProvinceName, this.CityName, this.AreaName);

                    if (region != null)
                    {
                        return region.Points;
                    }
                }

                return default;
            }
        }


    }
}
