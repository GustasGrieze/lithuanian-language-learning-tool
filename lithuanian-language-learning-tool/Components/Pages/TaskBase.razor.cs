using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using lithuanian_language_learning_tool.Helpers;
using lithuanian_language_learning_tool.Models;
using System.Threading.Tasks;
using lithuanian_language_learning_tool.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace lithuanian_language_learning_tool.Components.Pages
{
    public abstract class TaskBase<TTask> : ComponentBase where TTask : CustomTask, new()
    {
        [Inject]
        protected IUserService UserService { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }


        protected int score = 0;
        protected int correctAnswersCount = 0;
        protected Timer timer = new Timer();
        protected List<TTask> tasks = new List<TTask>();
        protected int currentTaskIndex = 0;
        protected TTask currentTask;

        
        
        
        //-----
        protected string? feedbackMessage;
        protected string feedbackClass = "";
        //-----


        protected bool reviewMode = false;
        protected bool startExercise = false;
        protected bool showFeedback = false;
        protected bool showSummary = false;


        protected bool showFlash = false;
        protected bool _lastAnswerCorrect = false;

        protected override void OnInitialized()
        {
            GenerateDefaultTasks();
        }

        protected void InitTasks()
        {
            foreach (var task in tasks)
            {
                task.UserText = task.Sentence;
            }
        }

        protected void LoadCustomTasks(string fileContent)
        {
            tasks = ParseUploadedTasks(fileContent);
            StartExercise();
        }
        protected List<TTask> ParseUploadedTasks(string fileContent)
        {
            var uploadedTasks = JsonSerializer.Deserialize<List<TTask>>(fileContent);
            if (uploadedTasks != null && uploadedTasks.Count > 0)
            {
                return uploadedTasks;
            }
            else
            {
                throw new Exception("Failed to parse tasks from the file.");
            }
        }

        /**
         * Be carful as this selectedAnswer arguement is only optional due to different implentation in Spelling and Punctuation but is actually necessary.
         */
        protected abstract bool IsAnswerCorrect(string selectedAnswer);
        protected async Task CheckAnswer(string selectedAnswer="")
        {
            showFeedback = true;
            currentTask.TaskStatus = IsAnswerCorrect(selectedAnswer);
            feedbackMessage = currentTask.TaskStatus
                ? "Teisingai!"
                : $"Neteisingai. Teisingas atsakymas: {tasks[currentTaskIndex].CorrectAnswer}";
            correctAnswersCount = currentTask.TaskStatus
                ? correctAnswersCount + 1
                : correctAnswersCount;
            currentTask.TaskStatus = currentTask.TaskStatus;

            _lastAnswerCorrect = currentTask.TaskStatus;
            showFlash = true;
            await Task.Delay(300);
            showFlash = false;

            score += currentTask.CalculateScore(currentTask.TaskStatus, multiplier: 2); // simple scoring system - needs improvement (time based score)
            StateHasChanged();

            await NextTask();
        }

        protected void StartWithDefaultTasks()
        {
            GenerateDefaultTasks();
            StartExercise();
        }

        protected abstract List<TTask> GenerateDefaultTasks();

        protected virtual void StartExercise()
        {
            score = 0;
            currentTaskIndex = 0;
            correctAnswersCount = 0;
            feedbackMessage = null;
            showSummary = false;
            showFeedback = false;
            reviewMode = false;
            startExercise = true;
            if (tasks.Count > 0)
            {
                currentTask = tasks[currentTaskIndex];
                currentTask.UserText = currentTask.Sentence;
                InitTasks();
            }
            RestartTasks();

        }

        protected void RestartTasks()
        {
            
            currentTask.TaskStatus = false;
            foreach (var task in tasks)
            {
                task.Reset();
            }
            StateHasChanged();
        }

        protected void GoToTask(int taskIndex = 0)
        {
            if (taskIndex >= 0 && taskIndex < tasks.Count)
            {
                currentTaskIndex = taskIndex;
                currentTask = tasks[currentTaskIndex];
                feedbackMessage = null;
                showSummary = false;
                reviewMode = true;
            }
            else
            {
                feedbackMessage = "Invalid task index.";
            }
        }

        protected async void  TimerOut()
        {
            await EndExercise();
        }
        protected async void SkipTask()
        {
            await NextTask();
        }

        protected virtual async Task NextTask()
        {
            if (currentTaskIndex < tasks.Count - 1)
            {
                timer.Dispose();
                timer.ResetTimer();

                currentTaskIndex++;
                currentTask = tasks[currentTaskIndex];
                feedbackMessage = null;
                showFeedback = false;
                currentTask.TaskStatus = false;

            }
            else
            {
                await EndExercise();
            }
        }
        protected virtual async Task EndExercise()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var currentUser = await UserService.GetCurrentUserAsync(authState);

            if(currentUser != null &&!currentUser.IsGuest)
                await UpdateUserHighScore(currentUser);
            showSummary = true;
        }

        protected async Task UpdateUserHighScore(User currentUser)
        {
            if (currentUser != null)
            {
                if (score > currentUser.HighScore)
                {
                    currentUser.HighScore = score;
                    await UserService.UpdateUserAsync(currentUser);
                }
            }
        }


    }
}
