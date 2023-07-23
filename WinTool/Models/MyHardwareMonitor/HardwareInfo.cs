using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTool.Models.MyHardwareMonitor
{
    internal class HardwareInfo : NotifyBaseObject
    {

        public MyHardwareNodeType HardwareType { get; set; }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisedPropertyChanged();
            }
        }

        private float _maxValue = 0;
        public float MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                RaisedPropertyChanged();
            }
        }

        private float _minValue = 0;
        public float MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                RaisedPropertyChanged();
            }
        }

        private float _sensorValue = 0;
        public float SensorValue
        {
            get => _sensorValue;
            set
            {
                _sensorValue = value;
                RaisedPropertyChanged();
            }
        }

        public ObservableCollection<HardwareInfo> Children { get; set; } = new ObservableCollection<HardwareInfo>();

    }
}
