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
                CorrectAnswer = "u",
                Explanation = "Žodyje 'Lietuvos', raidė 'u' yra tinkama, nes tai yra kilmininko forma žodžio 'Lietuva'. Lietuvoje raidė 'ū' naudojama ilguose skiemenyse, tačiau šiame kontekste trumpasis 'u' yra taisyklingas."
            },
            new global::Task
            {
                Sentence = "Lietuvoje yra daug gra_ių ežerų.",
                Options = new List<string> { "ž", "š", "s", "z" },
                CorrectAnswer = "ž",
                Explanation = "Žodis 'gražių' reikalauja minkštosios 'ž', nes tai būdvardžio forma, susijusi su žodžiu 'gražus'."
            },
            new global::Task
            {
                Sentence = "Kaunas yra antras pag_l dydį Lietuvos miestas.",
                Options = new List<string> { "a", "ą", "e", "ę" },
                CorrectAnswer = "a",
                Explanation = "Žodis 'pagal' naudojamas kaip prielinksnis, ir šiame kontekste reikia raidės 'a'. Lietuvių kalboje kaip 'ą' ir 'ę' yra ilgesnės balsių formos, naudojamos kitais atvejais, bet ne 6iame prielinksnyje."
            }
        };

        protected int currentTaskIndex = 0;
        protected bool showFeedback = false;
        protected bool showNextButton = false;
        protected bool isCorrect = false;
        protected string feedbackMessage = "";
        protected int correctTotal = 0;

        protected List<bool> taskStatus = new List<bool>();
        protected string explanationMessage = "";
        protected string correctAnswer = "";
        protected  string userText = "";
        protected bool showSummary = false;

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
            if (currentTaskIndex < tasks.Count - 1)
            {
                currentTaskIndex++;
                userText = tasks[currentTaskIndex].Sentence;
                showFeedback = false;
                showNextButton = false;
            }
            else
            {
                showSummary = true;
            }
            
        }

        protected void RestartTasks()
        {

            currentTaskIndex = 0;
            showFeedback = false;
            showNextButton = false;
            isCorrect = false;
            feedbackMessage = "";
            correctTotal = 0;

            explanationMessage = "";
            correctAnswer = "";
            userText = "";
            showSummary = false;

            currentTaskIndex = 0;
            correctTotal = 0;
            userText = tasks[currentTaskIndex].Sentence;
            feedbackMessage = null;
            isCorrect = false;
            showSummary = false;
        }

        protected void GoToTask(int taskIndex)
        {
            if (taskIndex >= 0 && taskIndex < tasks.Count)
            {
                currentTaskIndex = taskIndex;
                userText = tasks[currentTaskIndex].Sentence;  
                feedbackMessage = null;  
                correctAnswer = $"Teisingas atsakymas: {tasks[currentTaskIndex].CorrectAnswer}";
                explanationMessage = $"Paaiškinimas: {tasks[currentTaskIndex].Explanation}";  
                showSummary = false;  
            }
            else
            {
                feedbackMessage = "Invalid task index.";
            }
        }
    }
}
