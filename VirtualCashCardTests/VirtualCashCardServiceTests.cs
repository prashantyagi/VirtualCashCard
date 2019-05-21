using System;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace VirtualCashCard.Service.Tests
{
    public class VirtualCashCardServiceTests
    {
        private IVirtualCashCardService _virtualCashCardService;
        private Mock<IVirtualCashCardRepository> _virtualCashCardRepository;
        [SetUp]
        public void Setup()
        {
            _virtualCashCardRepository = new Mock<IVirtualCashCardRepository>();
            _virtualCashCardService = new VirtualCashCardService(_virtualCashCardRepository.Object);
        }

        #region Withdraw
        // Basic card test, One could add testcases for card format
        [Test]
        public void Withdraw_InvalidCard_ReturnError()
        {
            // Arrange
            var cardNumber = string.Empty;

            // Act
            var response = _virtualCashCardService.Withdraw(cardNumber, string.Empty, 1).Result;

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeFalse();
            response.ErrorMessage.ShouldBe("Invalid card number.");
        }

        // Basic pin test, One could add testcases for pin format
        [Test]
        public void Withdraw_InvalidPin_ReturnError()
        {
            // Arrange
            var cardNumber = "4444123456";
            var pin = string.Empty;

            // Act
            var response = _virtualCashCardService.Withdraw(cardNumber, pin,1).Result;

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeFalse();
            response.ErrorMessage.ShouldBe("Invalid pin number.");
        }

        [Test]
        public void Withdraw_UnknownCard_ReturnError()
        {
            // Arrange
            var cardNumber = "4444123456";
            var pin = "1234";
            _virtualCashCardRepository.Setup(x => x.GetAccount(cardNumber, pin))
                .ReturnsAsync(new Response<Account>() { ErrorMessage = "No associated account found." })
                .Verifiable();

            // Act
            var response = _virtualCashCardService.Withdraw(cardNumber, pin,1).Result;

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeFalse();
            response.ErrorMessage.ShouldBe("No associated account found.");
        }

        [Test]
        public void Withdraw_UnknownPin_ReturnError()
        {
            // Arrange
            var cardNumber = "4444123456";
            var pin = "1234";
            var account = new Account 
            {
                CardNumber = cardNumber
            };

            _virtualCashCardRepository.Setup(x => x.GetAccount(cardNumber, pin))
                .ReturnsAsync(new Response<Account>() { ErrorMessage = "Incorrect pin." })
                .Verifiable();

            // Act
            var response = _virtualCashCardService.Withdraw(cardNumber, pin,1).Result;

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeFalse();
            response.ErrorMessage.ShouldBe("Incorrect pin.");
        }

        // basic test, can add more testcases for min & max etc
        //eg [TestCase(0, "Amount cannot be zero.")]
        //eg [TestCase(500, "Amount specified is more than allowed.")]
        [Test]
        public void Withdraw_InvalidAmount_ReturnError()
        {
            // Arrange

            // Act
            var response = _virtualCashCardService.Withdraw("12345678", "1234", 0).Result;

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeFalse();
            response.ErrorMessage.ShouldBe("Invalid amount.");
        }

        [Test]
        public void Withdraw_InSufficientBalance_ReturnError()
        {
            // Arrange
            var cardNumber = "4444123456";
            var pin = "1234";
            int amount = 20;
            var account = new Account
            {
                Id = 1,
                CardNumber = cardNumber
            };

            _virtualCashCardRepository.Setup(x => x.GetAccount(cardNumber, pin))
                .ReturnsAsync(new Response<Account>() { Success = true, Data = account })
                .Verifiable();
            _virtualCashCardRepository.Setup(x => x.Withdraw(account.Id, amount))
               .ReturnsAsync(new Response { ErrorMessage = "Insufficient balance." })
               .Verifiable();

            // Act
            var response = _virtualCashCardService.Withdraw(cardNumber, pin, amount).Result;

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeFalse();
            response.ErrorMessage.ShouldBe("Insufficient balance.");
        }

        [Test]
        public void Withdraw_RepositoryWithdrawException_ReturnError()
        {
            // Arrange
            var cardNumber = "4444123456";
            var pin = "1234";
            int amount = 20;
            var account = new Account
            {
                Id = 1,
                CardNumber = cardNumber
            };

            _virtualCashCardRepository.Setup(x => x.GetAccount(cardNumber, pin))
                .ReturnsAsync(new Response<Account>() { Success = true, Data = account })
                .Verifiable();
            _virtualCashCardRepository.Setup(x => x.Withdraw(account.Id, amount))
               .ThrowsAsync(new Exception("Unhandled Repository exception..."))
               .Verifiable();

            // Act
            var response = _virtualCashCardService.Withdraw(cardNumber, pin, amount).Result;

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeFalse();
            response.ErrorMessage.ShouldBe("Unexpected error while withdrawing.");
        }

        [Test]
        public void Withdraw_HappyPath_ReturnSuccess()
        {
            // Arrange
            var cardNumber = "4444123456";
            var pin = "1234";
            int amount = 20;
            var account = new Account
            {
                Id = 1,
                CardNumber = cardNumber
            };

            _virtualCashCardRepository.Setup(x => x.GetAccount(cardNumber, pin))
                .ReturnsAsync(new Response<Account>() { Success = true, Data = account })
                .Verifiable();
            _virtualCashCardRepository.Setup(x => x.Withdraw(account.Id, amount))
               .ReturnsAsync(new Response { Success = true })
               .Verifiable();

            // Act
            var response = _virtualCashCardService.Withdraw(cardNumber, pin, amount).Result;

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeTrue();
        }

        #endregion

        #region Deposit

        [Test]
        public void Deposit_InvalidAccount_ReturnError()
        {
            // Arrange
            var accountNumber = string.Empty;

            // Act
            var response = _virtualCashCardService.Deposit(accountNumber, 1).Result;

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeFalse();
            response.ErrorMessage.ShouldBe("Invalid account number.");
        }
        [Test]
        public void Deposit_InvalidAmount_ReturnError()
        {
            // Arrange

            // Act
            var response = _virtualCashCardService.Deposit("12345678", 0).Result;

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeFalse();
            response.ErrorMessage.ShouldBe("Invalid amount.");
        }

        [Test]
        public void Deposit_UnknownAccount_ReturnError()
        {
            // Arrange
            var accountNumber = "00123456";
            _virtualCashCardRepository.Setup(x => x.GetAccount(accountNumber))
                .ReturnsAsync(new Response<Account>() { ErrorMessage = "No associated account found." })
                .Verifiable();

            // Act
            var response = _virtualCashCardService.Deposit(accountNumber, 1).Result;

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeFalse();
            response.ErrorMessage.ShouldBe("No associated account found.");
        }

        [Test]
        public void Deposit_HappyPath_ReturnSuccess()
        {
            // Arrange
            var accountNumber = "4444123456";
            int amount = 20;
            var account = new Account
            {
                Id = 1,
                AccountNumber = accountNumber,
                CardNumber = "34324234"
            };

            _virtualCashCardRepository.Setup(x => x.GetAccount(accountNumber))
                .ReturnsAsync(new Response<Account>() { Success = true, Data = account })
                .Verifiable();
            _virtualCashCardRepository.Setup(x => x.Deposit(account.Id, amount))
               .ReturnsAsync(new Response { Success = true })
               .Verifiable();

            // Act
            var response = _virtualCashCardService.Deposit(accountNumber, amount).Result;

            // Assert
            response.ShouldNotBeNull();
            response.Success.ShouldBeTrue();
        }
        #endregion
    }
}