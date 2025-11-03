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
    public class CategoryServiceTests
    {
        private Mock<IRepository<Category>> _repositoryMock = null!;
        private Mock<IDomainFactory> _factoryMock = null!;
        private CategoryService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Category>>();
            _factoryMock = new Mock<IDomainFactory>();
            _service = new CategoryService(_repositoryMock.Object, _factoryMock.Object);
        }

        #region CreateCategoryAsync

        [Test]
        public async Task CreateCategoryAsync_ShouldReturnOk_WhenValidData()
        {
            Category category = new(MoneyType.Income, "Salary");
            _factoryMock.Setup(f => f.CreateCategory(MoneyType.Income, "Salary", null)).Returns(category);

            Result<Category> result = await _service.CreateCategoryAsync(MoneyType.Income, "Salary");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(category, result.Value);
            _repositoryMock.Verify(r => r.AddAsync(category), Times.Once);
        }

        [Test]
        public async Task CreateCategoryAsync_ShouldReturnFail_WhenFactoryThrowsArgumentException()
        {
            _factoryMock.Setup(f => f.CreateCategory(It.IsAny<MoneyType>(), It.IsAny<string>(), null))
                        .Throws(new ArgumentException("Invalid category name"));

            Result<Category> result = await _service.CreateCategoryAsync(MoneyType.Expense, "");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Invalid category name", result.Message);
        }

        [Test]
        public async Task CreateCategoryAsync_ShouldReturnFail_WhenRepositoryThrowsException()
        {
            Category category = new(MoneyType.Income, "Salary");
            _factoryMock.Setup(f => f.CreateCategory(MoneyType.Income, "Salary", null)).Returns(category);
            _repositoryMock.Setup(r => r.AddAsync(category)).ThrowsAsync(new Exception("Database error"));

            Result<Category> result = await _service.CreateCategoryAsync(MoneyType.Income, "Salary");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Ошибка при создании категории", result.Message);
        }

        #endregion

        #region DeleteCategoryAsync

        [Test]
        public async Task DeleteCategoryAsync_ShouldReturnOk_WhenRepositorySucceeds()
        {
            Guid id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            Result result = await _service.DeleteCategoryAsync(id);

            Assert.IsTrue(result.IsSuccess);
            _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Test]
        public async Task DeleteCategoryAsync_ShouldReturnFail_WhenRepositoryThrowsException()
        {
            Guid id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(id)).ThrowsAsync(new Exception("DB failure"));

            Result result = await _service.DeleteCategoryAsync(id);

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Ошибка при удалении категории", result.Message);
        }

        #endregion

        #region UpdateNameAsync

        [Test]
        public async Task UpdateNameAsync_ShouldReturnOk_WhenCategoryExists()
        {
            Category category = new(MoneyType.Income, "OldName");
            _repositoryMock.Setup(r => r.GetAsync(category.Id)).ReturnsAsync(category);

            Result<Category> result = await _service.UpdateNameAsync(category.Id, "NewName");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("NewName", result.Value?.Name);
            _repositoryMock.Verify(r => r.UpdateAsync(category), Times.Once);
        }

        [Test]
        public async Task UpdateNameAsync_ShouldReturnFail_WhenCategoryNotFound()
        {
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<Guid>())).ReturnsAsync((Category?)null);

            Result<Category> result = await _service.UpdateNameAsync(Guid.NewGuid(), "NewName");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Категория не найдена", result.Message);
        }

        [Test]
        public async Task UpdateNameAsync_ShouldReturnFail_WhenUpdateThrowsArgumentException()
        {
            Category category = new(MoneyType.Income, "OldName");
            _repositoryMock.Setup(r => r.GetAsync(category.Id)).ReturnsAsync(category);

            Result<Category> result = await _service.UpdateNameAsync(category.Id, "");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Category name cannot be empty", result.Message);
        }

        [Test]
        public async Task UpdateNameAsync_ShouldReturnFail_WhenRepositoryThrowsException()
        {
            Category category = new(MoneyType.Income, "OldName");
            _repositoryMock.Setup(r => r.GetAsync(category.Id)).ReturnsAsync(category);
            _repositoryMock.Setup(r => r.UpdateAsync(category)).ThrowsAsync(new Exception("DB update failed"));

            Result<Category> result = await _service.UpdateNameAsync(category.Id, "NewName");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains("Ошибка при обновлении категории", result.Message);
        }

        #endregion

        #region GetByIdAsync and GetAllAsync

        [Test]
        public async Task GetByIdAsync_ShouldReturnCategory_WhenExists()
        {
            Category category = new(MoneyType.Income, "Salary");
            _repositoryMock.Setup(r => r.GetAsync(category.Id)).ReturnsAsync(category);

            Category? result = await _service.GetByIdAsync(category.Id);

            Assert.AreEqual(category, result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnListOfCategories()
        {
            List<Category> categories =
            [
                new(MoneyType.Income, "Salary"),
                new(MoneyType.Expense, "Food")
            ];
            _repositoryMock.Setup(r => r.ListAsync()).ReturnsAsync(categories);

            IEnumerable<Category> result = await _service.GetAllAsync();

            CollectionAssert.AreEqual(categories, result);
        }

        #endregion
    }
}
