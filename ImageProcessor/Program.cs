using ImageProcessor;

const string brokerUri = "amqp://guest:guest@localhost:5672/%2f";

await using var subscriber = new ImageProcessorSubscriber();
await subscriber.InitAsync(brokerUri);