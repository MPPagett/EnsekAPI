using NUnit.Framework;
using EnsekAPI.Services;
using System.Globalization;

namespace EnsekAPI.Tests.UnitTests
{
    [TestFixture]
    public class ValidatorServiceTests
    {
        private ValidatorService _validatorService;

        [SetUp]
        public void SetUp()
        {
            _validatorService = new ValidatorService();
        }

        #region Positive
        [Test]
        public void ValidateInt_IsInteger_ReturnsTrue()
        {
            // Arrange
            // Act
            var result = _validatorService.ValidateInt("1");

            // Assert
            Assert.That(result.Item1);
            Assert.That(result.Item2 == 1);
        }

        public void ValidateInt_IsIntegerAndAboveMin_ReturnsTrue()
        {
            // Arrange
            // Act
            var result = _validatorService.ValidateInt("1", 0);

            // Assert
            Assert.That(result.Item1);
            Assert.That(result.Item2 == 1);
        }

        [Test]
        public void ValidateDateTime_IsDateTimeInCorrectFormat_ReturnsTrue()
        {
            // Arrange
            var dateTimeString = "22/04/2019 12:25";
            DateTime.TryParseExact(dateTimeString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);

            // Act
            var result = _validatorService.ValidateDateTime(dateTimeString);

            // Assert
            Assert.That(result.Item1);
            Assert.That(result.Item2 == dateTime);
        }

        #endregion

        #region Negative
        public void ValidateInt_IsIntegerButBelowMin_ReturnsFalse()
        {
            // Arrange
            // Act
            var result = _validatorService.ValidateInt("1", 5);

            // Assert
            Assert.That(result.Item1 == false);
            Assert.That(result.Item2 == 1);
        }

        [Test]
        [TestCase("xyz")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("!$%")]

        public void ValidateInt_InputInvalid_ReturnsFalse(string input)
        {
            // Arrange
            // Act
            var result = _validatorService.ValidateInt(input);

            // Assert
            Assert.That(!result.Item1);
            Assert.That(result.Item2 == 0);
        }

        [Test]
        [TestCase("12/12/2024 55:23")]
        [TestCase("12/12/202 14:23")]
        [TestCase("12/12/2024")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("!$%")]

        public void ValidateDateTime_InputInvalid_ReturnsFalse(string input)
        {
            // Arrange
            // Act
            var result = _validatorService.ValidateDateTime(input);

            // Assert
            Assert.That(!result.Item1);
        }
        #endregion
    }
}