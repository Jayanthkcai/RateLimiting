using NUnit.Framework;
using Moq;
using OnlineBanking.Controllers;
using OnlineBanking.Models;
using OnlineBanking.Services;
using Microsoft.AspNetCore.Mvc;

namespace OnlineBankingTest
{
    [TestFixture]
    public class AccountTest
    {
        private Mock<IAccountService> _mockAccountService;
        private AccountController _accountController;

        [SetUp]
        public void Setup()
        {
            _mockAccountService = new Mock<IAccountService>();
            _accountController = new AccountController(_mockAccountService.Object);
        }

        [Test]
        public void GetBalance_ValidAccountId_ReturnsBalance()
        {
            // Arrange
            int accountId = 1;
            decimal expectedBalance = 100.50m;
            _mockAccountService.Setup(service => service.GetBalance(accountId)).Returns(expectedBalance);

            // Act
            var result = _accountController.GetBalance(accountId).Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(expectedBalance, result.Value);
        }

        [Test]
        public void GetBalance_InvalidAccountId_ReturnsNotFound()
        {
            // Arrange
            int accountId = 1;
            _mockAccountService.Setup(service => service.GetBalance(accountId)).Returns((decimal?)null);

            // Act
            var result = _accountController.GetBalance(accountId).Result as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Account not found", result.Value);
        }

        [Test]
        public void Transfer_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new TransferRequest { FromAccount = 1, ToAccount = 2, Amount = 50.00m };
            _mockAccountService.Setup(service => service.Transfer(request.FromAccount, request.ToAccount, request.Amount)).Returns(true);

            // Act
            var result = _accountController.Transfer(request) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Transfer successful", result.Value);
        }

        [Test]
        public void Transfer_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new TransferRequest { FromAccount = 1, ToAccount = 2, Amount = 50.00m };
            _mockAccountService.Setup(service => service.Transfer(request.FromAccount, request.ToAccount, request.Amount)).Returns(false);

            // Act
            var result = _accountController.Transfer(request) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Transfer failed", result.Value);
        }
    }
}
