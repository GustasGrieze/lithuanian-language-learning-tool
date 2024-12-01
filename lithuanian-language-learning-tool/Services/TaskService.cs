using lithuanian_language_learning_tool.Data;
using lithuanian_language_learning_tool.Models;
using Microsoft.EntityFrameworkCore;


namespace lithuanian_language_learning_tool.Services
{
    public interface ITaskService<T> where T : CustomTask
    {
        Task AddTaskAsync(T task);
        Task<T> GetTaskAsync(int taskId);
        Task<List<T>> GetAllTasksAsync();
        Task UpdateTaskAsync(T task);
        Task DeleteTaskAsync(int taskId);
        Task<List<string>> GetOptionsAsync(int taskId);
    }

    public class TaskService<T> : ITaskService<T> where T : CustomTask
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddTaskAsync(T task)
        {
            _context.CustomTasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task<T> GetTaskAsync(int taskId)
        {
            return await _context.CustomTasks
                .OfType<T>() // Filters to the specific derived type
                .Include(ct => ct.AnswerOptions)
                .FirstOrDefaultAsync(ct => ct.Id == taskId);
        }

        public async Task<List<T>> GetAllTasksAsync()
        {
            return await _context.CustomTasks
                .OfType<T>()
                .Include(ct => ct.AnswerOptions)
                .ToListAsync();
        }

        public async Task UpdateTaskAsync(T task)
        {
            var existingTask = await _context.CustomTasks
                .OfType<T>()
                .Include(ct => ct.AnswerOptions)
                .FirstOrDefaultAsync(ct => ct.Id == task.Id);

            if (existingTask != null)
            {
                existingTask.Sentence = task.Sentence;
                existingTask.UserText = task.UserText;
                existingTask.CorrectAnswer = task.CorrectAnswer;
                existingTask.Explanation = task.Explanation;
                existingTask.TaskStatus = task.TaskStatus;
                existingTask.Topic = task.Topic;

                // Set new options via Options property, which handles AnswerOptions
                existingTask.Options = task.Options;

                if (existingTask is PunctuationTask punctuationTask && task is PunctuationTask updatedPunctuationTask)
                {
                    punctuationTask.Highlights = updatedPunctuationTask.Highlights;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            var task = await _context.CustomTasks
                .OfType<T>()
                .FirstOrDefaultAsync(ct => ct.Id == taskId);

            if (task != null)
            {
                _context.CustomTasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetOptionsAsync(int taskId)
        {
            var task = await GetTaskAsync(taskId);
            return task?.Options ?? new List<string>();
        }
    }
}

