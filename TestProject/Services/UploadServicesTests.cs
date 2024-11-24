// File: UploadServiceTests.cs
using lithuanian_language_learning_tool.Exceptions;
using lithuanian_language_learning_tool.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace TestProject.Services.Tests
{
    public class UploadServiceTests
    {
        private readonly Mock<ILogger<UploadService>> _loggerMock;
        private readonly UploadService _uploadService;

        public UploadServiceTests()
        {
            _loggerMock = new Mock<ILogger<UploadService>>();
            _uploadService = new UploadService(_loggerMock.Object);
        }

        [Theory]
        [InlineData("punctuation", "[{\"Sentence\":\"Test sentence.\",\"Options\":[\".\",\",\"],\"CorrectAnswer\":\".\",\"Explanation\":\"End punctuation.\"}]")]
        [InlineData("spelling", "[{\"Sentence\":\"Test sentence\",\"Options\":[\"a\",\"b\"],\"CorrectAnswer\":\"a\",\"Explanation\":\"Correct letter.\"}]")]
        public void ValidateJsonStructure_ValidJson_DoesNotThrow(string taskType, string jsonContent)
        {
            // Act & Assert
            _uploadService.ValidateJsonStructure(jsonContent, taskType);
        }

        [Theory]
        [InlineData("punctuation", "[]", "Failas neturi užduočių arba yra tuščias.")]
        [InlineData("spelling", "[{\"Sentence\":\"Test sentence\",\"Options\":[],\"CorrectAnswer\":\"a\",\"Explanation\":\"Correct letter.\"}]", "Options laukelis yra tuščias arba netinkamas.")]
        [InlineData("punctuation", "[{\"Sentence\":\"Test sentence\",\"Options\":[\"a\",\"b\"],\"CorrectAnswer\":\"a\",\"Explanation\":\"Explanation.\"}]", "Netinkama Options struktūra: skyrybos užduotyse leidžiami tik skyrybos ženklai.")]
        [InlineData("spelling", "[{\"Sentence\":\"Test sentence\",\"Options\":[\".\",\",\"],\"CorrectAnswer\":\".\",\"Explanation\":\"Explanation.\"}]", "Netinkama Options struktūra: rašybos užduotyse leidžiamos tik raidės.")]
        [InlineData("punctuation", "[{\"Sentence\":\"Test sentence\",\"Options\":[\".\",\"invalid\"],\"CorrectAnswer\":\".\",\"Explanation\":\"Explanation.\"}]", "Netinkama Options struktūra: skyrybos užduotyse leidžiami tik skyrybos ženklai.")]
        [InlineData("punctuation", "invalid json", "JSON failas yra netinkamai suformatuotas.")]
        public void ValidateJsonStructure_InvalidJson_ThrowsTaskUploadException(string taskType, string jsonContent, string expectedMessage)
        {
            // Act & Assert
            var exception = Assert.Throws<TaskUploadException>(() => _uploadService.ValidateJsonStructure(jsonContent, taskType));
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void LogException_LogsError()
        {
            // Arrange
            var exception = new Exception("Test exception");

            // Act
            _uploadService.LogException(exception);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Klaida įkeliant užduotį.")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
