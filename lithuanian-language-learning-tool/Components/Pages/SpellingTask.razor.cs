using Microsoft.AspNetCore.Components;

namespace lithuanian_language_learning_tool.Components.Pages
{
    public class SpellingTaskBase : ComponentBase
    {
        protected List<global::Task> tasks = new List<global::Task>
        {
            new global::Task
            {
                Sentence = "Vilnius yra Liet_vos sostinė.",
                Options = new List<string> { "u", "ū", "o", "uo" },
                CorrectAnswer = "u"
            },
            new global::Task
            {
                Sentence = "Lietuvoje yra daug gra_ių ežerų.",
                Options = new List<string> { "ž", "š", "s", "z" },
                CorrectAnswer = "ž"
            },
            new global::Task
            {
                Sentence = "Kaunas yra antras pag_l dydį Lietuvos miestas.",
                Options = new List<string> { "a", "ą", "e", "ę" },
                CorrectAnswer = "a"
            }
        };

        protected int currentTaskIndex = 0;
        protected bool showFeedback = false;
        protected bool showNextButton = false;
        protected bool isCorrect = false;
        protected string feedbackMessage = "";
        protected int correctTotal = 0;
        protected List<bool> taskStatus = new List<bool>();

        protected override void OnInitialized()
        {
            taskStatus = new List<bool>(tasks.Count);
            for (int i = 0; i < tasks.Count; i++)
            {
                taskStatus.Add(false);
            }
        }

        protected void CheckAnswer(string selectedAnswer)
        {
            showFeedback = true;
            showNextButton = true;
            isCorrect = selectedAnswer == tasks[currentTaskIndex].CorrectAnswer;
            feedbackMessage = isCorrect
                ? "Teisingai!"
                : $"Neteisingai. Teisingas atsakymas: {tasks[currentTaskIndex].CorrectAnswer}";
            correctTotal = isCorrect ? correctTotal + 1 : correctTotal;
            taskStatus[currentTaskIndex] = isCorrect;
        }

        protected void NextTask()
        {
            currentTaskIndex++;
            showFeedback = false;
            showNextButton = false;
        }

        protected void RestartTasks()
        {
            currentTaskIndex = 0;
            showFeedback = false;
            showNextButton = false;
            isCorrect = false;
            feedbackMessage = "";
            correctTotal = 0;
        }
    }
}
