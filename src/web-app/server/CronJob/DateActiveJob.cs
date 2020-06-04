using CodeJar.Domain;
using CodeJar.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TodoWebAPI.CronJob
{
    public class DateActiveJob : CronJobService
    {
        private readonly IConfiguration _configuration;
        private readonly IScheduleConfig<DateActiveJob> config;
        private readonly ILogger<DateActiveJob> _logger;
        public DateActiveJob(
            IConfiguration configuration,
            IScheduleConfig<DateActiveJob> config,
            ILogger<DateActiveJob> logger)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _configuration = configuration;
            this.config = config;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Due Date Starts.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("Storage"));

            var codeRepository = new AdoCodeRepository(connection); 
            var generatedCodes = await codeRepository.GetCodesForActivationAsync(DateTime.Now.Date, _configuration.GetSection("Base26")["alphabet"]);

            foreach(var code in generatedCodes)
                code.Activate(DateTime.Now.Date);

            await codeRepository.UpdateCodesAsync(generatedCodes);

            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} Due Date Job is working.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Due Date Job is Stopping");
            return base.StopAsync(cancellationToken);
        }

        public async Task<int> PageCountAsync(int id)
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("Storage"));

            var pages = 0;

            var pagesRemainder = 0;

            await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT BatchSize FROM Batch WHERE ID = @id";

                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var numberOfCodes = (int)reader["BatchSize"];

                        pages = numberOfCodes / 10;

                        pagesRemainder = numberOfCodes % 10;

                        if (pagesRemainder > 0)
                        {
                            pages++;
                        }
                    }
                }
            }

            await connection.CloseAsync();

            return pages;
        }
    }
}
