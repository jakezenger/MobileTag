using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobileTag;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileTag.Models;


namespace MobileTag.Tests
{
    

    [TestClass()]
    public class DatabaseTests
    {
        
        [TestInitialize]
        public void _init()
        {
            
        }

        [TestMethod()]
        public void GetCellTest()
        {
            Cell comparisonCell = new Cell(0);
            comparisonCell.Latitude = 44.677684000000000m;
            comparisonCell.Longitude = 57.23257900000000m;
            Cell TestCell = Database.GetCell(0);

            Assert.AreEqual(comparisonCell.Latitude, TestCell.Latitude);
            Assert.AreEqual(comparisonCell.Longitude, TestCell.Longitude);
        }

        [TestMethod()]
        public void GetPlayerTest()
        {
            Team team = new Team(2, "Green");
            Player testPlayer = new Player(1, "ethan", team, 0);
            Player fromServer = Database.GetPlayer("ethan");

            Assert.AreEqual(testPlayer.ID, fromServer.ID);
            Assert.AreEqual(testPlayer.Team.ID, fromServer.Team.ID);
            Assert.AreEqual(testPlayer.Team.TeamName, fromServer.Team.TeamName);
        }

        [TestMethod()]
        public void ValidateLoginCredentialsTest1()
        {
            Assert.AreEqual(1, Database.ValidateLoginCredentials("nicknick", "nicknick"));
        }
    }
}
