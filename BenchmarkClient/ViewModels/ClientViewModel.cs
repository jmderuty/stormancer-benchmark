using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public ClientViewModel(int id)
        {
            Id = id;
        }
        public void AddMetric(Metric metric)
        {
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
                if(value != _lastValue)
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
        public double Min { get; set; }

        public double Max { get; set; }

        public double Avg { get; set; }

        public DateTime Date { get; set; }

        public int Id { get; set; }
    }
}
