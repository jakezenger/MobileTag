using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobileTag;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileTag.Models;
using Moq;


namespace MobileTag.Tests
{

    //[TestClass]
    //public void Save_CustomerIsNotNull_GetsAddedToRepository()
    //{
    ////Arrange
    //Mock<IContainer> mockContainer = new Mock<IContainer>();
    //Mock<ICustomerView> mockView = new Mock<ICustomerView>();

    //CustomerViewModel viewModel = new CustomerViewModel(mockView.Object, mockContainer.Object);
    //viewModel.CustomersRepository = new CustomersRepository();
    //viewModel.Customer = new Mock<Customer>().Object;

    ////Act
    //viewModel.Save();

    ////Assert
    //Assert.IsTrue(viewModel.CustomersRepository.Count == 1);
    //}


    [TestClass()]
    public class DatabaseTests
    {
        public Cell TestCell;
        public MockRepository MockRepository { get; private set; }
        [TestInitialize]
        public void _init()
        {
            
        }

        [TestMethod()]
        public void GetCellTest()
        {

            Mock<Cell> mockCell = new Mock<Cell>();




            //mockCell.SetupGet(doc => doc.UserName).Returns("TestPerson");
            //mockCell.SetupGet(doc => doc.Latitude).Returns(44.677684000000000m);
            //mockCell.SetupGet(doc => doc.Latitude).Returns(57.23257900000000m);

            //Cell comparisonCell = new Cell(0);
            //comparisonCell.Latitude = 44.677684000000000m;
            //comparisonCell.Longitude = 57.23257900000000m;
            //TestCell = Database.GetCell(0);

            //Assert.AreEqual(comparisonCell.Latitude, TestCell.Latitude);
            //Assert.AreEqual(comparisonCell.Longitude, TestCell.Longitude);
        }
        
        [TestMethod()]
        public void GetPlayerTest()
        {
            Team team = new Team(2, "Green");
            Player testPlayer = new Player(1, team, 0);
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