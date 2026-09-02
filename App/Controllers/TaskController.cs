using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoEditor.Dtos;
using PhotoEditor.Services;

namespace PhotoEditor.Controllers;

[ApiController]
[Route("")]
[Authorize]
public class TaskController(RunTaskScheduler scheduler) : ControllerBase
{
    [HttpPost("task")]
    public IActionResult LoadTask()
    {
        var res = scheduler.LoadTask();
        return Ok(res);
    }
    [HttpGet("status/{task_id}")]
    public IActionResult LoadTaskStatus(string task_id)
    {
        var res = scheduler.TaskStatus(task_id);
        return Ok(new StatusResponseDto(res));
    }
    [HttpGet("result/{task_id}")]
    public IActionResult LoadTaskResult(string task_id)
    {
        var res = scheduler.TaskResult(task_id);
        return Ok(res);
    }
}