using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class MessageBus:IMessageBus
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ProducerConfig config;
        public MessageBus(string bootstrapServers)
        {
            config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                ClientId = Dns.GetHostName(),
                Acks = Acks.All,
                MessageSendMaxRetries = 2, // Retry up to 5 times
                RetryBackoffMs = 200, // Wait 200ms between retries
                MessageTimeoutMs = 60000 // Set message timeout to 60 seconds
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublishMessage(object message,string queueName)
        {

            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    try
                    {
                        var deliveryResult = await producer.ProduceAsync(queueName, new Message<Null, string> { Value = System.Text.Json.JsonSerializer.Serialize(message)});
                        Console.WriteLine($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
                    }
                    catch (ProduceException<Null, string> e)
                    {
                        Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Delivery failed: {e.Message}");
            }
        }
    }
}
