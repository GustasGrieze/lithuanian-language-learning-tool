using System.Collections.Generic;
using System.Text.Json;
using lithuanian_language_learning_tool.Models;
using Microsoft.AspNetCore.Components;

namespace lithuanian_language_learning_tool.Components.Pages
{
    public partial class PunctuationTaskBase : TaskBase<PunctuationTask>
    {

        protected override void StartExercise()
        {
            base.StartExercise();
            currentTask.InitializeHighlights();
            
        }

        protected override void NextTask()
        {
            base.NextTask();
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
            if (currentTask?.Highlights == null) return;

            for (var i = 0; i < currentTask.Highlights.Count; i++)
            {
                var highlight = currentTask.Highlights[i];
                highlight.IsSelected = highlight.SpaceIndex == spaceIndex;
                currentTask.Highlights[i] = highlight;
            }

            StateHasChanged();
        }


        protected void InsertPunctuation(string punctuation)
        {
            if (currentTask == null) return;

            if (string.IsNullOrEmpty(currentTask.UserText))
            {
                currentTask.UserText = currentTask.Sentence;
            }

            int selectedIndex = currentTask.Highlights.FindIndex(h => h.IsSelected);
            if (selectedIndex != -1)
            {
                var selectedHighlight = currentTask.Highlights[selectedIndex];
                int insertionIndex = selectedHighlight.SpaceIndex;

                if (insertionIndex > 0 && char.IsPunctuation(currentTask.UserText[insertionIndex - 1]))
                {
                    currentTask.UserText = currentTask.UserText.Remove(insertionIndex - 1, 1)
                                                           .Insert(insertionIndex - 1, punctuation);
                    selectedHighlight.IsSelected = false;
                    currentTask.Highlights[selectedIndex] = selectedHighlight;
                }
                else
                {
                    currentTask.UserText = currentTask.UserText.Insert(insertionIndex, punctuation);
                    int punctuationLength = punctuation.Length;
                    for (var i = 0; i < currentTask.Highlights.Count; i++)
                    {
                        if (currentTask.Highlights[i].SpaceIndex >= insertionIndex)
                        {
                            var hl = currentTask.Highlights[i];
                            hl.SpaceIndex += punctuationLength;
                            currentTask.Highlights[i] = hl;
                        }
                    }
                    selectedHighlight.HasPunctuation = true;
                    selectedHighlight.IsSelected = false;
                    //currentTask.Highlights[selectedIndex] = selectedHighlight;
                }

               
            }
            StateHasChanged();
        }



        protected override bool IsAnswerCorrect(string selectedAnswer)
        {
            return currentTask.UserText.Length == currentTask.CorrectAnswer.Length &&
                   currentTask.CorrectAnswer
                       .Select((ch, i) => new { Char = ch, Index = i })
                       .Where(x => char.IsPunctuation(x.Char))
                       .All(x => currentTask.UserText[x.Index] == x.Char);
        }
    }
}