using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkClient.ViewModels
{
    class ClientViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ObservableCollection<Metric> _metric = new ObservableCollection<Metric>();
        private readonly Process _prc;
        public ClientViewModel(int id, Process prc)
        {
            Id = id;
            _prc = prc;
        }
        ~ClientViewModel()
        {
            if (_prc != null && !_prc.HasExited)
            {
                _prc.Kill();
            }
        }
        public void AddMetric(Metric metric)
        {
            metric.Normalize();
            Metrics.Add(metric);
            LastValue = metric;
        }

        private Metric _lastValue;
        public Metric LastValue
        {
            get
            {
                return _lastValue;
            }
            set
            {
                if (value != _lastValue)
                {
                    _lastValue = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<Metric> Metrics
        {
            get
            {
                return _metric;
            }
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public int Id { get; private set; }
    }

    class Metric
    {
        public void Normalize()
        {
            Min = Math.Round(Min);
            Max = Math.Round(Max);
            Avg = Math.Round(Avg);
        }
        public double Min { get; set; }

        public double Max { get; set; }

        public double Avg { get; set; }

        public DateTime Date { get; set; }

        public int Id { get; set; }
    }
}
