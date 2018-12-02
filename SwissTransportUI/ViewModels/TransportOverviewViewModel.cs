using System;
using System.Linq;
using SwissTransport;

namespace SwissTransportUI.ViewModels
{
    public class TransportOverviewViewModel
    {
        private string _startStation;
        private string _destinationStation;
        //private DateTime _dateAndTime;todo
        //private bool _isDateAndTimeArrivalForArrival;

        public TransportOverviewViewModel()
        {
            SearchCommand = new DelegateCommand(GetConnections, s => StartStation != null &&
                                                                     DestinationStation != null);
        }        

        public DelegateCommand SearchCommand { get; }

        public string StartStation
        {
            get =>_startStation;
            set
            {
                _startStation = value;

                SearchCommand.RaiseCanExecuteChanged();
            }
        }

        public string DestinationStation
        {
            get => _destinationStation;
            set
            {
                _destinationStation = value;

                SearchCommand.RaiseCanExecuteChanged();
            }
        }

        public DateTime DateAndTime { get; set; }

        public bool IsDateAndTimeArrivalForArrival { get; set; }
      

        public Connections Connections { get; set; }

        private void GetConnections()
        {
            var transport = new Transport();

            //var connections = transport.GetConnections(SearchInfos.StartStation, SearchInfos.DestinationStation);
            var connections = transport.GetConnections(_startStation, _destinationStation);

            if (IsDateAndTimeArrivalForArrival)//todo
            {
                //connections.ConnectionList.Where(c => c.To.ArrivalTimestamp <= SearchInfos.Time);
                connections.ConnectionList.Where(c => c.To.Arrival != "");
            }
            else
            {
                //connections.ConnectionList.Where(c => c.From.DepartureTimestamp <= SearchInfos.Time);
                connections.ConnectionList.Where(c => c.From.Departure != "");
            }
        }
    }
}
