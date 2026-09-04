using RabbitMQ.AMQP.Client;
using PhotoEditor.Models;
using RabbitMQ.AMQP.Client.Impl;
using PhotoEditor.Data;
using PhotoEditor.DTOs;
using System.Text.Json;
using System.Text;

namespace PhotoEditor.Services;

public class ImageTaskScheduler(IImageTaskRepository repo) : IAsyncDisposable
{
    // ----- PUBLISHER CONFIG ----- //
    private const string ExchangeName = "EditorExchange";
    private const string ContainerId = "photo-editor";
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

        IManagement management = _connection.Management();

        IExchangeSpecification exchangeSpec = management.Exchange(ExchangeName).Type("fanout");
        await exchangeSpec.DeclareAsync();

        IQueueSpecification requestQueue = management.Queue(RequestQueueName);
        await requestQueue.DeclareAsync();

        IBindingSpecification bindSpec = management
            .Binding()
            .SourceExchange(exchangeSpec)
            .DestinationQueue(RequestQueueName)
            .Key(string.Empty);

        await bindSpec.BindAsync();
        _requester = await _connection
            .RequesterBuilder()
            .RequestAddress()
            .Exchange(ExchangeName)
            .Requester()
            .BuildAsync();
    }

    public async Task<(CreateTaskResult, ImageTask?)> LoadTaskAsync(TaskOptions options)
    {
        if (_requester is null)
        {
            throw new InvalidOperationException("RunTashScheduler is not initialized.");
        }

        var task = new ImageTask
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

        var optionsJson = JsonSerializer.Serialize(options);
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