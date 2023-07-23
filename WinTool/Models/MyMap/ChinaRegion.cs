using GMap.NET;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTool.Models.MyMap
{


    public class RegionItem
    {
        //public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Rings { get; set; } = string.Empty;

        private List<PointLatLng>? _points;
        public List<PointLatLng>? Points
        {
            get
            {
                if (_points != null)
                {
                    return _points;
                }


                if (string.IsNullOrEmpty(Rings))
                {
                    return default;
                }
                else
                {
                    if (_points == null)
                    {
                        //110.123 23.123,111.123 23.456,112.789 23.4521
                        //Rings = "117.39029331445574 40.22709438118154,117.35366397363859 40.235789561011686,117.34231539399046 40.244054510418415,117.33658967496107 40.27760410237,117.2958172224446 40.27836114590715,117.29431197240323 40.293167339231566,117.28838719176605 40.3015430827197,117.27439077904492 40.30891407168649,117.27135796893768 40.32549841286138,117.27504284334458 40.33233669724397,117.26229548803206 40.33769412370389,117.24181786176895 40.37043965813851,117.22668752189378 40.369085885109634,117.2236789269057 40.37545069518245";
                        _points = new List<PointLatLng>();
                        var strSpan = Rings.AsSpan();
                        var startIndex = 0;
                        try
                        {
                            for (int i = 0; i < strSpan.Length; i++)
                            {
                                if (strSpan[i] == ',')
                                {
                                    var subSpan = strSpan.Slice(startIndex, i - startIndex);
                                    var spaceIndex = subSpan.IndexOf(' ');
                                    var lng = double.Parse(subSpan.Slice(0, spaceIndex));
                                    var lat = double.Parse(subSpan.Slice(spaceIndex));
                                    _points.Add(new PointLatLng(lat, lng));
                                    startIndex = i + 1;
                                }
                            }

                            var lastIndex = strSpan.LastIndexOf(',');
                            var subSpan2 = strSpan.Slice(lastIndex + 1);
                            var spaceIndex2 = subSpan2.IndexOf(' ');
                            var lng2 = double.Parse(subSpan2.Slice(0, spaceIndex2));
                            var lat2 = double.Parse(subSpan2.Slice(spaceIndex2));
                            _points.Add(new PointLatLng(lat2, lng2));
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"get points error, {ex.Message}\n{ex.StackTrace}");
                            return default;
                        }


                        //var point = Rings.Split(',');
                        //foreach (var p in point)
                        //{
                        //    var each = p.Split(' ');
                        //    var lng = double.Parse(each[0]);
                        //    var lat = double.Parse(each[1]);
                        //    _points.Add(new PointLatLng(lat, lng));
                        //}
                    }

                    return _points;
                }
            }
        }

    }

    public class China
    {
        //public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<Province>? Province { get; set; }

        private const double EARTH_RADIUS = 6378137;//赤道半径(单位m)   

        /// <summary>
        /// 转化为弧度(rad) 
        /// </summary>
        /// <param name="d">经度或纬度</param>
        /// <returns>弧度</returns>
        private static double Rad(double d)
        {
            return d * Math.PI / 180.0;
        }


        internal static double GetDistance(double lng1, double lat1, double lng2, double lat2)
        {
            double radLat1 = Rad(lat1);
            double radLat2 = Rad(lat2);
            double a = radLat1 - radLat2;
            double b = Rad(lng1) - Rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            return s * EARTH_RADIUS;

        }

        internal static double CalculateArea(List<double> lons, List<double> lats)
        {
            double sum = 0;
            double prevcolat = 0;
            double prevaz = 0;
            double colat0 = 0;
            double az0 = 0;

            for (int i = 0; i < lats.Count; i++)
            {
                double colat = 2 * Math.Atan2(Math.Sqrt(Math.Pow(Math.Sin(lats[i] * Math.PI / 180 / 2), 2) + Math.Cos(lats[i] * Math.PI / 180) * Math.Pow(Math.Sin(lons[i] * Math.PI / 180 / 2), 2)), Math.Sqrt(1 - Math.Pow(Math.Sin(lats[i] * Math.PI / 180 / 2), 2) - Math.Cos(lats[i] * Math.PI / 180) * Math.Pow(Math.Sin(lons[i] * Math.PI / 180 / 2), 2)));
                double az = 0;
                if (lats[i] >= 90)
                {
                    az = 0;
                }
                else if (lats[i] <= -90)
                {
                    az = Math.PI;
                }
                else
                {
                    az = Math.Atan2(Math.Cos(lats[i] * Math.PI / 180) * Math.Sin(lons[i] * Math.PI / 180), Math.Sin(lats[i] * Math.PI / 180)) % (2 * Math.PI);
                }
                if (i == 0)
                {
                    colat0 = colat;
                    az0 = az;
                }
                if (i > 0 && i < lats.Count)
                {
                    sum = sum + (1 - Math.Cos(prevcolat + (colat - prevcolat) / 2)) * Math.PI * ((Math.Abs(az - prevaz) / Math.PI) - 2 * Math.Ceiling(((Math.Abs(az - prevaz) / Math.PI) - 1) / 2)) * Math.Sign(az - prevaz);
                }
                prevcolat = colat;
                prevaz = az;
            }
            sum = sum + (1 - Math.Cos(prevcolat + (colat0 - prevcolat) / 2)) * (az0 - prevaz);
            return 5.10072E14 * Math.Min(Math.Abs(sum) / 4 / Math.PI, 1 - Math.Abs(sum) / 4 / Math.PI);
        }




        internal RegionItem? FindRegion(string province, string city, string area)
        {
            if (string.IsNullOrEmpty(province) &&
                string.IsNullOrEmpty(city) &&
                string.IsNullOrEmpty(area))
            {
                throw new Exception("【省】【市】【区】不能同时为空");
            }
            if (string.IsNullOrEmpty(province))
            {
                throw new Exception("【省】不能为空");
            }

            RegionItem? result = null;
            if (Province != null)
            {
                result = Province.Where(s => s.Name == province).FirstOrDefault();
            }

            if (string.IsNullOrEmpty(city))
            {
                return result;
            }
            else
            {
                var p = result as Province;
                result = p?.City?.Where(s => s.Name == city).FirstOrDefault();
            }

            if (string.IsNullOrEmpty(area))
            {
                return result;
            }
            else
            {
                var c = result as City;
                return c?.Piecearea?.Where(s => s.Name == area).FirstOrDefault();
            }
        }

    }





    public class Province : RegionItem
    {
        public List<City>? City { get; set; }

    }

    public class City : RegionItem
    {
        public List<Piecearea>? Piecearea { get; set; }

    }

    public class Piecearea : RegionItem
    {


    }

}
