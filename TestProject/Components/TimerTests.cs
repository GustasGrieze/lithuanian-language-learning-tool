using lithuanian_language_learning_tool.Services;
using Moq;
using TimerComponent = lithuanian_language_learning_tool.Components.Timer;

namespace TestProject.Components.Tests
{
    public class TimerTests : TestContext
    {
        private readonly Mock<ITimer> _timerMock;
        private readonly Mock<IUploadService> _uploadServiceMock;

        public TimerTests()
        {
            _timerMock = new Mock<ITimer>();
            _uploadServiceMock = new Mock<IUploadService>();

            // Register mocked services
            Services.AddSingleton<ITimer>(_timerMock.Object);
            Services.AddSingleton<IUploadService>(_uploadServiceMock.Object);
        }

        [Fact]
        public void Timer_Initial_Rendering_Displays_Correct_Time()
        {
            // Arrange
            int secondsToRun = 120;

            // Act
            var component = RenderComponent<TimerComponent>(parameters => parameters
                .Add(p => p.SecondsToRun, secondsToRun)
            );

            // Assert
            var timeDisplay = component.Find(".clock h1");
            Assert.Equal("02:00", timeDisplay.TextContent);
        }

        //[Fact]
        //public void Timer_Countdown_Updates_Time_Display()
        //{
        //    // Arrange
        //    int secondsToRun = 10;
        //    var component = RenderComponent<TimerComponent>(parameters => parameters
        //        .Add(p => p.SecondsToRun, secondsToRun)
        //    );

        //    // Act
        //    _timerMock.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));

        //    // Assert
        //    var timeDisplay = component.Find(".clock h1");
        //    Assert.Equal("00:09", timeDisplay.TextContent);
        //}

        //[Fact]
        //public void Timer_Reaches_Zero_Invokes_TimerOut_Callback()
        //{
        //    // Arrange
        //    int secondsToRun = 2;
        //    bool timerOutInvoked = false;

        //    var component = RenderComponent<TimerComponent>(parameters => parameters
        //        .Add(p => p.SecondsToRun, secondsToRun)
        //        .Add(p => p.TimerOut, EventCallback.Factory.Create(this, () => timerOutInvoked = true))
        //    );

        //    // Act
        //    _timerMock.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));
        //    _timerMock.Raise(t => t.Elapsed += null, new TimerElapsedEventArgs(DateTime.Now));

        //    // Assert
        //    Assert.True(timerOutInvoked);
        //}

        [Fact]
        public void ProgressBar_Class_Is_Correct_Based_On_Resetting()
        {
            // Arrange
            int secondsToRun = 60;

            var component = RenderComponent<TimerComponent>(parameters => parameters
                .Add(p => p.SecondsToRun, secondsToRun)
            );

            // Assert
            var progressBar = component.Find("div.progress-bar-reset");
            Assert.NotNull(progressBar);
        }

        //[Fact]
        //public async Task ResetTimer_Stops_And_Restarts_Timer()
        //{
        //    // Arrange
        //    int secondsToRun = 30;
        //    var component = RenderComponent<TimerComponent>(parameters => parameters
        //        .Add(p => p.SecondsToRun, secondsToRun)
        //    );

        //    // Act
        //    await component.InvokeAsync(() => component.Instance.ResetTimer());

        //    // Assert
        //    _timerMock.Verify(t => t.Stop(), Times.Once); // Verify that the timer was stopped once
        //    _timerMock.Verify(t => t.Start(), Times.Exactly(2)); // Verify that the timer was started twice
        //}

        //[Fact]
        //public async Task StopTimer_Stops_Timer()
        //{
        //    // Arrange
        //    int secondsToRun = 30;
        //    var component = RenderComponent<TimerComponent>(parameters => parameters
        //        .Add(p => p.SecondsToRun, secondsToRun)
        //    );

        //    // Act
        //    await component.InvokeAsync(() => component.Instance.StopTimer());

        //    // Assert
        //    _timerMock.Verify(t => t.Stop(), Times.Once);
        //}

        //[Fact]
        //public void Dispose_Disposes_Timer()
        //{
        //    // Arrange
        //    int secondsToRun = 30;
        //    var component = RenderComponent<TimerComponent>(parameters => parameters
        //        .Add(p => p.SecondsToRun, secondsToRun)
        //    );

        //    // Act
        //    component.Instance.Dispose(); // Explicitly call Dispose on the component instance

        //    // Assert
        //    _timerMock.Verify(t => t.Dispose(), Times.Once);
        //}
    }
}
