using System.Collections.Generic;
using System.Text.Json;
using lithuanian_language_learning_tool.Models;
using Microsoft.AspNetCore.Components;

namespace lithuanian_language_learning_tool.Components.Pages
{
    public partial class SpellingTaskBase : TaskBase<SpellingTask>
    {

        protected override List<SpellingTask> GenerateDefaultTasks()
        {
            return tasks = new List<SpellingTask>
            {
                new SpellingTask
                {
                    Sentence = "Vilnius yra Liet_vos sostinė.",
                    Options = new List<string> { "u", "ū", "o", "uo" },
                    CorrectAnswer = "u",
                    Explanation = "Teisingas atsakymas yra 'u', nes žodyje 'Lietuvos' rašoma trumpa balsė 'u'. Šis žodis yra kilmininko forma, reiškianti 'Lietuva'."
                },
                new SpellingTask
                {
                    Sentence = "Lietuvoje yra daug gra_ių ežerų.",
                    Options = new List<string> { "ž", "š", "s", "z" },
                    CorrectAnswer = "ž",
                    Explanation = "Teisingas atsakymas yra 'ž', nes 'gražių' kyla iš gražus."
                },
                new SpellingTask
                {
                    Sentence = "Kaunas yra antras pag_l dydį Lietuvos miestas.",
                    Options = new List<string> { "a", "ą", "e", "ę" },
                    CorrectAnswer = "a",
                    Explanation = "Teisingas atsakymas yra 'a', nes 'pagal' yra tinkama prielinksnio forma."
                }
            };
        }
        protected override bool IsAnswerCorrect(string selectedAnswer)
        {
            return selectedAnswer == currentTask.CorrectAnswer;
        }
    }
}