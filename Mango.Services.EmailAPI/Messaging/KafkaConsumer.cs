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
        private readonly string _topic;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private Task _executetask;

        public KafkaConsumer(IConfiguration configuration, ILogger<KafkaConsumer> logger, EmailService emailService)
        {
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
            _topic = _configuration["TopicAndQueueNames:EmailShoppingCartQueue"] ?? throw new ArgumentNullException();
            var config = new ConsumerConfig
            {
                GroupId = "kafka-consumer-group",
                BootstrapServers = _configuration["ServiceBusConnectionString"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _executetask = Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topic);
            _executetask = Task.Run(() => ConsumeMessage(stoppingToken), stoppingToken);
            _ = _executetask;
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
            var body = Encoding.UTF8.GetString(message.Body);

            string email = JsonConvert.DeserializeObject<string>(body);
            try
            {
                //TODO - try to log email
                await _emailService.RegisterUserEmailAndLog(email);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}
