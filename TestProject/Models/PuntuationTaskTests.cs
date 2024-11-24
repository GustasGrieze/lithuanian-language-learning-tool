using lithuanian_language_learning_tool.Models;
using Xunit;
namespace TestProject.Models
{
    public class PuntuationTaskTests
    {
        [Fact]
        public void InitializeHighlights_ShouldIdentifySpacesInSentence()
        {
            // Arrange
            var task = new PunctuationTask
            {
                Sentence = "This is a test sentence."
            };

            // Act
            task.InitializeHighlights();

            // Assert
            Assert.Equal(4, task.Highlights.Count); // There are 4 spaces
            Assert.Equal(4, task.Highlights[0].SpaceIndex); // Index of the first space
            Assert.Equal(7, task.Highlights[1].SpaceIndex); // Index of the second space
            Assert.Equal(9, task.Highlights[2].SpaceIndex); // Index of the third space
            Assert.Equal(14, task.Highlights[3].SpaceIndex); // Index of the fourth space
        }

        [Fact]
        public void InitializeHighlights_ShouldHandleSentencesWithoutSpaces()
        {
            // Arrange
            var task = new PunctuationTask
            {
                Sentence = "NoSpacesHere"
            };

            // Act
            task.InitializeHighlights();

            // Assert
            Assert.Empty(task.Highlights);
        }

        [Theory]
        [InlineData("Test sentence with spaces.", 3)]
        [InlineData("Only one space.", 2)] 
        [InlineData("", 0)]
        public void InitializeHighlights_ShouldClearExistingHighlights(string sentence, int expectedHighlightCount)
        {
            // Arrange
            var task = new PunctuationTask
            {
                Sentence = sentence
            };
            task.Highlights.Add(new PunctuationTask.Highlight(0));

            // Act
            task.InitializeHighlights();

            // Assert
            Assert.Equal(expectedHighlightCount, task.Highlights.Count); 
        }


    }
}
