using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace SwissTransport
{
    public class Transport : ITransport
    {
        public Stations GetStations(string locationName)
        {
            var request = CreateWebRequest($"http://transport.opendata.ch/v1/locations?query={locationName}");
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();

            if (responseStream != null)
            {
                var message = new StreamReader(responseStream).ReadToEnd();
                var stations = JsonConvert.DeserializeObject<Stations>(message
                    , new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                return stations;
            }

            return null;
        }

        public StationBoardRoot GetStationBoard(string station, DateTime date, DateTime time)
        {
            var request = CreateWebRequest($"http://transport.opendata.ch/v1/stationboard?station={station}&date={date}&time={time}");
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();

            if (responseStream != null)
            {
                var readToEnd = new StreamReader(responseStream).ReadToEnd();
                var stationboard =
                    JsonConvert.DeserializeObject<StationBoardRoot>(readToEnd, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                return stationboard;
            }

            return null;
        }

        public Connections GetConnections(string fromStation, string toStation, DateTime date, DateTime time, bool isDateAndTimeForArrival)
        {
            var request = CreateWebRequest($"http://transport.opendata.ch/v1/connections?from={fromStation}&to={toStation}&isArrivalTime={isDateAndTimeForArrival}&time={time}&date={date}");
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();

            if (responseStream != null)
            {
                var readToEnd = new StreamReader(responseStream).ReadToEnd();
                var connections =
                    JsonConvert.DeserializeObject<Connections>(readToEnd, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                return connections;
            }

            return null;
        }

        private static WebRequest CreateWebRequest(string url)
        {
            var request = WebRequest.Create(url);
            var webProxy = WebRequest.DefaultWebProxy;

            webProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            request.Proxy = webProxy;
            
            return request;
        }
    }
}
