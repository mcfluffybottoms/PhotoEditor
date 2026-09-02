
using PhotoEditor.Models;

namespace PhotoEditor.Services;

public class RunTaskScheduler
{
    public RunTask<string> LoadTask()
    {
        return new RunTask<string>
        {
            Uuid = Guid.NewGuid(),
            Status = RunTaskStatus.CREATED,
            Result = null
        };
    }
    public RunTaskStatus TaskStatus(string task_id)
    {
        return RunTaskStatus.COMPLETED;
    }
    public string TaskResult(string task_id)
    {
        return "result" + task_id;
    }
}