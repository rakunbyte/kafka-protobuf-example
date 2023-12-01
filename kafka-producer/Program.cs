using Com.Github.Rakunbyte;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;


var producerConfig = new ProducerConfig {
    BootstrapServers = "localhost:19092",
    SecurityProtocol = SecurityProtocol.Plaintext,
};

var schemaRegistryConfig = new SchemaRegistryConfig
{
    Url = "http://localhost:8081"
};

using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
using var producer =
    new ProducerBuilder<string, SimpleMessage>(producerConfig)
        .SetValueSerializer(new ProtobufSerializer<SimpleMessage>(schemaRegistry))
        .Build();

while (true)
{
    for (var i = 0; i < 100; i++)
    {
        try
        {
            var key = i.ToString();
            var message = new SimpleMessage
            {
                Content = $"Some Content {i}",
                DateTime = DateTime.UtcNow.ToLongDateString()
            };
                
            var dr = await producer.ProduceAsync("SimpleMessageTopic", new Message<string, SimpleMessage> { Key = key, Value=message });
            Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Delivery failed: {e.Error.Reason}");
        }
    }

    producer.Flush(TimeSpan.FromSeconds(10));
}