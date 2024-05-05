using NUnit.Framework;
using EnsekAPI.Services;
using Moq;
using EnsekAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using EnsekAPI.Data.Models;
using EnsekAPI.Models;

namespace EnsekAPI.Tests.UnitTests
{
    [TestFixture]
    public class MeterReadingValidatorServiceTests
    {
        private MeterReadingService _meterReadingService;
        private Mock<IValidatorService> _mockValidatorService;
        private Mock<IAccountValidatorService> _mockAccountValidatorService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<EnsekDbContext>()
            .UseInMemoryDatabase(databaseName: "Ensek")
            .Options;
            
            var context = new EnsekDbContext(options);
            context.MeterReadings.Add(new MeterReading { AccountId = 1, DateTime = DateTime.Now, Value = 100 });
            context.MeterReadings.Add(new MeterReading { AccountId = 2, DateTime = DateTime.Now, Value = 200 });
            context.SaveChanges();

            _mockValidatorService = new Mock<IValidatorService>();
            _mockAccountValidatorService = new Mock<IAccountValidatorService>();

            _meterReadingService = new MeterReadingService(
                context,
                _mockValidatorService.Object,
                _mockAccountValidatorService.Object
            );
        }

        #region Positive
        [Test]
        public async Task ValidateUpload_LineIsValid_ReturnsTrue()
        {
            // Arrange
            Mock<IFormFile> fileMock = CreateFileMock();

            SetupValidateDateTime(true, DateTime.Now);
            SetupValidateReading(true, 100);
            SetupValidateAccount(true, 2);

            // Act
            var result = await _meterReadingService.ValidateUpload(fileMock.Object);

            // Assert
            AssertResult(result, 1, true, null);

        }

        #endregion

        #region Negative
        [Test]
        public async Task ValidateUpload_CsvLineLongerThanExpected_ReturnsFalseWithError()
        {
            // Arrange
            Mock<IFormFile> fileMock = CreateFileMock("1,01/01/2024 12:00,100,100");

            SetupValidateDateTime(true, DateTime.Now);
            SetupValidateReading(true, 100);
            SetupValidateAccount(true, 1);

            // Act
            var result = await _meterReadingService.ValidateUpload(fileMock.Object);

            // Assert
            AssertResult(result, 1, false, "Invalid format. Too many values entered. ");

        }

        [Test]
        public async Task ValidateUpload_AccountIdIsInvalid_ReturnsFalseWithError()
        {
            // Arrange
            Mock<IFormFile> fileMock = CreateFileMock();

            SetupValidateDateTime(true, DateTime.Now);
            SetupValidateReading(true, 100);
            SetupValidateAccount(false, 0);

            // Act
            var result = await _meterReadingService.ValidateUpload(fileMock.Object);

            // Assert
            AssertResult(result, 1, false, "Invalid Account ID. Must be a whole number and account must exist.");
        }

        [Test]
        public async Task ValidateUpload_DateTimeIsInvalid_ReturnsFalseWithError()
        {
            // Arrange
            Mock<IFormFile> fileMock = CreateFileMock();

            SetupValidateDateTime(false, DateTime.Now);
            SetupValidateReading(true, 100);
            SetupValidateAccount(true, 1);

            // Act
            var result = await _meterReadingService.ValidateUpload(fileMock.Object);

            // Assert
            AssertResult(result, 1, false, "Invalid Meter Reading Date Time. Must be in the format DD/MM/YYYY HH:MM. ");
        }

        [Test]
        public async Task ValidateUpload_MeterReadingIsInvalid_ReturnsFalseWithError()
        {
            // Arrange
            Mock<IFormFile> fileMock = CreateFileMock();

            SetupValidateDateTime(true, DateTime.Now);
            SetupValidateReading(false, 0);
            SetupValidateAccount(true, 1);

            // Act
            var result = await _meterReadingService.ValidateUpload(fileMock.Object);

            // Assert
            AssertResult(result, 1, false, $"Invalid Meter Read Value. Must be a whole number and greater than {Constants.General.MinMeterReading}. ");
        }

        [Test]
        public async Task ValidateUpload_MoreRecentReadingExistsForAccount_ReturnsFalseWithError()
        {
            // Arrange
            Mock<IFormFile> fileMock = CreateFileMock();

            SetupValidateDateTime(true, DateTime.Now.AddDays(-1));
            SetupValidateReading(true, 100);
            SetupValidateAccount(true, 1);

            // Act
            var result = await _meterReadingService.ValidateUpload(fileMock.Object);

            // Assert
            AssertResult(result, 1, false, "More recent reading already exists. ");
        }

        [Test]
        public async Task ValidateUpload_ReadingExistsForAccount_ReturnsFalseWithError()
        {
            // Arrange
            Mock<IFormFile> fileMock = CreateFileMock();

            SetupValidateDateTime(true, DateTime.Now);
            SetupValidateReading(true, 200);
            SetupValidateAccount(true, 2);

            // Act
            var result = await _meterReadingService.ValidateUpload(fileMock.Object);

            // Assert
            AssertResult(result, 1, false, "Reading already exists. ");
        }
        #endregion

        #region Helpers
        private Mock<IFormFile> CreateFileMock(string line = "")
        {
            var fileMock = new Mock<IFormFile>();
            var csvData = new StringBuilder();
            csvData.AppendLine("AccountID,ReadingDate,ReadingValue");
            csvData.AppendLine(!string.IsNullOrEmpty(line) ? line : "1,01/01/2024 12:00,100");

            var csvBytes = Encoding.UTF8.GetBytes(csvData.ToString());
            var stream = new MemoryStream(csvBytes);
            fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);
            return fileMock;
        }

        private void SetupValidateAccount(bool valid, int returnedAccountId)
        {
            _mockAccountValidatorService.Setup(a => a.ValidateAccount(It.IsAny<string>())).Returns(new Tuple<bool, int>(valid, returnedAccountId));
        }

        private void SetupValidateReading(bool valid, int returnedInteger)
        {
            _mockValidatorService.Setup(v => v.ValidateInt(It.IsAny<string>(), It.IsAny<int>())).Returns(new Tuple<bool, int>(valid, returnedInteger));
        }

        private void SetupValidateDateTime(bool valid, DateTime returnedDateTime)
        {
            _mockValidatorService.Setup(v => v.ValidateDateTime(It.IsAny<string>())).Returns(new Tuple<bool, DateTime>(valid, returnedDateTime));
        }

        private void AssertResult(List<MeterReadingValidationResult> result, int count, bool isValid, string error)
        {
            Assert.That(result.Count() == count);

            var validationResult = result.First();
            Assert.That(validationResult.Valid == isValid);
            Assert.That(validationResult.Error == error);
        }
        #endregion
    }
}