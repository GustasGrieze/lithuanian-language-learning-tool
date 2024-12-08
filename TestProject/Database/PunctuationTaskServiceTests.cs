// File: PunctuationTaskServiceTests.cs
using lithuanian_language_learning_tool.Models;
using lithuanian_language_learning_tool.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestProject.Database
{
    public class PunctuationTaskServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly ITaskService<PunctuationTask> _taskService;
        private readonly DatabaseFixture _fixture;

        public PunctuationTaskServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _taskService = _fixture.ServiceProvider.GetRequiredService<ITaskService<PunctuationTask>>();
        }

        [Fact]
        public async Task AddAndRetrievePunctuationTask()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var task = new PunctuationTask
            {
                Sentence = "Sveiki kaip sekasi",
                UserText = "Sveiki kaip sekasi",
                CorrectAnswer = "Sveiki, kaip sekasi?",
                Explanation = "A comma is needed after 'Sveiki' and a question mark at the end.",
                TaskStatus = false,
                Topic = "Basic Greetings"
            };
            var options = new List<string> { ",", ".", "!" };
            task.Options = options; // Set options via the task's Options property

            // Act
            await _taskService.AddTaskAsync(task); // Pass only the task
            var retrievedTask = await _taskService.GetTaskAsync(task.Id);

            // Assert
            Assert.NotNull(retrievedTask);
            Assert.Equal("Sveiki kaip sekasi", retrievedTask.UserText);
            Assert.Equal("Sveiki, kaip sekasi?", retrievedTask.CorrectAnswer);
            Assert.Equal(3, retrievedTask.AnswerOptions.Count);
            Assert.Contains(retrievedTask.AnswerOptions, o => o.OptionText == ",");
            Assert.Contains(retrievedTask.AnswerOptions, o => o.OptionText == ".");
            Assert.Contains(retrievedTask.AnswerOptions, o => o.OptionText == "!");
        }

        [Fact]
        public async Task UpdatePunctuationTaskOptions()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var task = new PunctuationTask
            {
                Sentence = "Kaip tau sekasi",
                UserText = "Kaip tau sekasi",
                CorrectAnswer = "Kaip tau sekasi?",
                Explanation = "A question mark is needed at the end of the sentence.",
                TaskStatus = false,
                Topic = "Basic Questions"
            };
            var initialOptions = new List<string> { "!", ".", "?" };
            task.Options = initialOptions; // Set initial options
            await _taskService.AddTaskAsync(task); // Pass only the task

            // Act
            var newOptions = new List<string> { "!!", "?", ":" };
            task.CorrectAnswer = "Kaip tau sekasi!";
            task.Options = newOptions; // Update options via the task's Options property
            await _taskService.UpdateTaskAsync(task); // Pass only the task
            var updatedTask = await _taskService.GetTaskAsync(task.Id);

            // Assert
            Assert.NotNull(updatedTask);
            Assert.Equal("Kaip tau sekasi!", updatedTask.CorrectAnswer);
            Assert.Equal(3, updatedTask.AnswerOptions.Count);
            Assert.Contains(updatedTask.AnswerOptions, o => o.OptionText == "!!");
            Assert.Contains(updatedTask.AnswerOptions, o => o.OptionText == "?");
            Assert.Contains(updatedTask.AnswerOptions, o => o.OptionText == ":");
            Assert.DoesNotContain(updatedTask.AnswerOptions, o => o.OptionText == "!");
            Assert.DoesNotContain(updatedTask.AnswerOptions, o => o.OptionText == ".");
        }

        [Fact]
        public async Task DeletePunctuationTask()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            var task = new PunctuationTask
            {
                Sentence = "Labas rytas",
                UserText = "Labas rytas",
                CorrectAnswer = "Labas rytas.",
                Explanation = "A period is needed at the end of the sentence.",
                TaskStatus = false,
                Topic = "Basic Greetings"
            };
            var options = new List<string> { ",", ".", "!" };
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
