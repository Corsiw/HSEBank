using Application.Interfaces;
using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Factories;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class BankAccountServiceTests
    {
        private Mock<IRepository<BankAccount>> _repositoryMock = null!;
        private Mock<IDomainFactory> _factoryMock = null!;
        private BankAccountService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<BankAccount>>();
            _factoryMock = new Mock<IDomainFactory>();
            _service = new BankAccountService(_repositoryMock.Object, _factoryMock.Object);
        }

        #region CreateAccountAsync Tests

        [Test]
        public async Task CreateAccountAsync_ShouldReturnOkResult_WhenValidParameters()
        {
            BankAccount account = new("Test", 100);
            _factoryMock.Setup(f => f.CreateBankAccount("Test", 100, null)).Returns(account);

            Result<BankAccount> result = await _service.CreateAccountAsync("Test", 100);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(account, result.Value);
            _repositoryMock.Verify(r => r.AddAsync(account), Times.Once);
        }

        [Test]
        public async Task CreateAccountAsync_ShouldReturnFailResult_WhenFactoryThrowsArgumentException()
        {
            _factoryMock.Setup(f => f.CreateBankAccount(It.IsAny<string>(), It.IsAny<decimal>(), null))
                        .Throws(new ArgumentException("Invalid name"));

            Result<BankAccount> result = await _service.CreateAccountAsync("", 100);

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Invalid name", result.Message);
        }

        [Test]
        public async Task CreateAccountAsync_ShouldReturnFailResult_WhenRepositoryThrowsException()
        {
            BankAccount account = new("Test", 100);
            _factoryMock.Setup(f => f.CreateBankAccount("Test", 100, null)).Returns(account);
            _repositoryMock.Setup(r => r.AddAsync(account)).ThrowsAsync(new Exception("Db error"));

            Result<BankAccount> result = await _service.CreateAccountAsync("Test", 100);

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Ошибка при создании счёта", result.Message);
        }

        #endregion

        #region UpdateBalanceAsync Tests

        [Test]
        public async Task UpdateBalanceAsync_ShouldReturnOkResult_WhenAccountExists()
        {
            BankAccount account = new("Test", 100);
            _repositoryMock.Setup(r => r.GetAsync(account.Id)).ReturnsAsync(account);

            Result<BankAccount> result = await _service.UpdateBalanceAsync(account.Id, 200);

            Assert.IsTrue(result.IsSuccess);
            if (result.Value != null)
            {
                Assert.AreEqual(200, result.Value.Balance);
            }

            _repositoryMock.Verify(r => r.UpdateAsync(account), Times.Once);
        }

        [Test]
        public async Task UpdateBalanceAsync_ShouldReturnFailResult_WhenAccountNotFound()
        {
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>())).ReturnsAsync((BankAccount?)null);

            Result<BankAccount> result = await _service.UpdateBalanceAsync(Guid.NewGuid(), 200);

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Счёт не найден", result.Message);
        }

        [Test]
        public async Task UpdateBalanceAsync_ShouldReturnFailResult_WhenUpdateThrowsArgumentException()
        {
            BankAccount account = new("Test", 100);
            _repositoryMock.Setup(r => r.GetAsync(account.Id)).ReturnsAsync(account);

            Result<BankAccount> result = await _service.UpdateBalanceAsync(account.Id, -50);

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("New balance cannot be negative", result.Message);
        }

        #endregion

        #region UpdateNameAsync Tests

        [Test]
        public async Task UpdateNameAsync_ShouldReturnOkResult_WhenAccountExists()
        {
            BankAccount account = new("Test", 100);
            _repositoryMock.Setup(r => r.GetAsync(account.Id)).ReturnsAsync(account);

            Result<BankAccount> result = await _service.UpdateNameAsync(account.Id, "NewName");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("NewName", result.Value?.Name);
            _repositoryMock.Verify(r => r.UpdateAsync(account), Times.Once);
        }

        [Test]
        public async Task UpdateNameAsync_ShouldReturnFailResult_WhenAccountNotFound()
        {
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>())).ReturnsAsync((BankAccount?)null);

            Result<BankAccount> result = await _service.UpdateNameAsync(Guid.NewGuid(), "NewName");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Счёт не найден", result.Message);
        }

        [Test]
        public async Task UpdateNameAsync_ShouldReturnFailResult_WhenUpdateThrowsArgumentException()
        {
            BankAccount account = new("Test", 100);
            _repositoryMock.Setup(r => r.GetAsync(account.Id)).ReturnsAsync(account);

            Result<BankAccount> result = await _service.UpdateNameAsync(account.Id, "");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Account name cannot be empty", result.Message);
        }

        #endregion

        #region DeleteAccountAsync Tests

        [Test]
        public async Task DeleteAccountAsync_ShouldReturnOkResult_WhenRepositorySucceeds()
        {
            Guid id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            Result result = await _service.DeleteAccountAsync(id);

            Assert.IsTrue(result.IsSuccess);
            _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Test]
        public async Task DeleteAccountAsync_ShouldReturnFailResult_WhenRepositoryThrowsException()
        {
            Guid id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(id)).ThrowsAsync(new Exception("Db error"));

            Result result = await _service.DeleteAccountAsync(id);

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Ошибка при удалении счёта", result.Message);
        }

        #endregion

        #region GetByIdAsync and GetAllAsync Tests

        [Test]
        public async Task GetByIdAsync_ShouldReturnAccount_WhenExists()
        {
            BankAccount account = new("Test", 100);
            _repositoryMock.Setup(r => r.GetAsync(account.Id)).ReturnsAsync(account);

            BankAccount? result = await _service.GetByIdAsync(account.Id);

            Assert.AreEqual(account, result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllAccounts()
        {
            List<BankAccount> accounts =
            [
                new BankAccount("A", 100),
                new BankAccount("B", 200)
            ];
            _repositoryMock.Setup(r => r.ListAsync()).ReturnsAsync(accounts);

            IEnumerable<BankAccount> result = await _service.GetAllAsync();

            CollectionAssert.AreEqual(accounts, result);
        }

        #endregion
    }
}
