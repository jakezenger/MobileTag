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
        [TestInitialize]
        public void _init()
        {

        }

        [TestMethod()]
        public async void GetCellTest()
        {
            decimal lat = 44.677684000000000m;
            decimal lng = 57.23257900000000m;
            Cell comparisonCell = new Cell(lat, lng);

            Cell TestCell = await Database.GetCell(0);

            Assert.AreEqual(comparisonCell.Latitude, TestCell.Latitude);
            Assert.AreEqual(comparisonCell.Longitude, TestCell.Longitude);
        }

        [TestMethod()]
        public async void GetPlayerTest()
        {
            Team team = new Team(2, "Green");
            Wallet wallet = new Wallet();
            wallet.Confinium = 10000;

            Player testPlayer = new Player(1, "ethan", team, 0, new List<Mine>() ,wallet);
            Player fromServer = await Database.GetPlayer("ethan");

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
