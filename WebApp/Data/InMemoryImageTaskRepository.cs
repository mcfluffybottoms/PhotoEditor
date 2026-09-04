using PhotoEditor.DTOs;
using PhotoEditor.Models;

namespace PhotoEditor.Data;

public class InMemoryImageTaskRepository : IImageTaskRepository
{
    private readonly Dictionary<string, ImageTask> tasks = [];

    public Task<bool> AddTaskAsync(ImageTask task, CancellationToken ct = default)
    {
        if (tasks.ContainsKey(task.Uuid.ToString()))
        {
            return Task.FromResult(false);
        }
        tasks.Add(task.Uuid.ToString(), task);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteTaskAsync(string task_id, CancellationToken ct = default)
    {
        if (!tasks.ContainsKey(task_id))
        {
            return Task.FromResult(false);
        }
        tasks.Remove(task_id);
        return Task.FromResult(true);
    }

    public Task<bool> ExistsTaskAsync(string task_id, CancellationToken ct = default)
    {
        return Task.FromResult(tasks.ContainsKey(task_id));
    }

    public Task<ImageTask?> GetTaskAsync(string task_id, CancellationToken ct = default)
    {
        return Task.FromResult(tasks.GetValueOrDefault(task_id));
    }

    public Task<bool> UpdateTaskAsync(ImageTask task, CancellationToken ct = default)
    {
        if (!tasks.ContainsKey(task.Uuid.ToString()))
        {
            return Task.FromResult(false);
        }
        tasks[task.Uuid.ToString()] = task;
        return Task.FromResult(true);
    }
}