using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkClient
{
    public class ConfigViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _sendPeriod;
        public int SendPeriod
        {
            get
            {
                return _sendPeriod;
            }
            set
            {
                if(value!=_sendPeriod)
                {
                    _sendPeriod = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _packetSize;
        public int PacketSize
        {
            get
            {
                return _packetSize;
            }
            set
            {
                if(value!=_packetSize)
                {
                    _packetSize = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged(string propertyName = null)
        {
            var propertyChanged = PropertyChanged;
            if(propertyChanged!=null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
