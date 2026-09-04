using PhotoEditor.Models;

namespace PhotoEditor.Data;

public interface IImageTaskRepository
{
    Task<bool> AddTaskAsync(ImageTask task, CancellationToken ct = default);
    Task<bool> DeleteTaskAsync(string task_id, CancellationToken ct = default);
    Task<ImageTask?> GetTaskAsync(string task_id, CancellationToken ct = default);
    Task<bool> UpdateTaskAsync(ImageTask task, CancellationToken ct = default);
    Task<bool> ExistsTaskAsync(string task_id, CancellationToken ct = default);
}