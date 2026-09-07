using ImageProcessor;

const string brokerUri = "amqp://guest:guest@localhost:5672/%2f";

await using var subscriber = new ImageProcessorSubscriber();
await subscriber.InitAsync(brokerUri);

Console.WriteLine("Image processor is running.");
Console.WriteLine("Press Ctrl+C to stop.");

var completion = new TaskCompletionSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    completion.TrySetResult();
};
await completion.Task;