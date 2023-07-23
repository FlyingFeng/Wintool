using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTool.MyMapLogic
{
    internal class AMapProvider : GMapProvider
    {

        public AMapProvider()
        {
            Copyright = "高德地图";
            RefererUrl = "http://www.amap.com";
            MinZoom = 1;
            MaxZoom = 18;
        }

        private static readonly string urlFormat = "http://webrd0{0}.is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x={1}&y={2}&z={3}";
        private GMapProvider[]? _overlays;
        private Guid _id = new Guid("d4b31479-61a6-43c7-8d73-8406cc705e72");


        public static AMapProvider Instance = new AMapProvider();

        public override Guid Id => _id;

        public override string Name => "高德地图";

        public override PureProjection Projection => MercatorProjection.Instance;

        public override GMapProvider[] Overlays
        {
            get
            {
                _overlays ??= new GMapProvider[] { this };
                return _overlays;
            }
        }

        public override PureImage? GetTileImage(GPoint pos, int zoom)
        {
            try
            {
                var url = MakeTileImageUrl(pos, zoom);
                return GetTileImageUsingHttp(url);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"-----------{ex.Message}\n{ex.StackTrace}----------");
                return default;
            }
        }


        private string MakeTileImageUrl(GPoint pos, int zoom)
        {
            var num = (pos.X + pos.Y) % 4 + 1;
            num = 3;
            string url = string.Format(urlFormat, num, pos.X, pos.Y, zoom);

            return url;
        }

    }
}
