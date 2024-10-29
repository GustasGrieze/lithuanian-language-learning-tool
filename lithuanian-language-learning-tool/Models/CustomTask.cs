using System.Threading.Tasks;

namespace lithuanian_language_learning_tool.Models
{
    public class CustomTask
    {
        public string Sentence { get; set; }  // The original sentence presented to the user
        public string UserText { get; set; }  // User's current version of the sentence
        public List<string> Options { get; set; }  // The list of punctuation or answer options
        public string CorrectAnswer { get; set; }  // The correct version of the sentence with correct punctuation
        public string Explanation { get; set; } = ""; // The explanation for why this punctuation is correct
        public bool TaskStatus { get; set; } = false; // incorrect by default
        public virtual void Reset()
        {
            UserText = Sentence;
            TaskStatus = TaskStatus = false;
        }

        public virtual int CalculateScore(bool isCorrect, int multiplier = 1)
        {
            return isCorrect ? 10 * multiplier : 0;
        }

    }
}