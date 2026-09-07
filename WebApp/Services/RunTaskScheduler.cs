using RabbitMQ.AMQP.Client;
using PhotoEditor.Models;
using RabbitMQ.AMQP.Client.Impl;
using PhotoEditor.Data;
using PhotoEditor.DTOs;
using System.Text.Json;
using System.Text;

namespace PhotoEditor.Services;

public class ImageTaskScheduler(IImageTaskRepository repo, IFileStorageRepository fileRepo) : IAsyncDisposable
{
    // ----- PUBLISHER CONFIG ----- //
    private const string ExchangeName = "EditorExchange";
    private const string ContainerId = "photo-editor-api";
    private const string RequestQueueName = "ImageEditorRequestQueue";
    IRequester? _requester;
    IEnvironment? _environment;
    IConnection? _connection;
    // ---------------------------- //
    public enum CreateTaskResult
    {
        CONFLICT,
        ACCEPTED,
        DENIED,
    }

    public async Task InitAsync(string brokerUri)
    {
        ConnectionSettings settings = ConnectionSettingsBuilder.Create()
            .Uri(new Uri(brokerUri))
            .ContainerId(ContainerId)
            .Build();

        _environment = AmqpEnvironment.Create(settings);
        _connection = await _environment.CreateConnectionAsync();

        _requester = await _connection
            .RequesterBuilder()
            .RequestAddress()
            .Queue(RequestQueueName)
            .Requester()
            .BuildAsync();
    }

    public async Task<(CreateTaskResult, ImageTask?)> LoadTaskAsync(FileUploadDto options)
    {
        if (_requester is null)
        {
            throw new InvalidOperationException("RunTaskScheduler is not initialized.");
        }

        var task = new ImageTask // TODO ADD MORE INFO ON TASK
        {
            Uuid = Guid.NewGuid(),
            Status = ImageTaskStatus.CREATED,
            ResultPath = null
        };

        bool isAdded = await repo.AddTaskAsync(task);
        if (!isAdded)
        {
            return (CreateTaskResult.CONFLICT, null);
        }

        var path = await fileRepo.StoreFile(options.Image, options.NewFilename);
        if (path is null)
        {
            return (CreateTaskResult.DENIED, null);
        }
        var optionsJson = JsonSerializer.Serialize(new TaskOptions
        {
            Uuid = task.Uuid.ToString(),
            ImagePath = path,
            Filename = options.NewFilename,
            Parameters = options.Parameters
        });
        IMessage message = new AmqpMessage(Encoding.UTF8.GetBytes(optionsJson));
        IMessage reply = await _requester.PublishAsync(message);
        return await ProcessReply(task, reply);
    }

    public async Task<(CreateTaskResult, ImageTask?)> ProcessReply(ImageTask task, IMessage reply)
    {
        string json = Encoding.UTF8.GetString(reply.Body()!);
        AcceptTaskResult? result = JsonSerializer.Deserialize<AcceptTaskResult>(json)
            ?? throw new InvalidOperationException($"{nameof(ProcessReply)} received an invalid response.");
        if (result.Accepted)
        {
            task.Status = ImageTaskStatus.COMPLETED;
            task.ResultPath = result.ResultPath;
            await repo.UpdateTaskAsync(task);
            return (CreateTaskResult.ACCEPTED, task);
        }
        else
        {
            task.Status = ImageTaskStatus.FAILED;
            task.Error = result.Error;
            await repo.UpdateTaskAsync(task);
            return (CreateTaskResult.DENIED, task);
        }
    }

    public async Task<ImageTaskStatus?> TaskStatusAsync(string task_id)
    {
        var task = await repo.GetTaskAsync(task_id);
        if (task is null)
        {
            return null;
        }
        return task.Status;
    }
    public async Task<string?> TaskResultAsync(string task_id)
    {
        var task = await repo.GetTaskAsync(task_id);
        if (task is null)
        {
            return null;
        }
        return task.ResultPath;
    }

    public async ValueTask DisposeAsync()
    {
        if (_requester is not null)
        {
            await _requester.CloseAsync();
        }
        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }
        if (_environment is not null)
        {
            await _environment.CloseAsync();
        }
        GC.SuppressFinalize(this);
    }
}