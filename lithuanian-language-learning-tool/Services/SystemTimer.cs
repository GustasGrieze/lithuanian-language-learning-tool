// File: Services/SystemTimer.cs

namespace lithuanian_language_learning_tool.Services
{
    public interface ITimer : IDisposable
    {
        event EventHandler<TimerElapsedEventArgs> Elapsed;
        bool AutoReset { get; set; }
        double Interval { get; set; }
        void Start();
        void Stop();
    }

    public class TimerElapsedEventArgs : EventArgs
    {
        public DateTime SignalTime { get; }

        public TimerElapsedEventArgs(DateTime signalTime)
        {
            SignalTime = signalTime;
        }
    }


    public class SystemTimer : ITimer
    {
        private readonly System.Timers.Timer _timer;

        public SystemTimer()
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += (s, e) => Elapsed?.Invoke(s, new TimerElapsedEventArgs(e.SignalTime));
        }

        public event EventHandler<TimerElapsedEventArgs> Elapsed;

        public bool AutoReset
        {
            get => _timer.AutoReset;
            set => _timer.AutoReset = value;
        }

        public double Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        public void Start() => _timer.Start();

        public void Stop() => _timer.Stop();

        public void Dispose() => _timer.Dispose();
    }

}
