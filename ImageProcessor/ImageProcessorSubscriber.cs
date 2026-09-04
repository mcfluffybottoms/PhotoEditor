using System.Text;
using System.Text.Json;
using ImageProcessor.ImageManipulation;
using ImageProcessor.Models;
using RabbitMQ.AMQP.Client;
using RabbitMQ.AMQP.Client.Impl;

namespace ImageProcessor;

public class ImageProcessorSubscriber : IAsyncDisposable
{
    // ----- CONSUMER CONFIG ----- //
    private const string ExchangeName = "EditorExchange";
    private const string ContainerId = "photo-editor";
    private const string RequestQueueName = "ImageEditorRequestQueue";
    IResponder? _responder;
    IEnvironment? _environment;
    IConnection? _connection;
    // --------------------------- //
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

        _responder = await _connection
            .ResponderBuilder()
            .RequestQueue(RequestQueueName)
            .Handler(HandleRequest)
            .BuildAsync();
    }
    private async Task<IMessage> HandleRequest(IResponder.IContext ctx, IMessage request)
    {
        if (request.Body() is null)
        {
            return CreateResponse(new AcceptTaskResult
            {
                Accepted = false,
                Error = "Request is empty"
            });
        }

        try
        {
            string json = Encoding.UTF8.GetString(request.Body());
            TaskOptions? options = JsonSerializer.Deserialize<TaskOptions>(json);

            if (options is null)
            {
                return CreateResponse(new AcceptTaskResult
                {
                    Accepted = false,
                    Error = "Invalid request."
                });
            }

            string resultPath = await SendAsync(options);
            return CreateResponse(new AcceptTaskResult
            {
                Uuid = options.Uuid,
                Accepted = true,
                ResultPath = resultPath
            });
        }
        catch (Exception e)
        {
            return CreateResponse(new AcceptTaskResult
            {
                Accepted = false,
                Error = $"Error while processing request: {e.Message}."
            });
        }
    }
    private static AmqpMessage CreateResponse(AcceptTaskResult result)
    {
        string json = JsonSerializer.Serialize(result);
        return new AmqpMessage(Encoding.UTF8.GetBytes(json));
    }
    private static async Task<string> SendAsync(TaskOptions options)
    {
        var result = await ImageOperations.Process(options);

        var filename = options.Filename;
        if (filename is null)
        {
            do
            {
                filename = Guid.NewGuid().ToString();
            } while (File.Exists($"{filename}.png"));
        }
        string path = $"{filename}.png";
        result.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        return path;
    }
    public async ValueTask DisposeAsync()
    {
        if (_responder is not null)
        {
            await _responder.CloseAsync();
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