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
    internal class AMapHybridMapProvider : GMapProvider
    {

        public AMapHybridMapProvider()
        {
            Copyright = "高德地图";
            RefererUrl = "http://www.amap.com";
        }

        private readonly Guid _id = new Guid("6cd5b7ac-1863-4fe6-88b2-657313177b76");

        public override Guid Id => _id;
        private GMapProvider[]? _overlays;

        public override string Name => "高德混合地图";

        public override PureProjection Projection => MercatorProjection.Instance;
        public static AMapHybridMapProvider Instance = new AMapHybridMapProvider();

        public override GMapProvider[] Overlays
        {
            get
            {
                if (_overlays == null)
                {
                    _overlays = new GMapProvider[]
                    {
                        AMapSatelliteMapProvider.Instance,this
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

        static readonly string urlFormat = "http://webst0{0}.is.autonavi.com/appmaptile?style=8&x={1}&y={2}&z={3}";

    }
}
