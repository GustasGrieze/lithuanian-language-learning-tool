using lithuanian_language_learning_tool.Models;

namespace lithuanian_language_learning_tool.Components.Pages
{
    public partial class PunctuationTaskBase : TaskBase<PunctuationTask>
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (tasks.Count > 0)
            {
                foreach (var task in tasks)
                {
                    task.InitializeHighlights();
                }
            }
        }

        protected virtual async Task StartExercise(bool refetchNewTasks = false)
        {
            await base.StartExercise(refetchNewTasks);
            currentTask?.InitializeHighlights();

        }

        protected override async Task NextTask()
        {
            await base.NextTask();
            if (currentTaskIndex < tasks.Count)
            {
                currentTask.InitializeHighlights();
            }
        }
        protected override List<PunctuationTask> GenerateDefaultTasks()
        {
            return tasks = new List<PunctuationTask>
            {
                new PunctuationTask
                {
                    Sentence = "Vilnius Lietuvos sostinė yra vienas seniausių Europos miestų...",
                    Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                    CorrectAnswer = "Vilnius, Lietuvos sostinė, yra vienas seniausių Europos miestų...",
                    Explanation = "Kablelis naudojamas atskirti miestą ir aprašą."
                },
                new PunctuationTask
                {
                    Sentence = "Petriukas surado piniginę kuri neturėjo jokių pinigų.",
                    Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                    CorrectAnswer = "Petriukas surado piniginę, kuri neturėjo jokių pinigų.",
                    Explanation = "Kablelis čia būtinas prieš jungtuką „kuri“."
                },
                new PunctuationTask
                {
                    Sentence = "Išeidamas sutikau labai malonų žmogų kuris turėjo žaizdą ant veido.",
                    Options = new List<string> { ".", ",", ";", ":", "!", "?" },
                    CorrectAnswer = "Išeidamas sutikau labai malonų žmogų, kuris turėjo žaizdą ant veido.",
                    Explanation = "Čia būtinas kablelis prieš jungtuką „kuris“."
                },
                new PunctuationTask
                {
                    Sentence = "Sakinys kalbinis vienetas sudarytas iš vieno ar daugiau žodžių.",
                    Options = new List<string> { ".", ",", ";", ":", " -", "?" },
                    CorrectAnswer = "Sakinys - kalbinis vienetas, sudarytas iš vieno ar daugiau žodžių.",
                    Explanation = "Brūkšnys naudojamas pabrėžti sakinį apibūdinantį elementą."
                }

            };
        }

        protected void ToggleHighlight(int spaceIndex)
        {
            currentTask?.ToggleHighlight(spaceIndex);
            StateHasChanged();
        }


        protected void HandleInsertPunctuation(string punctuation)
        {
            if (currentTask == null) return;

            currentTask.InsertPunctuation(punctuation);
            StateHasChanged();
        }

        protected void HandleDeletePunctuation()
        {
            if (currentTask == null) return;

            currentTask.DeletePunctuation();
            StateHasChanged();
        }

        protected override bool IsAnswerCorrect(string selectedAnswer)
        {
            return currentTask?.IsAnswerCorrect() ?? false;
        }


        ///--->>> For testing
        protected internal void SetCurrentTask(PunctuationTask task)
        {
            currentTask = task;
        }

    }
}