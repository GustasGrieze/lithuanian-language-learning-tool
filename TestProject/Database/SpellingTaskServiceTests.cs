// File: SpellingTaskServiceTests.cs
using lithuanian_language_learning_tool.Models;
using lithuanian_language_learning_tool.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestProject.Database
{
    public class SpellingTaskServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly ITaskService<SpellingTask> _taskService;
        private readonly DatabaseFixture _fixture;

        public SpellingTaskServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _taskService = _fixture.ServiceProvider.GetRequiredService<ITaskService<SpellingTask>>();
        }

        [Fact]
        public async Task AddAndRetrieveSpellingTask()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var task = new SpellingTask
            {
                Sentence = "Rašyk teisingai",
                UserText = "Rašyk teisingai",
                CorrectAnswer = "Rašyk teisingai.",
                Explanation = "A period is needed at the end of the sentence.",
                TaskStatus = false,
                Topic = "Basic Spelling"
            };
            var options = new List<string> { ".", "!", "?" };
            task.Options = options; // Set options via the task's Options property

            // Act
            await _taskService.AddTaskAsync(task); // Pass only the task
            var retrievedTask = await _taskService.GetTaskAsync(task.Id);

            // Assert
            Assert.NotNull(retrievedTask);
            Assert.Equal("Rašyk teisingai", retrievedTask.UserText);
            Assert.Equal("Rašyk teisingai.", retrievedTask.CorrectAnswer);
            Assert.Equal(3, retrievedTask.AnswerOptions.Count);
            Assert.Contains(retrievedTask.AnswerOptions, o => o.OptionText == ".");
            Assert.Contains(retrievedTask.AnswerOptions, o => o.OptionText == "!");
            Assert.Contains(retrievedTask.AnswerOptions, o => o.OptionText == "?");
        }

        [Fact]
        public async Task UpdateSpellingTaskOptions()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var task = new SpellingTask
            {
                Sentence = "Taisyti klaidas",
                UserText = "Taisyti klaidas",
                CorrectAnswer = "Taisyti klaidas.",
                Explanation = "A period is needed at the end of the sentence.",
                TaskStatus = false,
                Topic = "Advanced Spelling"
            };
            var initialOptions = new List<string> { ",", ".", "!" };
            task.Options = initialOptions; // Set initial options
            await _taskService.AddTaskAsync(task); // Pass only the task

            // Act
            var newOptions = new List<string> { ":", "?" };
            task.CorrectAnswer = "Pasirinkumu pataisymas!";
            task.Options = newOptions; // Update options via the task's Options property
            await _taskService.UpdateTaskAsync(task); // Pass only the task
            var updatedTask = await _taskService.GetTaskAsync(task.Id);

            // Assert
            Assert.NotNull(updatedTask);
            Assert.Equal("Pasirinkumu pataisymas!", updatedTask.CorrectAnswer);
            Assert.Equal(2, updatedTask.AnswerOptions.Count);
            Assert.Contains(updatedTask.AnswerOptions, o => o.OptionText == ":");
            Assert.Contains(updatedTask.AnswerOptions, o => o.OptionText == "?");
            Assert.DoesNotContain(updatedTask.AnswerOptions, o => o.OptionText == ",");
            Assert.DoesNotContain(updatedTask.AnswerOptions, o => o.OptionText == ".");
            Assert.DoesNotContain(updatedTask.AnswerOptions, o => o.OptionText == "!");
        }

        [Fact]
        public async Task DeleteSpellingTask()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var task = new SpellingTask
            {
                Sentence = "Rašyk teisingai",
                UserText = "Rašyk teisingai",
                CorrectAnswer = "Rašyk teisingai.",
                Explanation = "A period is needed at the end of the sentence.",
                TaskStatus = false,
                Topic = "Basic Spelling"
            };
            var options = new List<string> { ".", "!", "?" };
            task.Options = options; // Set options via the task's Options property
            await _taskService.AddTaskAsync(task); // Pass only the task
            var taskId = task.Id;

            // Act
            await _taskService.DeleteTaskAsync(taskId);
            var deletedTask = await _taskService.GetTaskAsync(taskId);

            // Assert
            Assert.Null(deletedTask);
        }
    }
}
