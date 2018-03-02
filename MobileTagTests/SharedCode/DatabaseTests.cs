using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobileTag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileTag.Models;

namespace MobileTag.Tests
{
    [TestClass()]
    public class DatabaseTests
    {
        public Cell TestCell;
        [TestInitialize]
        public void _init()
        {
            
        }

        [TestMethod()]
        public void GetCellTest()
        {
            Cell comparisonCell = new Cell(0);
            comparisonCell.Latitude = 44.677684000000000m;
             comparisonCell.Longitude =  57.23257900000000m;
            TestCell = Database.GetCell(0);
          
            Assert.AreEqual(comparisonCell.Latitude,TestCell.Latitude);
            Assert.AreEqual(comparisonCell.Longitude, TestCell.Longitude);
        }

        [TestMethod()]
        public void AddUserTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidateLoginCredentialsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetCellTeamTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetPlayerTest()
        {
            Assert.Fail();
        }
    }
}