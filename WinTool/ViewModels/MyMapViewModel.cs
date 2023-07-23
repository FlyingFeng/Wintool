using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinTool.Internal;
using WinTool.Models.MyMap;

namespace WinTool.ViewModels
{
    internal class MyMapViewModel : NotifyBaseObject
    {

        private bool _isLoaded;
        public MyMapViewModel()
        {
            if (!_isLoaded)
            {
                try
                {
                    //var filePath = Path.Combine(AppContext.BaseDirectory, "RegionData", "ChinaBoundary");
                    //var text = File.ReadAllText(filePath);
                    //var obj = JsonConvert.DeserializeObject<China>(text);
                    //Caller.ChinaRegion = obj;
                    Log.Information("loaded China region data finished...");
                    InitMapData();
                    _isLoaded = true;
                }
                catch (Exception ex)
                {
                    Log.Error($"load china region data error, {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        public List<MapNodeModel> MapNodes { get; set; } = new List<MapNodeModel>();

        public void InitMapData()
        {

            if (Caller.ChinaRegion != null)
            {
                try
                {
                    if (Caller.ChinaRegion.Province != null)
                    {
                        foreach (Province p in Caller.ChinaRegion.Province)
                        {
                            var node = new MapNodeModel
                            {
                                ProvinceName = p.Name,
                                NodeName = p.Name,
                                NodeType = NodeEnum.Province
                            };
                            MapNodes.Add(node);
                            if (p.City != null)
                            {
                                foreach (City city in p.City)
                                {
                                    var cityNode = new MapNodeModel
                                    {
                                        ProvinceName = p.Name,
                                        CityName = city.Name,
                                        NodeName = city.Name,
                                        NodeType = NodeEnum.City
                                    };
                                    node.Children.Add(cityNode);

                                    if (city.Piecearea != null)
                                    {
                                        foreach (Piecearea area in city.Piecearea)
                                        {
                                            var areaNode = new MapNodeModel
                                            {
                                                AreaName = area.Name,
                                                CityName = city.Name,
                                                NodeName = area.Name,
                                                NodeType = NodeEnum.Area,
                                                ProvinceName = p.Name
                                            };
                                            cityNode.Children.Add(areaNode);
                                        }
                                    }

                                }
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"InitMapData error, {ex.Message}\n{ex.StackTrace}");
                    MapNodes.Clear();
                }
            }

        }


        private ICommand? _loadedCommand;
        public ICommand? LoadedCommand
        {
            get => (_loadedCommand ??= new NormalCommand((s) =>
            {
                //if (!_isLoaded)
                //{
                //    try
                //    {
                //        var filePath = Path.Combine(AppContext.BaseDirectory, "RegionData", "ChinaBoundary");
                //        var text = File.ReadAllText(filePath);
                //        var obj = JsonConvert.DeserializeObject<China>(text);
                //        Caller.ChinaRegion = obj;
                //        Log.Information("loaded China region data finished...");
                //        InitMapData();
                //        _isLoaded = true;
                //    }
                //    catch (Exception ex)
                //    {
                //        Log.Error($"load china region data error, {ex.Message}\n{ex.StackTrace}");
                //    }
                //}
            }));
        }


    }
}
