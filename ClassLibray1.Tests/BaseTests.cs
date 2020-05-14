using Bogus;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Xunit;

namespace ClassLibray1.Tests
{
    public abstract class BaseTests : IAsyncLifetime
    {
        protected readonly IConfiguration _configuration;
        protected readonly Faker _faker;
        protected readonly CosmosClient _cosmosClient;

        protected BaseTests()
        {
            // NOTE: Make sure to set these files to copy to output directory

            _configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json")
                 .AddJsonFile("appsettings.Development.json")
                 .Build();

            _faker = new Faker();

            _cosmosClient =
                new CosmosClientBuilder(
                    _configuration["COSMOSDB_CONNECTIONSTRING"])
                    .WithConnectionModeDirect()
                    .Build();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            await _cosmosClient.CreateDatabaseIfNotExistsAsync(
                _configuration["COSMOSDB_DATABASEID"]);

            var cosmosDatabase =
                _cosmosClient.GetDatabase(
                    _configuration["COSMOSDB_DATABASEID"]);

            await cosmosDatabase.CreateContainerIfNotExistsAsync(
                new ContainerProperties
                {
                    Id = "toDos",
                    PartitionKeyPath = "/toDoId"
                });
        }
    }
}
