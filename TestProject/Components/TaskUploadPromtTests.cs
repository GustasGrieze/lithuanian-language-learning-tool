//using lithuanian_language_learning_tool.Components;
//using lithuanian_language_learning_tool.Exceptions;
//using lithuanian_language_learning_tool.Services;
//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Components.Forms;
//using Moq;
//using System;
//using System.Threading.Tasks;
//using TestProject.Helpers;

//namespace TestProject.Components.Tests
//{
//    public class TaskUploadPromptTests : TestContext
//    {
//        private readonly Mock<IUploadService> _uploadServiceMock;
//        private readonly FakeNavigationManager _navigationManager;

//        public TaskUploadPromptTests()
//        {
//            // Initialize mocks
//            _uploadServiceMock = new Mock<IUploadService>();

//            // Register mocked services first
//            Services.AddSingleton<IUploadService>(_uploadServiceMock.Object);

//            // Access FakeNavigationManager only after registering all services
//            _navigationManager = Services.GetRequiredService<FakeNavigationManager>();
//            _navigationManager.NavigateTo("https://localhost/rasybos/uzduotys");
//        }

//        [Fact]
//        public void Shows_InputFile_When_ShowUpload_Is_True()
//        {
//            // Arrange
//            var component = RenderComponent<TaskUploadPrompt>();

//            // Act
//            // Click the "Įkelti" (Upload) button
//            var uploadButton = component.Find("button.btn.btn-primary");
//            uploadButton.Click();

//            // Assert
//            // The InputFile should now be visible
//            Assert.True(component.FindAll("input[type='file']").Count > 0);
//        }

//        [Fact]
//        public async Task Clicking_UseExisting_Button_Invokes_OnUseDefaultTasks_Callback()
//        {
//            // Arrange
//            var useDefaultTasksInvoked = false;
//            var component = RenderComponent<TaskUploadPrompt>(parameters => parameters
//                .Add(p => p.OnUseDefaultTasks, EventCallback.Factory.Create(this, () => useDefaultTasksInvoked = true))
//            );

//            // Act
//            // Click the "Naudoti egzistuojančias" (Use Existing) button
//            var useExistingButton = component.Find("button.btn.btn-secondary");
//            useExistingButton.Click();

//            // Assert
//            Assert.True(useDefaultTasksInvoked);
//        }

//        [Fact]
//        public async Task HandleFileSelected_With_Valid_File_Invokes_OnTaskUploaded()
//        {
//            // Arrange
//            var sampleJson = "[{\"Sentence\":\"Test sentence.\",\"Options\":[\"a\",\"b\"],\"CorrectAnswer\":\"a\",\"Explanation\":\"End punctuation.\"}]";
//            var onTaskUploadedInvoked = false;
//            string uploadedContent = null;

//            // Setup UploadService to validate successfully
//            _uploadServiceMock.Setup(us => us.ValidateJsonStructure(sampleJson, "spelling"));

//            var component = RenderComponent<TaskUploadPrompt>(parameters => parameters
//                .Add(p => p.OnTaskUploaded, EventCallback.Factory.Create<string>(this, (string content) =>
//                {
//                    onTaskUploadedInvoked = true;
//                    uploadedContent = content;
//                }))
//            );

//            // Act
//            // Click the "Įkelti" (Upload) button to show the file input
//            var uploadButton = component.Find("button.btn.btn-primary");
//            uploadButton.Click();

//            // Find the InputFile component
//            IRenderedComponent<InputFile> inputFile = component.FindComponent<InputFile>();

//            // Simulate file upload using BUnit's UploadFile method
//            inputFile.UploadFiles(InputFileContent.CreateFromText(sampleJson, "tasks.json"));

//            // Assert
//            Assert.True(onTaskUploadedInvoked);
//            Assert.Equal(sampleJson, uploadedContent);
//            _uploadServiceMock.Verify(us => us.ValidateJsonStructure(sampleJson, "spelling"), Times.Once);
//            Assert.DoesNotContain("alert-danger", component.Markup); // No error message should be shown
//        }

//        [Fact]
//        public async Task HandleFileSelected_With_Invalid_File_Shows_ErrorMessage_And_Logs_Exception()
//        {
//            // Arrange
//            var invalidJson = "[{\"Sentence\":\"Test sentence\",\"Options\":[\"abc\"],\"CorrectAnswer\":\"a\",\"Explanation\":\"Explanation.\"}]"; // "abc" exceeds length <=3 and should be invalid if taskType is "spelling"
//            var component = RenderComponent<TaskUploadPrompt>();

//            // Setup UploadService to throw TaskUploadException
//            _uploadServiceMock.Setup(us => us.ValidateJsonStructure(invalidJson, "spelling"))
//                .Throws(new TaskUploadException("Netinkama Options struktūra: rašybos užduotyse leidžiamos tik raidės."));

//            // Act
//            // Click the "Įkelti" (Upload) button to show the file input
//            var uploadButton = component.Find("button.btn.btn-primary");
//            uploadButton.Click();

//            // Find the InputFile component
//            var inputFile = component.FindComponent<InputFile>();

//            // Simulate file upload with invalid content
//            inputFile.UploadFiles(InputFileContent.CreateFromText(invalidJson, "tasks.json"));

//            // Assert
//            // Verify that the error message is displayed
//            var errorDiv = component.Find("div.alert.alert-danger");
//            Assert.Equal("Klaida įkeliant failą: Netinkama Options struktūra: rašybos užduotyse leidžiamos tik raidės.", errorDiv.TextContent.Trim());

//            // Verify that ValidateJsonStructure was called once
//            _uploadServiceMock.Verify(us => us.ValidateJsonStructure(invalidJson, "spelling"), Times.Once);

//            // Verify that LogException was called once
//            _uploadServiceMock.Verify(us => us.LogException(It.IsAny<TaskUploadException>()), Times.Once);
//        }

//        [Fact]
//        public async Task HandleFileSelected_With_Empty_File_Shows_ErrorMessage_And_Logs_Exception()
//        {
//            // Arrange
//            var emptyContent = "";
//            var component = RenderComponent<TaskUploadPrompt>();

//            // Act
//            var uploadButton = component.Find("button.btn.btn-primary");
//            await component.InvokeAsync(() => uploadButton.Click());

//            var inputFile = component.FindComponent<InputFile>();
//            var files = new[] { new MockBrowserFile("tasks.json", "application/json", emptyContent) };
//            await component.InvokeAsync(() => inputFile.Instance.OnChange.InvokeAsync(new InputFileChangeEventArgs(files)));

//            // Assert
//            var errorDiv = component.Find("div.alert.alert-danger");
//            Assert.Equal("Klaida įkeliant failą: Įkelto failo turinys yra tuščias.", errorDiv.TextContent.Trim());

//            // Verify that ValidateJsonStructure was never called
//            _uploadServiceMock.Verify(us => us.ValidateJsonStructure(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

//            // Verify that LogException was called with the expected exception
//            _uploadServiceMock.Verify(us => us.LogException(It.IsAny<TaskUploadException>()), Times.Once);
//        }




//        [Theory]
//        [InlineData("/rasybos/uzduotys", "spelling")]
//        [InlineData("/skyrybos/uzduotys", "punctuation")]
//        [InlineData("/unknown/path", "")]
//        public void SetTaskTypeBasedOnUrl_Sets_Correct_TaskType(string uri, string expectedTaskType)
//        {
//            // Arrange
//            _navigationManager.NavigateTo($"https://localhost{uri}");
//            var component = RenderComponent<TaskUploadPrompt>();

//            // Act
//            // The taskType is set during OnInitialized, which has already been called

//            // Assert
//            Assert.Equal(expectedTaskType, component.Instance.TaskType);
//        }

//        [Fact]
//        public async Task StartWithDefaultTasks_Click_Invokes_OnUseDefaultTasks_Callback()
//        {
//            // Arrange
//            var onUseDefaultTasksInvoked = false;
//            var component = RenderComponent<TaskUploadPrompt>(parameters => parameters
//                .Add(p => p.OnUseDefaultTasks, EventCallback.Factory.Create(this, () => onUseDefaultTasksInvoked = true))
//            );

//            // Act
//            // Click the "Naudoti egzistuojančias" (Use Existing) button
//            var useExistingButton = component.Find("button.btn.btn-secondary");
//            useExistingButton.Click();

//            // Assert
//            Assert.True(onUseDefaultTasksInvoked);
//        }

//        [Fact]
//        public async Task HandleFileSelected_With_Exception_Shows_ErrorMessage_And_Logs_Exception()
//        {
//            // Arrange
//            var sampleJson = "[{\"Sentence\":\"Test sentence.\",\"Options\":[\"a\",\"b\"],\"CorrectAnswer\":\"a\",\"Explanation\":\"End punctuation.\"}]";
//            var component = RenderComponent<TaskUploadPrompt>();

//            // Mock UploadService to throw a generic exception
//            _uploadServiceMock.Setup(us => us.ValidateJsonStructure(sampleJson, "spelling"))
//                .Throws(new Exception("Unexpected error"));

//            // Act
//            var uploadButton = component.Find("button.btn.btn-primary");
//            await component.InvokeAsync(() => uploadButton.Click());

//            var inputFile = component.FindComponent<InputFile>();
//            var files = new[] { new MockBrowserFile("tasks.json", "application/json", sampleJson) };
//            await component.InvokeAsync(() => inputFile.Instance.OnChange.InvokeAsync(new InputFileChangeEventArgs(files)));

//            // Assert
//            var errorDiv = component.Find("div.alert.alert-danger");
//            Assert.Equal("Nenumatyta klaida: Unexpected error", errorDiv.TextContent.Trim());

//            // Verify that ValidateJsonStructure was called once
//            _uploadServiceMock.Verify(us => us.ValidateJsonStructure(sampleJson, "spelling"), Times.Once);

//            // Verify that LogException was called once
//            _uploadServiceMock.Verify(us => us.LogException(It.IsAny<Exception>()), Times.Once);
//        }



//    }
//}
