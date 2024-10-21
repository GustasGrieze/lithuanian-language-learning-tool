using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace lithuanian_language_learning_tool.Components.Pages
{
    public class PunctuationTaskBase : ComponentBase
    {
        protected Timer timer = new Timer();
        protected List<global::Task> tasks = new List<global::Task>
        {
            new global::Task
            {
                Sentence = "Vilnius Lietuvos sostinė yra vienas seniausių Europos miestų...",
                Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                CorrectAnswer = "Vilnius, Lietuvos sostinė, yra vienas seniausių Europos miestų...",
                Explanation = "Kablelis naudojamas atskirti miestą ir aprašą."
            },
            new global::Task
            {
                Sentence = "Petriukas surado piniginę kuri neturėjo jokių pinigų.",
                Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                CorrectAnswer = "Petriukas surado piniginę, kuri neturėjo jokių pinigų.",
                Explanation = "Kablelis čia būtinas prieš jungtuką „kuri“."
            },
            new global::Task
            {
                Sentence = "Išeidamas sutikau labai malonų žmogų kuris turėjo žaizdą ant veido.",
                Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                CorrectAnswer = "Išeidamas sutikau labai malonų žmogų, kuris turėjo žaizdą ant veido.",
                Explanation = "Čia būtinas kablelis prieš jungtuką „kuris“."
            },
            new global::Task
            {
                Sentence = "Sakinys kalbinis vienetas sudarytas iš vieno ar daugiau žodžių.",
                Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                CorrectAnswer = "Sakinys - kalbinis vienetas, sudarytas iš vieno ar daugiau žodžių.",
                Explanation = "Brūkšnys naudojamas pabrėžti sakinį apibūdinantį elementą."
            }
        };

        protected int currentTaskIndex = 0;
        protected string? userText;
        protected string? feedbackMessage;
        protected string feedbackClass = "";
        protected bool isCorrect = false;
        protected int correctAnswersCount = 0;
        protected bool showSummary = false;
        protected int score = 0;

        protected List<bool> taskStatus = new List<bool>();
        protected string explanationMessage = "";
		protected string correctAnswer = "";

        protected bool reviewMode = false;
        protected bool startExercise = false;

        protected override void OnInitialized()
        {
            userText = tasks[currentTaskIndex].Sentence;
            taskStatus = Enumerable.Repeat(false, tasks.Count).ToList();
        }

        protected void CheckPunctuation()
        {
            if (ComparePunctuationWithOriginal(userText, tasks[currentTaskIndex].CorrectAnswer))
            {
                feedbackMessage = "Puiku! Visi skyrybos ženklai teisingi.";
                feedbackClass = "correct";
                isCorrect = true;
                correctAnswersCount++;
                taskStatus[currentTaskIndex] = true;
                score = isCorrect ? score + (100) : score; // simple scoring system - needs improvement (time based score, punctuation marks count)
            }
            else
            {
                feedbackMessage = "Neteisingai! Bandykite dar kartą.";
                feedbackClass = "incorrect";
                isCorrect = false;
                taskStatus[currentTaskIndex] = false;
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
            NextTask();
        }

        protected void RestartTasks()
        {
            currentTaskIndex = 0;
            correctAnswersCount = 0;
            feedbackMessage = null;
            isCorrect = false;
            showSummary = false;
            score = 0;
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
                reviewMode = true;
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
                            Sentence = "Vilnius Lietuvos sostinė yra vienas seniausių Europos miestų...",
                            Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                            CorrectAnswer = "Vilnius, Lietuvos sostinė, yra vienas seniausių Europos miestų...",
                            Explanation = "Kablelis naudojamas atskirti miestą ir aprašą."
                        },
                        new global::Task
                        {
                            Sentence = "Petriukas surado piniginę kuri neturėjo jokių pinigų.",
                            Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                            CorrectAnswer = "Petriukas surado piniginę, kuri neturėjo jokių pinigų.",
                            Explanation = "Kablelis čia būtinas prieš jungtuką „kuri“."
                        },
                        new global::Task
                        {
                            Sentence = "Išeidamas sutikau labai malonų žmogų kuris turėjo žaizdą ant veido.",
                            Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                            CorrectAnswer = "Išeidamas sutikau labai malonų žmogų, kuris turėjo žaizdą ant veido.",
                            Explanation = "Čia būtinas kablelis prieš jungtuką „kuris“."
                        },
                        new global::Task
                        {
                            Sentence = "Sakinys kalbinis vienetas sudarytas iš vieno ar daugiau žodžių.",
                            Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                            CorrectAnswer = "Sakinys - kalbinis vienetas, sudarytas iš vieno ar daugiau žodžių.",
                            Explanation = "Brūkšnys naudojamas pabrėžti sakinį apibūdinantį elementą."
                        }
                    };
            }
        }
    }
}