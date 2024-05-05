using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Xunit;

namespace UnitTests
{
    [Fact]
    public async Task ValidateUpload_ValidFile_ReturnsExpectedResults()
    {
        // Arrange
        var dbContextMock = new Mock<EnsekDbContext>();
        var service = new MeterReadingService(dbContextMock.Object);

        var fileContent = "1001,01/01/2022 12:00,100\n1002,02/01/2022 12:00,200";
        var fileMock = new Mock<IFormFile>();
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(fileContent);
        writer.Flush();
        ms.Position = 0;
        fileMock.Setup(x => x.OpenReadStream()).Returns(ms);

        dbContextMock.Setup(x => x.Accounts.Select(a => a.Id)).Returns(new List<int> { 1001, 1002 });
        dbContextMock.Setup(x => x.MeterReadings).Returns(new List<MeterReading>());

        // Act
        var validationResults = await service.ValidateUpload(fileMock.Object);

        // Assert
        Assert.Equal(2, validationResults.Count);
        Assert.All(validationResults, vr => Assert.True(vr.Valid));
        Assert.Collection(validationResults,
            vr => Assert.Equal(new MeterReading { AccountId = 1001, DateTime = new DateTime(2022, 1, 1, 12, 0, 0), Value = 100 }, vr.MeterReading),
            vr => Assert.Equal(new MeterReading { AccountId = 1002, DateTime = new DateTime(2022, 1, 2, 12, 0, 0), Value = 200 }, vr.MeterReading));
    }

    [Fact]
    public async Task ValidateUpload_InvalidFile_ReturnsExpectedResults()
    {
        // Arrange
        var dbContextMock = new Mock<EnsekDbContext>();
        var service = new MeterReadingService(dbContextMock.Object);

        var fileContent = "1001,01/01/2022 12:00"; // Missing meter read value
        var fileMock = new Mock<IFormFile>();
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(fileContent);
        writer.Flush();
        ms.Position = 0;
        fileMock.Setup(x => x.OpenReadStream()).Returns(ms);

        // Act
        var validationResults = await service.ValidateUpload(fileMock.Object);

        // Assert
        Assert.Single(validationResults);
        Assert.False(validationResults[0].Valid);
        Assert.Contains("Invalid format. Too many values entered", validationResults[0].Error);
        Assert.Contains("Invalid Meter Read Value. Must be a whole number", validationResults[0].Error);
    }
}
