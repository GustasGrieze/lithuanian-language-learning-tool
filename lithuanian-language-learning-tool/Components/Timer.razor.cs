using System.Timers;
using Microsoft.AspNetCore.Components;

namespace lithuanian_language_learning_tool.Components
{
    public partial class Timer : ComponentBase, IDisposable
    {
        [Parameter] public int SecondsToRun { get; set; }

        private System.Timers.Timer _timer = null!;
        private int _secondsToRun;

        protected string Time { get; set; } = "00:00";

        [Parameter] public EventCallback TimerOut { get; set; }

        protected override void OnInitialized()
        {
            _secondsToRun = (SecondsToRun);
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Start();

            Time = TimeSpan.FromSeconds(_secondsToRun).ToString(@"mm\:ss");
        }

        private async void OnTimedEvent(object? sender, ElapsedEventArgs e)
        {
            _secondsToRun--;

            await InvokeAsync(() =>
            {
                Time = TimeSpan.FromSeconds(_secondsToRun).ToString(@"mm\:ss");
                StateHasChanged();
            });

            if (_secondsToRun <= 0)
            {
                _timer.Stop();
                await InvokeAsync(() =>
                    TimerOut.InvokeAsync()
                );
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}