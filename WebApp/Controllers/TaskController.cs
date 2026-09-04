using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoEditor.DTOs;
using PhotoEditor.Models;
using PhotoEditor.Services;

namespace PhotoEditor.Controllers;

[ApiController]
[Route("")]
[Authorize]
public class TaskController(ImageTaskScheduler scheduler) : ControllerBase
{
    [HttpPost("task")]
    public async Task<IActionResult> LoadTask(TaskOptions options)
    {
        var (status, task) = await scheduler.LoadTaskAsync(options);
        return status switch
        {
            ImageTaskScheduler.CreateTaskResult.ACCEPTED => Ok(new { task }),
            ImageTaskScheduler.CreateTaskResult.CONFLICT => Conflict(new ErrorDto("Task with this uuid already exists. Try uploading again.")),
            ImageTaskScheduler.CreateTaskResult.DENIED => StatusCode(500, "Error while queueing task."),
            _ => throw new UnreachableException("Code is unreachanble."),
        };
    }
    [HttpGet("status/{task_id}")]
    public async Task<IActionResult> LoadTaskStatus(string task_id)
    {
        var res = await scheduler.TaskStatusAsync(task_id);
        if (!res.HasValue)
        {
            return NotFound(new ErrorDto("Task not found."));
        }
        return Ok(new StatusResponseDto(res.Value));
    }
    [HttpGet("result/{task_id}")]
    public async Task<IActionResult> LoadTaskResult(string task_id)
    {
        var res = await scheduler.TaskResultAsync(task_id);
        if (res is null)
        {
            return NotFound(new ErrorDto("Task not found."));
        }
        return Ok(res);
    }
}