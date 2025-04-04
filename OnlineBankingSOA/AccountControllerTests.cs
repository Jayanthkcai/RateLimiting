using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineBanking.Controllers;
using OnlineBanking.Models;
using OnlineBanking.Services;
using System;

namespace OnlineBanking.Controllers.Tests
{
    [TestClass]
    public class AccountControllerTests
    {
        private AccountController _controller;
        private Mock<IAccountService> _mockAccountService;

        [TestInitialize]
        public void Setup()
        {
            _mockAccountService = new Mock<IAccountService>();
            _controller = new AccountController(_mockAccountService.Object);
        }

        [TestMethod]
        public void AccountControllerTest()
        {
            Assert.IsNotNull(_controller);
        }

        [TestMethod]
        public void GetBalanceTest()
        {
            var accountId = 1001;
            _mockAccountService.Setup(service => service.GetBalance(accountId)).Returns(100m);
            var result = _controller.GetBalance(accountId);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Value, typeof(decimal));
        }

        [TestMethod]
        public void TransferTest()
        {
            var fromAccountId = 1001;
            var toAccountId = 1002;
            var amount = 100m;
            _mockAccountService.Setup(service => service.Transfer(fromAccountId, toAccountId, amount)).Returns(true);
            var result = _controller.Transfer(new TransferRequest { FromAccount = fromAccountId, ToAccount = toAccountId, Amount = amount }) as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsTrue((bool)result.Value);
        }
    }
}
