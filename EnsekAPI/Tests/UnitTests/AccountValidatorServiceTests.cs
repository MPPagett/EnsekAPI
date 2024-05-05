using NUnit.Framework;
using EnsekAPI.Services;
using EnsekAPI.Data;
using Microsoft.EntityFrameworkCore;
using EnsekAPI.Data.Models;

namespace EnsekAPI.Tests.UnitTests
{
    [TestFixture]
    public class AccountValidatorServiceTests
    {
        private AccountValidatorService _accountValidatorService;
        private EnsekDbContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EnsekDbContext>()
            .UseInMemoryDatabase(databaseName: "Ensek")
            .Options;

            _dbContext = new EnsekDbContext(options);
            _dbContext.Accounts.Add(new Account { Id = 1, FirstName = "Test1", LastName = "Test1" });
            _dbContext.Accounts.Add(new Account { Id = 2, FirstName = "Test2", LastName = "Test2" });
            _dbContext.SaveChanges();

            _accountValidatorService = new AccountValidatorService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted(); // Ensure the database is deleted after each test
        }

        #region Positive
        [Test]
        public void ValidateAccount_AccountExists_ReturnsTrue()
        {
            // Arrange
            // Act
            var result = _accountValidatorService.ValidateAccount("1");

            // Assert
            Assert.That(result.Item1);
            Assert.That(result.Item2 == 1);
        }

        #endregion

        #region Negative
        [Test]
        public void ValidateAccount_AccountDoesNotExist_ReturnsFalse()
        {
            // Arrange
            // Act
            var result = _accountValidatorService.ValidateAccount("3");

            // Assert
            Assert.That(result.Item1 == false);
            Assert.That(result.Item2 == 3);
        }

        [Test]
        [TestCase("xyz")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("!$%")]

        public void ValidateAccount_InputInvalid_ReturnsFalse(string input)
        {
            // Arrange
            // Act
            var result = _accountValidatorService.ValidateAccount(input);

            // Assert
            Assert.That(!result.Item1);
            Assert.That(result.Item2 == 0);
        }
        #endregion
    }
}