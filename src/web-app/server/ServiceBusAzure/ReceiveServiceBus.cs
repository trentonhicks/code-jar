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
using System.Data.SqlClient;
using CodeJar.Infrastructure;
using System.Collections.Generic;
using System.IO;
using CodeFlip.CodeJar.Api;

namespace CodeJar.ServiceBusAzure
{
    public class ReceiveServiceBus : IHostedService
    {
        private IQueueClient _queueClient;
        const string QueueName = "codejar";
        const string ConnectionString = "Endpoint=sb://codefliptodo.servicebus.windows.net/;SharedAccessKeyName=web-app;SharedAccessKey=x9SEbxQ1AlykQv+ygjDh7hlVup1ZAOZkRTrhkuDHgJA=";
        
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;

        public ReceiveServiceBus(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<ReceiveServiceBus>();
            _loggerFactory = loggerFactory;
            _configuration = configuration;
        }

        public async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var data = Encoding.UTF8.GetString(message.Body);

            var codeRepository = new AdoCodeRepository(new SqlConnection(_configuration.GetConnectionString("Storage")));

            var batch = JsonConvert.DeserializeObject<Batch>(data);
            
            var reader = new CloudReader(_configuration.GetSection("File")["SeedBlobUrl"], new SqlConnection(_configuration.GetConnectionString("Storage")));

            var codes = batch.GenerateCodes(reader, DateTime.Now, _configuration.GetSection("Base26")["alphabet"]);

            await codeRepository.AddCodesAsync(codes);
        }

        protected void ProcessError(Exception e)
        {
            _logger.LogError(e, "Error while processing queue item in BusListenerService.");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _queueClient = new QueueClient(ConnectionString, QueueName);

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