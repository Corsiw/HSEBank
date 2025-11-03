using Application.Interfaces;
using Application.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Factories;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class OperationServiceTests
    {
        private Mock<IRepository<Operation>> _operationRepoMock = null!;
        private Mock<IRepository<BankAccount>> _bankRepoMock = null!;
        private Mock<IRepository<Category>> _categoryRepoMock = null!;
        private Mock<IDomainFactory> _factoryMock = null!;
        private OperationService _service = null!;

        [SetUp]
        public void Setup()
        {
            _operationRepoMock = new Mock<IRepository<Operation>>();
            _bankRepoMock = new Mock<IRepository<BankAccount>>();
            _categoryRepoMock = new Mock<IRepository<Category>>();
            _factoryMock = new Mock<IDomainFactory>();

            _service = new OperationService(
                _operationRepoMock.Object,
                _bankRepoMock.Object,
                _categoryRepoMock.Object,
                _factoryMock.Object);
        }

        #region AddOperationAsync

        [Test]
        public async Task AddOperationAsync_ShouldReturnOk_WhenValidIncomeOperation()
        {
            // Arrange
            BankAccount bank = new("TestBank", 1000m);
            Category category = new(MoneyType.Income, "Salary");
            Operation operation = new(MoneyType.Income, bank.Id, category.Id, 500m, DateTime.Now, "Payday");

            _bankRepoMock.Setup(r => r.GetAsync(bank.Id)).ReturnsAsync(bank);
            _categoryRepoMock.Setup(r => r.GetAsync(category.Id)).ReturnsAsync(category);
            _factoryMock.Setup(f => f.CreateOperation(MoneyType.Income, bank.Id, category.Id, 500m, It.IsAny<DateTime>(), "Payday", null)).Returns(operation);

            // Act
            Result<Operation> result = await _service.AddOperationAsync(MoneyType.Income, bank.Id, category.Id, 500m, DateTime.Now, "Payday");

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(operation, result.Value);
            Assert.AreEqual(1500m, bank.Balance); // Баланс должен увеличиться
            _operationRepoMock.Verify(r => r.AddAsync(operation), Times.Once);
            _bankRepoMock.Verify(r => r.UpdateAsync(bank), Times.Once);
        }

        [Test]
        public async Task AddOperationAsync_ShouldReturnOk_WhenValidExpenseOperation()
        {
            BankAccount bank = new("Card", 1000m);
            Category category = new(MoneyType.Expense, "Groceries");
            Operation operation = new(MoneyType.Expense, bank.Id, category.Id, 200m, DateTime.Now, "Food");

            _bankRepoMock.Setup(r => r.GetAsync(bank.Id)).ReturnsAsync(bank);
            _categoryRepoMock.Setup(r => r.GetAsync(category.Id)).ReturnsAsync(category);
            _factoryMock.Setup(f => f.CreateOperation(MoneyType.Expense, bank.Id, category.Id, 200m, It.IsAny<DateTime>(), "Food",  null)).Returns(operation);

            Result<Operation> result = await _service.AddOperationAsync(MoneyType.Expense, bank.Id, category.Id, 200m, DateTime.Now, "Food");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(operation, result.Value);
            Assert.AreEqual(800m, bank.Balance); // Баланс должен уменьшиться
        }

        [Test]
        public async Task AddOperationAsync_ShouldReturnFail_WhenAccountNotFound()
        {
            _bankRepoMock.Setup(r => r.GetAsync(It.IsAny<Guid>())).ReturnsAsync((BankAccount?)null);

            Result<Operation> result = await _service.AddOperationAsync(MoneyType.Income, Guid.NewGuid(), Guid.NewGuid(), 100, DateTime.Now);

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Счёт не найден", result.Message);
        }

        [Test]
        public async Task AddOperationAsync_ShouldReturnFail_WhenCategoryNotFound()
        {
            BankAccount bank = new("Bank", 500);
            _bankRepoMock.Setup(r => r.GetAsync(bank.Id)).ReturnsAsync(bank);
            _categoryRepoMock.Setup(r => r.GetAsync(It.IsAny<Guid>())).ReturnsAsync((Category?)null);

            Result<Operation> result = await _service.AddOperationAsync(MoneyType.Expense, bank.Id, Guid.NewGuid(), 100, DateTime.Now);

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Категория не найдена", result.Message);
        }

        [Test]
        public async Task AddOperationAsync_ShouldReturnFail_WhenFactoryThrowsArgumentException()
        {
            BankAccount bank = new("Bank", 500);
            Category category = new(MoneyType.Income, "Salary");
            _bankRepoMock.Setup(r => r.GetAsync(bank.Id)).ReturnsAsync(bank);
            _categoryRepoMock.Setup(r => r.GetAsync(category.Id)).ReturnsAsync(category);
            _factoryMock.Setup(f => f.CreateOperation(It.IsAny<MoneyType>(), bank.Id, category.Id, -50m, It.IsAny<DateTime>(), null, null))
                        .Throws(new ArgumentException("Amount must be positive."));

            Result<Operation> result = await _service.AddOperationAsync(MoneyType.Income, bank.Id, category.Id, -50m, DateTime.Now);

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Amount must be positive", result.Message);
        }

        [Test]
        public async Task AddOperationAsync_ShouldReturnFail_WhenRepositoryThrowsException()
        {
            BankAccount bank = new("Bank", 1000);
            Category category = new(MoneyType.Income, "Salary");
            Operation operation = new(MoneyType.Income, bank.Id, category.Id, 100, DateTime.Now, "Test");

            _bankRepoMock.Setup(r => r.GetAsync(bank.Id)).ReturnsAsync(bank);
            _categoryRepoMock.Setup(r => r.GetAsync(category.Id)).ReturnsAsync(category);
            _factoryMock.Setup(f => f.CreateOperation(It.IsAny<MoneyType>(), bank.Id, category.Id, 100, It.IsAny<DateTime>(), "Test", null))
                        .Returns(operation);
            _operationRepoMock.Setup(r => r.AddAsync(operation)).ThrowsAsync(new Exception("Database error"));

            Result<Operation> result = await _service.AddOperationAsync(MoneyType.Income, bank.Id, category.Id, 100, DateTime.Now, "Test");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Ошибка при добавлении операции", result.Message);
        }

        #endregion

        #region DeleteOperationAsync

        [Test]
        public async Task DeleteOperationAsync_ShouldReturnOk_WhenRepositorySucceeds()
        {
            Guid id = Guid.NewGuid();
            _operationRepoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            Result result = await _service.DeleteOperationAsync(id);

            Assert.IsTrue(result.IsSuccess);
            _operationRepoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Test]
        public async Task DeleteOperationAsync_ShouldReturnFail_WhenRepositoryThrowsException()
        {
            Guid id = Guid.NewGuid();
            _operationRepoMock.Setup(r => r.DeleteAsync(id)).ThrowsAsync(new Exception("DB failure"));

            Result result = await _service.DeleteOperationAsync(id);

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Ошибка при удалении операции", result.Message);
        }

        #endregion

        #region UpdateDescriptionAsync

        [Test]
        public async Task UpdateDescriptionAsync_ShouldReturnOk_WhenOperationExists()
        {
            Operation op = new Operation(MoneyType.Expense, Guid.NewGuid(), Guid.NewGuid(), 100, DateTime.Now, "Old");
            _operationRepoMock.Setup(r => r.GetAsync(op.Id)).ReturnsAsync(op);

            Result<Operation> result = await _service.UpdateDescriptionAsync(op.Id, "Updated desc");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Updated desc", result.Value?.Description);
            _operationRepoMock.Verify(r => r.UpdateAsync(op), Times.Once);
        }

        [Test]
        public async Task UpdateDescriptionAsync_ShouldReturnFail_WhenOperationNotFound()
        {
            _operationRepoMock.Setup(r => r.GetAsync(It.IsAny<Guid>())).ReturnsAsync((Operation?)null);

            Result<Operation> result = await _service.UpdateDescriptionAsync(Guid.NewGuid(), "Something");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Операция не найдена", result.Message);
        }

        [Test]
        public async Task UpdateDescriptionAsync_ShouldReturnFail_WhenRepositoryThrowsException()
        {
            Operation op = new(MoneyType.Income, Guid.NewGuid(), Guid.NewGuid(), 50, DateTime.Now, "Desc");
            _operationRepoMock.Setup(r => r.GetAsync(op.Id)).ReturnsAsync(op);
            _operationRepoMock.Setup(r => r.UpdateAsync(op)).ThrowsAsync(new Exception("DB update failed"));

            Result<Operation> result = await _service.UpdateDescriptionAsync(op.Id, "New desc");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Ошибка при обновлении операции", result.Message);
        }

        #endregion

        #region GetByIdAsync and GetAllAsync

        [Test]
        public async Task GetByIdAsync_ShouldReturnOperation_WhenExists()
        {
            Operation op = new Operation(MoneyType.Income, Guid.NewGuid(), Guid.NewGuid(), 100, DateTime.Now);
            _operationRepoMock.Setup(r => r.GetAsync(op.Id)).ReturnsAsync(op);

            Operation? result = await _service.GetByIdAsync(op.Id);

            Assert.AreEqual(op, result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnListOfOperations()
        {
            List<Operation> list =
            [
                new(MoneyType.Income, Guid.NewGuid(), Guid.NewGuid(), 100, DateTime.Now),
                new(MoneyType.Expense, Guid.NewGuid(), Guid.NewGuid(), 50, DateTime.Now)
            ];
            _operationRepoMock.Setup(r => r.ListAsync()).ReturnsAsync(list);

            IEnumerable<Operation> result = await _service.GetAllAsync();

            CollectionAssert.AreEqual(list, result);
        }

        #endregion
    }
}
