using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SwissTransport
{
    [TestClass]
    public class TransportTest
    {
        private ITransport _testee;

        [TestMethod]
        public void Locations()
        {
            _testee = new Transport();
            var stations = _testee.GetStations("Sursee,");

            Assert.AreEqual(10, stations.StationList.Count);
        }

        [TestMethod]
        public void StationBoard()
        {
            _testee = new Transport();
            var stationBoard = _testee.GetStationBoard("Sursee", DateTime.Today, DateTime.Now);

            Assert.IsNotNull(stationBoard);
        }

        [TestMethod]
        public void Connections()
        {
            _testee = new Transport();
            var connections = _testee.GetConnections("Sursee", "Luzern", DateTime.Today, DateTime.Now, true);

            Assert.IsNotNull(connections);
        }
    }
}
