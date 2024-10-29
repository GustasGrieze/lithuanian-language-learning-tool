using lithuanian_language_learning_tool.Models;
namespace lithuanian_language_learning_tool.Helpers
{
    public static class CustomTaskExtensions
    {
        // Extension method to calculate the score based on task correctness
        public static int CalculateScore(this CustomTask task, bool isCorrect, int multiplier = 1)
        {

            return isCorrect ? 100 * multiplier : 0;
        }
    }
}