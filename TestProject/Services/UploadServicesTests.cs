// File: UploadServiceTests.cs
using lithuanian_language_learning_tool.Exceptions;
using lithuanian_language_learning_tool.Models;
using lithuanian_language_learning_tool.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace TestProject.Services.Tests
{
    public class UploadServiceTests
    {
        private readonly Mock<ILogger<UploadService>> _loggerMock;
        private readonly Mock<ITaskService<PunctuationTask>> _punctuationTaskServiceMock;
        private readonly Mock<ITaskService<SpellingTask>> _spellingTaskServiceMock;
        private readonly UploadService _uploadService;

        public UploadServiceTests()
        {
            _loggerMock = new Mock<ILogger<UploadService>>();
            _punctuationTaskServiceMock = new Mock<ITaskService<PunctuationTask>>();
            _spellingTaskServiceMock = new Mock<ITaskService<SpellingTask>>();

            _uploadService = new UploadService(
                _loggerMock.Object,
                _punctuationTaskServiceMock.Object,
                _spellingTaskServiceMock.Object);
        }

        #region ValidateAndUploadAsync Tests

        [Theory]
        [InlineData("punctuation", "[{\"Sentence\":\"Test sentence.\",\"Options\":[\".\",\",\"],\"CorrectAnswer\":\".\",\"Explanation\":\"End punctuation.\"}]")]
        [InlineData("spelling", "[{\"Sentence\":\"Test sentence\",\"Options\":[\"a\",\"b\"],\"CorrectAnswer\":\"a\",\"Explanation\":\"Correct letter.\"}]")]
        public async Task ValidateAndUploadAsync_ValidJson_DoesNotThrow(string taskType, string jsonContent)
        {
            // Arrange
            if (taskType.Equals("punctuation", StringComparison.OrdinalIgnoreCase))
            {
                _punctuationTaskServiceMock
                    .Setup(s => s.AddTaskAsync(It.IsAny<PunctuationTask>()))
                    .Returns(Task.CompletedTask);
            }
            else if (taskType.Equals("spelling", StringComparison.OrdinalIgnoreCase))
            {
                _spellingTaskServiceMock
                    .Setup(s => s.AddTaskAsync(It.IsAny<SpellingTask>()))
                    .Returns(Task.CompletedTask);
            }

            // Act
            var exception = await Record.ExceptionAsync(() => _uploadService.ValidateAndUploadAsync(jsonContent, taskType));

            // Assert
            Assert.Null(exception);

            // Verify that AddTaskAsync was called the correct number of times
            if (taskType.Equals("punctuation", StringComparison.OrdinalIgnoreCase))
            {
                var punctuationTasks = JsonSerializer.Deserialize<List<PunctuationTask>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                _punctuationTaskServiceMock.Verify(s => s.AddTaskAsync(It.IsAny<PunctuationTask>()), Times.Exactly(punctuationTasks.Count));
            }
            else if (taskType.Equals("spelling", StringComparison.OrdinalIgnoreCase))
            {
                var spellingTasks = JsonSerializer.Deserialize<List<SpellingTask>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                _spellingTaskServiceMock.Verify(s => s.AddTaskAsync(It.IsAny<SpellingTask>()), Times.Exactly(spellingTasks.Count));
            }
        }

        [Theory]
        [InlineData("punctuation", "[]", "Failas neturi užduočių arba yra tuščias.")]
        [InlineData("spelling", "[{\"Sentence\":\"Test sentence\",\"Options\":[],\"CorrectAnswer\":\"a\",\"Explanation\":\"Correct letter.\"}]", "Options laukelis yra tuščias arba netinkamas.")]
        [InlineData("punctuation", "[{\"Sentence\":\"Test sentence\",\"Options\":[\"a\",\"b\"],\"CorrectAnswer\":\"a\",\"Explanation\":\"Explanation.\"}]", "Netinkama Options struktūra: skyrybos užduotyse leidžiami tik skyrybos ženklai.")]
        [InlineData("spelling", "[{\"Sentence\":\"Test sentence\",\"Options\":[\".\",\",\"],\"CorrectAnswer\":\".\",\"Explanation\":\"Explanation.\"}]", "Netinkama Options struktūra: rašybos užduotyse leidžiamos tik raidės ir ilgis iki 3 simbolių.")]
        [InlineData("punctuation", "[{\"Sentence\":\"Test sentence\",\"Options\":[\".\",\"invalid\"],\"CorrectAnswer\":\".\",\"Explanation\":\"Explanation.\"}]", "Netinkama Options struktūra: skyrybos užduotyse leidžiami tik skyrybos ženklai.")]
        [InlineData("punctuation", "invalid json", "JSON failas yra netinkamai suformatuotas.")]
        public async Task ValidateAndUploadAsync_InvalidJson_ThrowsTaskUploadException(string taskType, string jsonContent, string expectedMessage)
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<TaskUploadException>(() => _uploadService.ValidateAndUploadAsync(jsonContent, taskType));
            Assert.Equal(expectedMessage, exception.Message);

            // Ensure that AddTaskAsync was never called due to validation failure
            _punctuationTaskServiceMock.Verify(s => s.AddTaskAsync(It.IsAny<PunctuationTask>()), Times.Never);
            _spellingTaskServiceMock.Verify(s => s.AddTaskAsync(It.IsAny<SpellingTask>()), Times.Never);
        }

        #endregion

        #region LogException Tests

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

        #endregion
    }
}
