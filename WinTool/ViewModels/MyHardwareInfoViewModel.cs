using LibreHardwareMonitor.Hardware;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinTool.Models.MyHardwareMonitor;

namespace WinTool.ViewModels
{

    internal class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }

    internal class MyHardwareInfoViewModel : NotifyBaseObject
    {

        public ObservableCollection<HardwareInfo> ComputerInfo { get; set; } = new ObservableCollection<HardwareInfo>();
        private Computer? _computer;

        public MyHardwareInfoViewModel()
        {
            var root = new HardwareInfo
            {
                HardwareType = MyHardwareNodeType.Computer,
                Name = Environment.MachineName
            };

            ComputerInfo.Add(root);
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true
            };
            try
            {
                _computer.HardwareAdded += _computer_HardwareAdded;
                _computer.Open();
                Task.Run(async () =>
                {
                    var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
                    while (await timer.WaitForNextTickAsync())
                    {
                        Update();
                    }
                });

            }
            catch (Exception ex)
            {
                Log.Error($"Computer.Open() error, {ex.Message}\n{ex.StackTrace}");
            }
        }


        private HardwareInfo? FindNode(string name, HardwareInfo node)
        {
            if (node is null || node.Children is null)
            {
                return null;
            }

            if (node.Name == name)
            {
                return node;
            }

            foreach (var sub in node.Children)
            {
                var item = FindNode(name, sub);
                if (item is not null)
                {
                    return item;
                }
            }

            return null;
        }


        private void Update()
        {
            if (_computer is not null)
            {
                _computer.Accept(new UpdateVisitor());
                foreach (var hardWare in _computer.Hardware)
                {
                    foreach (var sensor in hardWare.Sensors)
                    {
                        var matched = FindNode(sensor.Name, ComputerInfo[0]);
                        if (matched is not null && matched.HardwareType == MyHardwareNodeType.Sensor)
                        {
                            matched.SensorValue = sensor.Value ?? 0;
                            matched.MinValue = sensor.Min ?? 0;
                            matched.MaxValue = sensor.Max ?? 0;
                        }
                    }
                }
            }
        }

        private void _computer_HardwareAdded(IHardware hardware)
        {
            var item = new HardwareInfo
            {
                HardwareType = MyHardwareNodeType.Hardware,
                Name = hardware.Name
            };
            var sensors = hardware.Sensors;
            if (sensors?.Length > 0)
            {
                var allGroups = sensors.Select(x => x.SensorType).Distinct();
                foreach (var group in allGroups)
                {
                    var groupItem = new HardwareInfo
                    {
                        Name = group.ToString(),
                        HardwareType = MyHardwareNodeType.Group
                    };
                    item.Children.Add(groupItem);
                }
                foreach (var eachSensor in sensors)
                {
                    var subSensor = new HardwareInfo
                    {
                        Name = eachSensor.Name,
                        HardwareType = MyHardwareNodeType.Sensor
                    };
                    var each = item.Children.Where(s => s.Name == eachSensor.SensorType.ToString()).FirstOrDefault();
                    each?.Children.Add(subSensor);
                }
            }

            ComputerInfo.First().Children.Add(item);
        }
    }
}
