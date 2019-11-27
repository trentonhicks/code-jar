using Microsoft.Extensions.Configuration;
using System;
using System.IO;


namespace _6_Digit_Code_Generator
{
    class Program
    {

        static void Main(string[] args)
        {

            // Link to appsettings.json configuration file
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("Storage");

            SQL sql = new SQL(connectionString);
            CodeGenerator codeGenerator = new CodeGenerator(connectionString);

            // Get all the codes
            // var codeRepository = sql.GetCodes();

            // Generate codes
            codeGenerator.CreateDigitalCode(amount: 50);
        }

    }
}
