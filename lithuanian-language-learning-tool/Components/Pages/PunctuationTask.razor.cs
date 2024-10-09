using Microsoft.AspNetCore.Components;
using System.Linq;

namespace lithuanian_language_learning_tool.Components.Pages
{
    public class PunctuationTaskBase : ComponentBase
    {
        protected List<global::Task> tasks = new List<global::Task>
        {
            new global::Task
            {
                Sentence = "Vilnius Lietuvos sostinė yra vienas seniausių Europos miestų...",
                Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                CorrectAnswer = "Vilnius, Lietuvos sostinė, yra vienas seniausių Europos miestų..."
            },
            new global::Task
            {
                Sentence = "Petriukas surado piniginę kuri neturėjo jokių pinigų.",
                Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                CorrectAnswer = "Petriukas surado piniginę, kuri neturėjo jokių pinigų."
            },
            new global::Task
            {
                Sentence = "Išeidamas sutikau labai malonų žmogų kuris turėjo žaizdą ant veido.",
                Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                CorrectAnswer = "Išeidamas sutikau labai malonų žmogų, kuris turėjo žaizdą ant veido."
            }
        };

        protected int currentTaskIndex = 0;
        protected string? userText;
        protected string? feedbackMessage;
        protected string feedbackClass = "";
        protected bool isCorrect = false;
        protected int correctAnswersCount = 0;
        protected bool showSummary = false;

        protected override void OnInitialized()
        {
            userText = tasks[currentTaskIndex].Sentence;
        }

        protected void CheckPunctuation()
        {
            if (ComparePunctuationWithOriginal(userText, tasks[currentTaskIndex].CorrectAnswer))
            {
                feedbackMessage = "Puiku! Visi skyrybos ženklai teisingi.";
                feedbackClass = "correct";
                isCorrect = true;
                correctAnswersCount++;
            }
            else
            {
                feedbackMessage = "Neteisingai! Bandykite dar kartą.";
                feedbackClass = "incorrect";
                isCorrect = false;
            }
        }

        protected bool ComparePunctuationWithOriginal(string userText, string correctText)
        {
            if (userText.Length != correctText.Length)
            {
                return false;
            }

            for (int i = 0; i < correctText.Length; i++)
            {
                if (char.IsPunctuation(correctText[i]))
                {
                    if (userText[i] != correctText[i])
                    {
                        return false; 
                    }
                }
            }
            return true;
        }

        protected void NextTask()
        {
            if (currentTaskIndex < tasks.Count - 1)
            {
                currentTaskIndex++;
                userText = tasks[currentTaskIndex].Sentence;
                feedbackMessage = null;
                isCorrect = false;
            }
            else
            {
                showSummary = true;
            }
        }

        protected void SkipTask()
        {
            if (currentTaskIndex < tasks.Count - 1)
            {
                currentTaskIndex++;
                userText = tasks[currentTaskIndex].Sentence;
                feedbackMessage = null;
                isCorrect = false;
            }
            else
            {
                showSummary = true;
            }
        }

        protected void RestartTasks()
        {
            currentTaskIndex = 0;
            correctAnswersCount = 0;
            userText = tasks[currentTaskIndex].Sentence;
            feedbackMessage = null;
            isCorrect = false;
            showSummary = false;
        }
    }
}
