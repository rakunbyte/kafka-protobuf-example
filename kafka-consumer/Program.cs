// See https://aka.ms/new-console-template for more information

using Com.Github.Rakunbyte;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = "localhost:19092",
    GroupId = "protobuf-example-consumer-group"
};


CancellationTokenSource cts = new CancellationTokenSource();
/* IF YOU WANT TO DESERIALIZE DIRECTLY FROM KAFKA
    using (var consumer =
           new ConsumerBuilder<string, SimpleMessage>(consumerConfig)
               .SetValueDeserializer(new ProtobufDeserializer<SimpleMessage>().AsSyncOverAsync())
               .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
               .Build())
*/
var xxx = new ProtobufDeserializer<SimpleMessage>();

using (var consumer =
       new ConsumerBuilder<string, byte[]>(consumerConfig)
           .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
           .Build())
    {
        consumer.Subscribe("SimpleMessageTopic");
        Console.WriteLine("Subscribing to Topic: SimpleMessageTopic");
        try
        {
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume(cts.Token);
                    var simpleMessageBytes = consumeResult.Message.Value;

                    var simpleMessage = await xxx.DeserializeAsync(simpleMessageBytes, false, new SerializationContext());
                    Console.WriteLine($"CONSUMING key: {consumeResult.Message.Key}, Content {simpleMessage.Content}, DateTime: {simpleMessage.DateTime}");
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume error: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
        }
    }