using Azure.Messaging.ServiceBus;
using Confluent.Kafka;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly ILogger<KafkaConsumer> _logger;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IConsumer<Ignore, string> _registrationconsumer;
        private readonly string _emailtopic;
        private readonly string _registrationtopic;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private List<Task> _executetasks;

        public KafkaConsumer(IConfiguration configuration, ILogger<KafkaConsumer> logger, EmailService emailService)
        {
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
            _emailtopic = _configuration["TopicAndQueueNames:EmailShoppingCartQueue"] ?? throw new ArgumentNullException();
            _registrationtopic = _configuration["TopicAndQueueNames:RegisterUserQueue"] ?? throw new ArgumentNullException();

            var config = new ConsumerConfig
            {
                GroupId = "kafka-consumer-group",
                BootstrapServers = _configuration["ServiceBusConnectionString"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _registrationconsumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _executetasks = new List<Task>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_emailtopic);
            _registrationconsumer.Subscribe(_registrationtopic);
            _executetasks = new List<Task>() { Task.Run(() => ConsumeMessage(stoppingToken), stoppingToken), Task.Run(() => OnUserRegisterRequestReceived(stoppingToken), stoppingToken) };
            _ = _executetasks;
        }
        private async Task ConsumeMessage(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(consumeResult.Message.Value);
                    //TODO - try to log email
                    await _emailService.EmailCartAndLog(objMessage);
                    _logger.LogInformation($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Consumer cancellation requested.");
                    break;
                }
                catch (ConsumeException e)
                {
                    _logger.LogError($"Error occurred: {e.Error.Reason}");
                }
            }
            return ;
        }


        private async Task OnUserRegisterRequestReceived(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _registrationconsumer.Consume(stoppingToken);
                    string email = JsonConvert.DeserializeObject<string>(consumeResult.Message.Value);
                    await _emailService.RegisterUserEmailAndLog(email);
                    _logger.LogInformation($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Consumer cancellation requested.");
                    break;
                }
                catch (ConsumeException e)
                {
                    _logger.LogError($"Error occurred: {e.Error.Reason}");
                }
            }
            return;
        }


        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}
