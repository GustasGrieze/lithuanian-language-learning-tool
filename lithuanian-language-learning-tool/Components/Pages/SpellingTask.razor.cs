using Microsoft.AspNetCore.Components;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;

namespace lithuanian_language_learning_tool.Components.Pages
{
    public class SpellingTaskBase : ComponentBase
    {
        protected Timer timer = new Timer();
        protected List<global::Task> tasks = new List<global::Task>
        {
            new global::Task
            {
                Sentence = "Vilnius yra Liet_vos sostinė.",
                Options = new List<string> { "u", "ū", "o", "uo" },
                CorrectAnswer = "u",
                Explanation = "Teisingas atsakymas yra 'u', nes žodyje 'Lietuvos' rašoma trumpa balsė 'u'. Šis žodis yra kilmininko forma, reiškianti 'Lietuva'."
            },
            new global::Task
            {
                Sentence = "Lietuvoje yra daug gra_ių ežerų.",
                Options = new List<string> { "ž", "š", "s", "z" },
                CorrectAnswer = "ž",
                Explanation = "Teisingas atsakymas yra 'ž', nes 'gražių' kyla iš gražus."
            },
            new global::Task
            {
                Sentence = "Kaunas yra antras pag_l dydį Lietuvos miestas.",
                Options = new List<string> { "a", "ą", "e", "ę" },
                CorrectAnswer = "a",
                Explanation = "Teisingas atsakymas yra 'a', nes 'pagal' yra tinkama prielinksnio forma."
            }

        };

        protected int currentTaskIndex = 0;
        protected bool showFeedback = false;
        protected bool showNextButton = false;
        protected bool isCorrect = false;
        protected string feedbackMessage = "";
        protected int correctTotal = 0;
        protected List<bool> taskStatus = new List<bool>();
        protected int score = 0;
		protected string explanationMessage = "";
        protected string correctAnswer = "";
        protected  string userText = "";
        protected bool showSummary = false;

        protected bool reviewMode = false;
        protected bool startExercise = false;

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
            score = isCorrect ? score + (100) : score; // simple scoring system - needs improvement (time based score)
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

            userText = tasks[currentTaskIndex].Sentence;
            score = 0;
            reviewMode = false;
        }
		
		protected void GoToTask(int taskIndex)
        {
            if (taskIndex >= 0 && taskIndex < tasks.Count)
            {
                currentTaskIndex = taskIndex;
                userText = tasks[currentTaskIndex].Sentence;  
                feedbackMessage = null;  
                correctAnswer = tasks[currentTaskIndex].CorrectAnswer;
                explanationMessage = tasks[currentTaskIndex].Explanation;  
                showSummary = false; 
                reviewMode=true;
            }
            else
            {
                feedbackMessage = "Invalid task index.";
            }
        }
         
        protected void TimerOut()
        {
            showSummary = true;
        }

        protected void LoadCustomTasks(string fileContent)
        {
            tasks = ParseUploadedTasks(fileContent);
            StartExercise();
        }

        protected void StartWithDefaultTasks()
        {
            // Maybe implement custom Logic to handle starting with default tasks
            StartExercise();
        }

        protected void StartExercise()
        {
            startExercise = true;
            userText = tasks[currentTaskIndex].Sentence;
            taskStatus = Enumerable.Repeat(false, tasks.Count).ToList();

            RestartTasks();
        }
        private List<global::Task> ParseUploadedTasks(string fileContent)
        {
            try
            {
                List<global::Task> uploadedTasks = JsonSerializer.Deserialize<List<global::Task>>(fileContent);

                if (uploadedTasks != null)
                {
                    return uploadedTasks;
                }
                else
                {
                    throw new Exception("Failed to parse tasks from the file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                StartWithDefaultTasks();
                return new List<global::Task>
                    {
                        new global::Task
                        {
                            Sentence = "Vilnius yra Liet_vos sostinė.",
                            Options = new List<string> { "u", "ū", "o", "uo" },
                            CorrectAnswer = "u",
                            Explanation = "Teisingas atsakymas yra 'u', nes žodyje 'Lietuvos' rašoma trumpa balsė 'u'. Šis žodis yra kilmininko forma, reiškianti 'Lietuva'."
                        },
                        new global::Task
                        {
                            Sentence = "Lietuvoje yra daug gra_ių ežerų.",
                            Options = new List<string> { "ž", "š", "s", "z" },
                            CorrectAnswer = "ž",
                            Explanation = "Teisingas atsakymas yra 'ž', nes 'gražių' kyla iš gražus."
                        },
                        new global::Task
                        {
                            Sentence = "Kaunas yra antras pag_l dydį Lietuvos miestas.",
                            Options = new List<string> { "a", "ą", "e", "ę" },
                            CorrectAnswer = "a",
                            Explanation = "Teisingas atsakymas yra 'a', nes 'pagal' yra tinkama prielinksnio forma."
                        }
                    };
            }
        }
    }
}