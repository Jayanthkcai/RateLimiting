using NUnit.Framework;
using Moq;
using OnlineBanking.Controllers;
using OnlineBanking.Models;
using OnlineBanking.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OnlineBankingTest
{
    [TestFixture]
    public class TransactionTest
    {
        private Mock<ITransactionService> _mockTransactionService;
        private TransactionController _transactionController;

        [SetUp]
        public void Setup()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _transactionController = new TransactionController(_mockTransactionService.Object);
        }

        [Test]
        public async Task ProcessTransaction_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new TransactionRequest
            {
                FromAccount = "123456",
                ToAccount = "654321",
                Amount = 100.00m,
                Currency = "USD",
                Description = "Test transaction"
            };

            var transactionResult = new TransactionResult
            {
                IsSuccess = true,
                TransactionId = "txn123",
                Message = "Transaction successful"
            };

            _mockTransactionService.Setup(service => service.ProcessTransaction(request))
                                   .Returns(transactionResult);

            // Act
            var result = await _transactionController.ProcessTransaction(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(transactionResult, okResult.Value);
        }

        [Test]
        public async Task ProcessTransaction_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            _transactionController.ModelState.AddModelError("error", "Invalid request");

            // Act
            var result = await _transactionController.ProcessTransaction(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Invalid transaction request.", badRequestResult.Value);
        }

        [Test]
        public async Task ProcessTransaction_FailedTransaction_ReturnsInternalServerError()
        {
            // Arrange
            var request = new TransactionRequest
            {
                FromAccount = "123456",
                ToAccount = "654321",
                Amount = 100.00m,
                Currency = "USD",
                Description = "Test transaction"
            };

            var transactionResult = new TransactionResult
            {
                IsSuccess = false,
                Message = "Transaction failed"
            };

            _mockTransactionService.Setup(service => service.ProcessTransaction(request))
                                   .Returns(transactionResult);

            // Act
            var result = await _transactionController.ProcessTransaction(request);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual(transactionResult.Message, objectResult.Value);
        }
    }
}
