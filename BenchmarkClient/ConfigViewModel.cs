using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkClient
{
    public class ConfigViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ConfigViewModel()
        {
            SendPeriod = 100;
            PacketSize = 16;
            ClientCount = 16;
        }
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

        private int _clientCount;

        public int ClientCount
        {
            get
            {
                return _clientCount;
            }
            set
            {
                if(value != _clientCount)
                {
                    _clientCount = value;
                    OnPropertyChanged();
                }
            }
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChanged = PropertyChanged;
            if(propertyChanged!=null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
