using System;

namespace SwissTransport
{
    public interface ITransport
    {
        Stations GetStations(string query);
        StationBoardRoot GetStationBoard(string station, DateTime date, DateTime time);
        Connections GetConnections(string fromStation, string toStation, DateTime date, DateTime time,
            bool isDateAndTimeForArrival);
    }
}