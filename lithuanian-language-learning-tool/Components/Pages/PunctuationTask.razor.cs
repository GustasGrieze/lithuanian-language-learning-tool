using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using lithuanian_language_learning_tool.Helpers;

namespace lithuanian_language_learning_tool.Components.Pages
{
    public class PunctuationTaskBase : ComponentBase
    {
        protected Timer timer = new Timer();
        protected List<CustomTask> tasks = new List<CustomTask>
        {
            new CustomTask
            {
                Sentence = "Vilnius Lietuvos sostinė yra vienas seniausių Europos miestų...",
                Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                CorrectAnswer = "Vilnius, Lietuvos sostinė, yra vienas seniausių Europos miestų...",
                Explanation = "Kablelis naudojamas atskirti miestą ir aprašą."
            },
            new CustomTask
            {
                Sentence = "Petriukas surado piniginę kuri neturėjo jokių pinigų.",
                Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                CorrectAnswer = "Petriukas surado piniginę, kuri neturėjo jokių pinigų.",
                Explanation = "Kablelis čia būtinas prieš jungtuką „kuri“."
            },
            new CustomTask
            {
                Sentence = "Išeidamas sutikau labai malonų žmogų kuris turėjo žaizdą ant veido.",
                Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                CorrectAnswer = "Išeidamas sutikau labai malonų žmogų, kuris turėjo žaizdą ant veido.",
                Explanation = "Čia būtinas kablelis prieš jungtuką „kuris“."
            },
            new CustomTask
            {
                Sentence = "Sakinys kalbinis vienetas sudarytas iš vieno ar daugiau žodžių.",
                Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                CorrectAnswer = "Sakinys - kalbinis vienetas, sudarytas iš vieno ar daugiau žodžių.",
                Explanation = "Brūkšnys naudojamas pabrėžti sakinį apibūdinantį elementą."
            }
        };

        protected int currentTaskIndex = 0;
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
            tasks[currentTaskIndex].UserText = tasks[currentTaskIndex].Sentence;
            tasks[currentTaskIndex].InitializeHighlights();
            taskStatus = Enumerable.Repeat(false, tasks.Count).ToList();
        }

        protected void CheckPunctuation()
        {
            if (ComparePunctuationWithOriginal(tasks[currentTaskIndex].UserText, tasks[currentTaskIndex].CorrectAnswer))
            {
                feedbackMessage = "Puiku! Visi skyrybos ženklai teisingi.";
                feedbackClass = "correct";
                isCorrect = true;
                correctAnswersCount++;
                taskStatus[currentTaskIndex] = true;

                score += tasks[currentTaskIndex].CalculateScore(isCorrect, multiplier: 2); // simple scoring system - needs improvement (time based score, punctuation marks count)
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
            bool isCorrect = ((userText.Length == correctText.Length) && (
                 correctText
                     .Select((ch, i) => new { Char = ch, Index = i })
                     .Where(x => char.IsPunctuation(x.Char))
                     .All(x => userText[x.Index] == x.Char)));

            return isCorrect;
        }

        protected void NextTask()
        {
            if (currentTaskIndex < tasks.Count - 1)
            {
                currentTaskIndex++;
                tasks[currentTaskIndex].UserText = tasks[currentTaskIndex].Sentence;
                tasks[currentTaskIndex].InitializeHighlights();
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
            foreach (var task in tasks)
            {
                task.UserText = task.Sentence;
                task.InitializeHighlights();
            }
        }

        protected void GoToTask(int taskIndex = 0)
        {
            if (taskIndex >= 0 && taskIndex < tasks.Count)
            {
                currentTaskIndex = taskIndex;
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
            reviewMode = false;
            tasks[currentTaskIndex].UserText = tasks[currentTaskIndex].Sentence;
            tasks[currentTaskIndex].InitializeHighlights();
            taskStatus = Enumerable.Repeat(false, tasks.Count).ToList();
            RestartTasks();
        }

        private List<CustomTask> ParseUploadedTasks(string fileContent)
        {
            try
            {
                List<CustomTask> uploadedTasks = JsonSerializer.Deserialize<List<CustomTask>>(fileContent);

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
                return new List<CustomTask>
                {
                    new CustomTask
                    {
                        Sentence = "Vilnius Lietuvos sostinė yra vienas seniausių Europos miestų...",
                        Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                        CorrectAnswer = "Vilnius, Lietuvos sostinė, yra vienas seniausių Europos miestų...",
                        Explanation = "Kablelis naudojamas atskirti miestą ir aprašą."
                    },
                    new CustomTask
                    {
                        Sentence = "Petriukas surado piniginę kuri neturėjo jokių pinigų.",
                        Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                        CorrectAnswer = "Petriukas surado piniginę, kuri neturėjo jokių pinigų.",
                        Explanation = "Kablelis čia būtinas prieš jungtuką „kuri“."
                    },
                    new CustomTask
                    {
                        Sentence = "Išeidamas sutikau labai malonų žmogų kuris turėjo žaizdą ant veido.",
                        Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                        CorrectAnswer = "Išeidamas sutikau labai malonų žmogų, kuris turėjo žaizdą ant veido.",
                        Explanation = "Čia būtinas kablelis prieš jungtuką „kuris“."
                    },
                    new CustomTask
                    {
                        Sentence = "Sakinys kalbinis vienetas sudarytas iš vieno ar daugiau žodžių.",
                        Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                        CorrectAnswer = "Sakinys - kalbinis vienetas, sudarytas iš vieno ar daugiau žodžių.",
                        Explanation = "Brūkšnys naudojamas pabrėžti sakinį apibūdinantį elementą."
                    }
                };
            }
        }

        protected void ToggleHighlight(int spaceIndex)
        {
            if (tasks[currentTaskIndex].Highlights == null)
                return;

            foreach (var highlight in tasks[currentTaskIndex].Highlights)
            {
                highlight.IsSelected = highlight.SpaceIndex == spaceIndex;
            }
        }

        protected void InsertPunctuation(string punctuation)
        {
            var currentTask = tasks[currentTaskIndex];

            if (string.IsNullOrEmpty(currentTask.UserText))
            {
                currentTask.UserText = currentTask.Sentence;
            }

            var selectedHighlight = currentTask.Highlights.FirstOrDefault(h => h.IsSelected);
            if (selectedHighlight != null)
            {
                int insertionIndex = selectedHighlight.SpaceIndex;

                // Check if there's already a punctuation before the space :(
                if (insertionIndex > 0 && char.IsPunctuation(currentTask.UserText[insertionIndex - 1]))
                {
                    currentTask.UserText = currentTask.UserText.Remove(insertionIndex - 1, 1);

                    currentTask.UserText = currentTask.UserText.Insert(insertionIndex - 1, punctuation);

                    selectedHighlight.IsSelected = false;
                }
                else
                {
                    // Add the punctuation normally :)
                   
                    currentTask.UserText = currentTask.UserText.Insert(insertionIndex, punctuation);

                   
                    int punctuationLength = punctuation.Length;
                    for (int i = 0; i < currentTask.Highlights.Count; i++)
                    {
                        var highlight = currentTask.Highlights[i];
                        if (highlight.SpaceIndex >= insertionIndex)
                        {
                            highlight.SpaceIndex += punctuationLength;
                            currentTask.Highlights[i] = highlight;
                        }
                    }

                   
                    selectedHighlight.HasPunctuation = true;
                    selectedHighlight.IsSelected = false;

                }

                StateHasChanged();
            }
        }

    }
}
