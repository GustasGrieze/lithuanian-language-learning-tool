using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using Xunit;
using Microsoft.AspNetCore.Components;
using lithuanian_language_learning_tool.Models;
using lithuanian_language_learning_tool.Components;


namespace TestProject.Components
{

    public class TaskReviewTests : TestContext
    {
        [Fact]
        public void TaskReviewShouldRenderTaskContentCorrectly()
        {
            // Arrange
            var task = new CustomTask
            {
                UserText = "This is the user's answer.",
                CorrectAnswer = "Correct answer text.",
                Explanation = "Explanation for the answer."
            };

            // Act
            var component = RenderComponent<TaskReview>(parameters => parameters
                .Add(p => p.Task, task)
            );

            // Assert
            Assert.Contains("This is the user's answer.", component.Markup);
            Assert.Contains("Correct answer text.", component.Markup);
            Assert.Contains("Explanation for the answer.", component.Markup);
        }
    }
}

