namespace lithuanian_language_learning_tool.Helpers
{
    public static class TaskExtensions
    {
        // Extension method to calculate the score based on task correctness
        public static int CalculateScore(this global::Task task, bool isCorrect, int multiplier = 1)
        {

            return isCorrect ? 100 * multiplier : 0;
        }
    }
}