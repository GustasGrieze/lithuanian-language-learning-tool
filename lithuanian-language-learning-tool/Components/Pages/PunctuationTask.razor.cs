using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Linq;
using Microsoft.JSInterop;

namespace lithuanian_language_learning_tool.Components.Pages
{
    public class PunctuationTaskBase: ComponentBase
    {
        [Inject]
        protected IJSRuntime JS { get; set; }
    

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
        protected List<string> userTextChars;
        protected string? draggedPunctuation;
        protected int gapPosition = -1;



        protected override void OnInitialized()
        {
            userText = tasks[currentTaskIndex].Sentence;
            taskStatus = Enumerable.Repeat(false, tasks.Count).ToList();
            userTextChars = userText.Select(c => c.ToString()).ToList();
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
            userText = tasks[currentTaskIndex].Sentence;
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
                correctAnswer = $"Teisingas atsakymas: {tasks[currentTaskIndex].CorrectAnswer}";
                explanationMessage = $"Paaiškinimas: {tasks[currentTaskIndex].Explanation}"; 
                showSummary = false;  
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

        protected override async System.Threading.Tasks.Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("dragAndDrop", ".draggable");
                await JS.InvokeVoidAsync("dropzoneHandler.initDraggable", "#draggable", "#dropzone");
            }

        }
        //-----
        private DotNetObjectReference<PunctuationTaskBase> objRef;

        /*protected override async System.Threading.Tasks.Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                objRef = DotNetObjectReference.Create(this);
            }
            await JS.InvokeVoidAsync("initializeDragAndDrop", objRef);
        }*/

        [JSInvokable]
        public void InsertPunctuation(int index, string punctuation)
        {
            userText = userText.Insert(index, punctuation);
            StateHasChanged();
        }

        public void Dispose()
        {
            objRef?.Dispose();
        }

        /*
        protected void OnDragStart(DragEventArgs e, string punctuation)
        {
            draggedPunctuation = punctuation;
            e.DataTransfer.EffectAllowed = "move";
            //e.DataTransfer.SetData("text", punctuation);
        }

        protected void OnDragOver(DragEventArgs e, int position)
        {
            //e.PreventDefault();
            gapPosition = position;
            StateHasChanged();
        }

        protected void OnDragLeave(DragEventArgs e, int position)
        {
            gapPosition = -1;
            StateHasChanged();
        }

        protected void OnDrop(DragEventArgs e, int position)
        {
            //e.PreventDefault();
            if (!string.IsNullOrEmpty(draggedPunctuation))
            {
                userTextChars.Insert(position, draggedPunctuation);
                userText = string.Concat(userTextChars);
                draggedPunctuation = null;
                gapPosition = -1;
                StateHasChanged();
            }
        }
        */

    }
}