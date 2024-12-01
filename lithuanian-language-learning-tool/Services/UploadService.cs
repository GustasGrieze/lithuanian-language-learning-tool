using global::lithuanian_language_learning_tool.Exceptions;
using lithuanian_language_learning_tool.Models;
using System.Text.Json;


namespace lithuanian_language_learning_tool.Services
{
    public interface IUploadService
    {
        Task ValidateAndUploadAsync(string jsonContent, string taskType);
        void LogException(Exception ex);
    }

    public class UploadService : IUploadService
    {
        private readonly ILogger<UploadService> _logger;
        private readonly ITaskService<PunctuationTask> _punctuationTaskService;
        private readonly ITaskService<SpellingTask> _spellingTaskService;

        public UploadService(
            ILogger<UploadService> logger,
            ITaskService<PunctuationTask> punctuationTaskService,
            ITaskService<SpellingTask> spellingTaskService)
        {
            _logger = logger;
            _punctuationTaskService = punctuationTaskService;
            _spellingTaskService = spellingTaskService;
        }

        public async Task ValidateAndUploadAsync(string jsonContent, string taskType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonContent))
                    throw new TaskUploadException("Failas yra tuščias.");

                List<Dictionary<string, object>> tasks = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonContent);

                if (tasks == null || !tasks.Any())
                    throw new TaskUploadException("Failas neturi užduočių arba yra tuščias.");

                foreach (var task in tasks)
                {
                    ValidateTaskStructure(task, taskType);
                }

                // If validation passes, deserialize into specific task types and add them
                if (taskType.Equals("punctuation", StringComparison.OrdinalIgnoreCase))
                {
                    var punctuationTasks = JsonSerializer.Deserialize<List<PunctuationTask>>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (punctuationTasks == null || !punctuationTasks.Any())
                        throw new TaskUploadException("Skyrybos užduočių sąrašas yra tuščias arba netinkamai suformatuotas.");

                    foreach (var punctuationTask in punctuationTasks)
                    {
                        punctuationTask.UserText = punctuationTask.Sentence;
                        await _punctuationTaskService.AddTaskAsync(punctuationTask);
                    }
                }
                else if (taskType.Equals("spelling", StringComparison.OrdinalIgnoreCase))
                {
                    var spellingTasks = JsonSerializer.Deserialize<List<SpellingTask>>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (spellingTasks == null || !spellingTasks.Any())
                        throw new TaskUploadException("Rašybos užduočių sąrašas yra tuščias arba netinkamai suformatuotas.");

                    foreach (var spellingTask in spellingTasks)
                    {
                        spellingTask.UserText = spellingTask.Sentence;
                        await _spellingTaskService.AddTaskAsync(spellingTask);
                    }
                }
                else
                {
                    throw new TaskUploadException("Neteisingas užduoties tipas.");
                }
            }
            catch (JsonException ex)
            {
                LogException(ex);
                throw new TaskUploadException("JSON failas yra netinkamai suformatuotas.", ex);
            }
            catch (Exception ex) when (!(ex is TaskUploadException))
            {
                LogException(ex);
                throw new TaskUploadException($"Klaida įkeliant užduotis: {ex.Message}", ex);
            }
        }

        private void ValidateTaskStructure(Dictionary<string, object> task, string taskType)
        {
            var requiredFields = new[] { "Sentence", "Options", "CorrectAnswer", "Explanation" };
            foreach (var field in requiredFields)
            {
                if (!task.ContainsKey(field))
                    throw new TaskUploadException($"Netinkama užduoties struktūra: trūksta laukų '{field}'.");
            }

            // Validate "Options" field based on task type
            var options = JsonSerializer.Deserialize<List<string>>(task["Options"].ToString());
            if (options == null || !options.Any())
                throw new TaskUploadException("Options laukelis yra tuščias arba netinkamas.");

            if (taskType.Equals("punctuation", StringComparison.OrdinalIgnoreCase))
            {
                var validPunctuation = new[] { ".", ",", ";", ":", "!", "?" };
                if (!options.All(opt => validPunctuation.Contains(opt)))
                    throw new TaskUploadException("Netinkama Options struktūra: skyrybos užduotyse leidžiami tik skyrybos ženklai.");
            }
            else if (taskType.Equals("spelling", StringComparison.OrdinalIgnoreCase))
            {
                if (!options.All(opt => opt.Length <= 3 && opt.All(char.IsLetter)))
                    throw new TaskUploadException("Netinkama Options struktūra: rašybos užduotyse leidžiamos tik raidės ir ilgis iki 3 simbolių.");
            }
        }

        public void LogException(Exception ex)
        {
            _logger.LogError(ex, "Klaida įkeliant užduotį.");
        }
    }

}


