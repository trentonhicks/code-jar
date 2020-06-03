using CodeJar.Domain;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace CodeJar.ServiceBusAzure
{
    public class ReceiveServiceBus : IHostedService
    {
        private IQueueClient _queueClient;
        const string QueueName = "notifications";
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;
        private readonly ICodeRepository _code;

        public ReceiveServiceBus(ILoggerFactory loggerFactory, IConfiguration configuration, ICodeRepository code)
        {
            _logger = loggerFactory.CreateLogger<ReceiveServiceBus>();
            _loggerFactory = loggerFactory;
            _configuration = configuration;
            _code = code;
        }
        public async Task RecieveMessage()
        {
            _queueClient = new QueueClient(_configuration.GetConnectionString("AzureServiceBus"), QueueName);

            await _queueClient.CloseAsync();
        }

        public async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var data = Encoding.UTF8.GetString(message.Body);

            var batch = JsonConvert.DeserializeObject<Batch>(data);

            //Run the mehtod that generates codes based on the size of the batch.

            await _code.AddCodesAsync(batch);

            await Task.Delay(5000);
        }

        protected void ProcessError(Exception e)
        {
            _logger.LogError(e, "Error while processing queue item in BusListenerService.");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _queueClient = new QueueClient(_configuration.GetConnectionString("AzureServiceBus"), QueueName);

            _logger.LogDebug($"BusListenerService starting; registering message handler.");

            var messageHandlerOptions = new MessageHandlerOptions(e =>
            {
                ProcessError(e.Exception);
                return Task.CompletedTask;
            })
            {
                MaxConcurrentCalls = 3,
                AutoComplete = true
            };
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"BusListenerService stopping.");
            await _queueClient.CloseAsync();
        }
    }
}