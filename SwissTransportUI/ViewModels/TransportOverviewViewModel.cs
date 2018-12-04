using SwissTransport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace SwissTransportUI.ViewModels
{
    public class TransportOverviewViewModel : INotifyPropertyChanged
    {
        private const string ERROR_DATE_OR_TIME_IS_NULL = "FEHLER: Die Datum- oder Zeitangabe hat keinen Wert.";
        private const string ERROR_STATION_NOT_AVAILABLE = "FEHLER: Die gewünschte Station gibt es nicht.";
        private const string GOOGLE_MAPS_LINK_BASE = "https://www.google.com/maps/search/?api=1&query=";

        private string _startStationName;
        private string _destinationStationName;
        private bool? _isDateAndTimeForArrival;
        private DateTime? _connectionSearchDate;
        private DateTime? _connectionSearchTime;
        private readonly Transport _transport;
        private string _stationName;
        private DateTime? _stationSearchDate;
        private DateTime? _stationSearchTime;

        public event PropertyChangedEventHandler PropertyChanged;

        public TransportOverviewViewModel()
        {
            _transport = new Transport();

            SearchConnectionCommand = new DelegateCommand(GetConnections, s => _startStationName != string.Empty &&
                                                                     _destinationStationName != string.Empty &&
                                                                     _connectionSearchDate != null &&
                                                                     _connectionSearchTime != null &&
                                                                     _isDateAndTimeForArrival != null);

            SearchSationCommand = new DelegateCommand(GetStationBoard, s => _stationName != string.Empty &&
                                                                            _stationSearchDate != null &&
                                                                            _stationSearchTime != null);

            SeeStartStationLocationCommand = new DelegateCommand(SeeStartStationLocation, s => _startStationName != string.Empty);
            SeeDestinationStationLocationCommand = new DelegateCommand(SeeDestinationStationLocation, s => _destinationStationName != string.Empty);
            SeeStationLocationCommand = new DelegateCommand(SeeStationLocation, s => _stationName != string.Empty);
        }

        public string StartStationName
        {
            get =>_startStationName;
            set
            {
                _startStationName = value;

                OnPropertyChanged(nameof(StartStations));
                SearchConnectionCommand.RaiseCanExecuteChanged();
                SeeStartStationLocationCommand.RaiseCanExecuteChanged();
            }
        }

        public string DestinationStationName
        {
            get => _destinationStationName;
            set
            {
                _destinationStationName = value;

                OnPropertyChanged(nameof(DestinationStations));
                SearchConnectionCommand.RaiseCanExecuteChanged();
                SeeDestinationStationLocationCommand.RaiseCanExecuteChanged();
            }
        }

        public string StationName
        {
            get => _stationName;
            set
            {
                _stationName = value; 

                OnPropertyChanged(nameof(Stations));
                SearchSationCommand.RaiseCanExecuteChanged();
                SeeStationLocationCommand.RaiseCanExecuteChanged();
            }
        }

        public DateTime? ConnectionSearchDate
        {
            get
            {
                if (_connectionSearchDate == null)
                {
                    return _connectionSearchDate = DateTime.Today;
                }

                return _connectionSearchDate;
            }
            set
            {
                _connectionSearchDate = value; 

                SearchConnectionCommand.RaiseCanExecuteChanged();
            }
        }

        public DateTime? StationSearchDate
        {
            get
            {
                if (_stationSearchDate == null)
                {
                    return _stationSearchDate = DateTime.Today;
                }

                return _stationSearchDate;
            }
            set
            {
                _connectionSearchDate = value;

                SearchSationCommand.RaiseCanExecuteChanged();
            }
        }

        public DateTime? ConnectionSearchTime
        {
            get
            {
                if (_connectionSearchTime == null)
                {
                    return _connectionSearchTime = DateTime.Now;
                }

                return _connectionSearchTime;
            }
            set
            {
                _connectionSearchTime = value;

                SearchConnectionCommand.RaiseCanExecuteChanged();
            }
        }

        public DateTime? StationSearchTime
        {
            get
            {
                if (_stationSearchTime == null)
                {
                    return _stationSearchTime = DateTime.Now;
                }

                return _stationSearchTime;
            }
            set
            {
                _stationSearchTime = value;

                SearchSationCommand.RaiseCanExecuteChanged();
            }
        }

        public bool? IsDateAndTimeForArrival
        {
            get
            {
                if (_isDateAndTimeForArrival == null)
                {
                    return _isDateAndTimeForArrival = true;
                }

                return _isDateAndTimeForArrival;
            }
            set
            {
                _isDateAndTimeForArrival = value;

                SearchConnectionCommand.RaiseCanExecuteChanged();
            }
        }

        public Connections Connections { get; set; }

        public StationBoardRoot StationBoard { get; set; }

        public List<string> StartStations
        {
            get
            {
                //s.Id != null gibt alle Station
                return GetStations(_startStationName).StationList
                    .Where(s => s.Id != null)
                    .Select(s => s.Name)
                    .ToList();               
            }
        }

        public List<string> DestinationStations
        {
            get
            {
                //s.Id != null gibt alle Station
                return GetStations(_destinationStationName).StationList
                    .Where(s => s.Id != null)
                    .Select(s => s.Name)
                    .ToList();
            }
        }

        public List<string> Stations
        {
            get
            {
                //s.Id != null gibt alle Station
                return GetStations(_stationName).StationList
                .Where(s => s.Id != null)
                .Select(s => s.Name)
                .ToList();
            }
        }

        public DelegateCommand SearchConnectionCommand { get; }

        public DelegateCommand SearchSationCommand { get; }

        public DelegateCommand SeeStartStationLocationCommand { get; }

        public DelegateCommand SeeDestinationStationLocationCommand { get; }

        public DelegateCommand SeeStationLocationCommand { get; }

        public void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GetConnections()
        {                        
            if (_connectionSearchDate.HasValue &&
                _connectionSearchTime.HasValue &&
                _isDateAndTimeForArrival.HasValue)
            {
                Connections = _transport.GetConnections(_startStationName, _destinationStationName, _connectionSearchDate.Value, _connectionSearchTime.Value, _isDateAndTimeForArrival.Value);
            }
            else
            {
                MessageBox.Show(ERROR_DATE_OR_TIME_IS_NULL);
                return;
            }                      

            OnPropertyChanged(nameof(Connections));
        }

        private void GetStationBoard()
        {
            if (_stationSearchDate.HasValue &&
                _stationSearchTime.HasValue)
            {
                StationBoard = _transport.GetStationBoard(_stationName, _stationSearchDate.Value, _stationSearchTime.Value);
            }
            else
            {
                MessageBox.Show(ERROR_DATE_OR_TIME_IS_NULL);
                return;
            }

            OnPropertyChanged(nameof(StationBoard));
        }

        /// <summary>
        /// Gets all Stations that have a similar name
        /// </summary>
        /// <param name="locationName"></param>
        /// <returns></returns>
        private Stations GetStations(string locationName)
        {
           return _transport.GetStations(locationName);
        }

        /// <summary>
        /// Gets the station by "stationName" parameter and passes the coordinates to OpenBrowser()
        /// </summary>
        /// <param name="stationName"></param>
        private void GetStationLocation(string stationName)
        {
            var station = GetStations(stationName).StationList.FirstOrDefault(s => s.Name == stationName);

            if (station == null)
            {
                MessageBox.Show(ERROR_STATION_NOT_AVAILABLE);
                return;
            }

            MessageBox.Show("öffne browser");

            OpenBrowser(GOOGLE_MAPS_LINK_BASE + station.Coordinate.XCoordinate + "," +
                        station.Coordinate.YCoordinate);

            MessageBox.Show("fertig");
        }

        private void SeeStartStationLocation()
        {
            GetStationLocation(_startStationName);   
        }

        private void SeeDestinationStationLocation()
        {
            GetStationLocation(_destinationStationName);
        }

        private void SeeStationLocation()
        {
            GetStationLocation(_stationName);
        }

        /// <summary>
        /// Opens default browser with the link from the parameter
        /// </summary>
        /// <param name="link"></param>
        private static void OpenBrowser(string link)
        {
            Process.Start(link);
        }

        [Conditional("DEBUG")]
        private void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                throw new ArgumentNullException(GetType().Name + " does not contain property: " + propertyName);
            }                
        }
    }
}
