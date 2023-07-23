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
    internal class AMapSatelliteMapProvider : GMapProvider
    {

        public AMapSatelliteMapProvider()
        {
            Copyright = "高德地图";
            RefererUrl = "http://www.amap.com";
        }

        private Guid _id = new Guid("a73280ff-6f98-4dde-afae-634a04f191b0");
        private GMapProvider[]? _overlays;

        public override Guid Id => _id;

        public static AMapSatelliteMapProvider Instance = new AMapSatelliteMapProvider();


        public override string Name => "高德卫星地图";

        public override PureProjection Projection => MercatorProjection.Instance;


        public override GMapProvider[] Overlays
        {
            get
            {
                if (_overlays == null)
                {
                    _overlays = new GMapProvider[]
                    {
                        this
                    };
                }

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


        string MakeTileImageUrl(GPoint pos, int zoom)
        {
            var num = (pos.X + pos.Y) % 4 + 1;
            num = 3;
            string url = string.Format(urlFormat, num, pos.X, pos.Y, zoom);
            return url;
        }

        private static readonly string urlFormat = "http://webst0{0}.is.autonavi.com/appmaptile?style=6&x={1}&y={2}&z={3}";

    }
}
