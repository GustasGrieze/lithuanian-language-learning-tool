using global::lithuanian_language_learning_tool.Exceptions;
using System.Text.Json;

namespace lithuanian_language_learning_tool.Services
{
    public interface IUploadService
    {
        void ValidateJsonStructure(string jsonContent, string taskType);
        void LogException(Exception ex);
    }

    public class UploadService : IUploadService
    {
        private readonly ILogger<UploadService> _logger;

        public UploadService(ILogger<UploadService> logger)
        {
            _logger = logger;
        }


        public void ValidateJsonStructure(string jsonContent, string taskType)
        {
            try
            {
                var tasks = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonContent);

                if (tasks == null || tasks.Count == 0)
                    throw new TaskUploadException("Failas neturi užduočių arba yra tuščias.");

                foreach (var task in tasks)
                {
                    if (!task.ContainsKey("Sentence") || !task.ContainsKey("Options") ||
                        !task.ContainsKey("CorrectAnswer") || !task.ContainsKey("Explanation"))
                    {
                        throw new TaskUploadException("Netinkama užduoties struktūra: trūksta laukų.");
                    }

                    // Validate "Options" field based on task type
                    var options = JsonSerializer.Deserialize<List<string>>(task["Options"].ToString());
                    if (options == null || options.Count == 0)
                    {
                        throw new TaskUploadException("Options laukelis yra tuščias arba netinkamas.");
                    }

                    if (taskType == "punctuation")
                    {
                        if (!options.All(opt => new[] { ".", ",", ";", ":", "!", "?" }.Contains(opt)))
                        {
                            throw new TaskUploadException("Netinkama Options struktūra: skyrybos užduotyse leidžiami tik skyrybos ženklai.");
                        }
                    }
                    else if (taskType == "spelling")
                    {
                        if (!options.All(opt => opt.Length <= 3 && char.IsLetter(opt[0])))
                        {
                            throw new TaskUploadException("Netinkama Options struktūra: rašybos užduotyse leidžiamos tik raidės.");
                        }
                    }
                }
            }
            catch (JsonException)
            {
                throw new TaskUploadException("JSON failas yra netinkamai suformatuotas.");
            }
        }
        public void LogException(Exception ex)
        {
            // Using ILogger for logging
            _logger.LogError(ex, "Klaida įkeliant užduotį.");
        }
    }
}


