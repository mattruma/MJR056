using ClassLibrary1;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ClassLibray1.Tests
{
    [ExcludeFromCodeCoverage]
    public class ToDoDataStoreTests : BaseTests
    {
        private readonly ToDoDataStore _toDoDataStore;
        private readonly Container _toDoDataContainer;

        public ToDoDataStoreTests()
        {
            _toDoDataStore =
                new ToDoDataStore(
                    _cosmosClient,
                    _configuration["COSMOSDB_DATABASEID"]);

            _toDoDataContainer =
                _cosmosClient
                    .GetDatabase(_configuration["COSMOSDB_DATABASEID"])
                    .GetContainer("toDos");
        }

        [Fact]
        public async Task When_AddAsync()
        {
            // Arrange

            var toDoData =
                new ToDoData
                {
                    Description = _faker.Lorem.Paragraph(1)
                };

            // Action

            await _toDoDataStore.AddAsync(
                toDoData);

            // Assert

            var toDoDataItemResponse =
                await _toDoDataContainer.ReadItemAsync<ToDoData>(
                    toDoData.Id,
                    new PartitionKey(toDoData.Id));

            toDoDataItemResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task When_DeleteByIdAsync()
        {
            // Arrange

            var toDoData =
                new ToDoData
                {
                    Description = _faker.Lorem.Paragraph(1)
                };

            await _toDoDataContainer.CreateItemAsync(
                toDoData,
                new PartitionKey(toDoData.Id));

            // Action

            await _toDoDataStore.DeleteByIdAsync(
                toDoData.Id);

            // Assert

            Func<Task> action = async () =>
                await _toDoDataContainer.ReadItemAsync<ToDoData>(
                    toDoData.Id,
                    new PartitionKey(toDoData.Id));

            action.Should().Throw<CosmosException>()
                .And.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task When_GetByIdAsync()
        {
            // Arrange

            var toDoData =
                new ToDoData
                {
                    Description = _faker.Lorem.Paragraph(1)
                };

            await _toDoDataContainer.CreateItemAsync(
                toDoData,
                new PartitionKey(toDoData.Id));

            // Action

            toDoData =
                await _toDoDataStore.GetByIdAsync(
                    toDoData.Id);

            // Assert

            toDoData.Should().NotBeNull();
        }

        [Fact]
        public async Task When_ListAsync()
        {
            // Arrange


            for (var index = 0; index < 4; index++)
            {
                var toDoData =
                    new ToDoData
                    {
                        Description = _faker.Lorem.Paragraph(1)
                    };

                await _toDoDataContainer.CreateItemAsync(
                    toDoData,
                    new PartitionKey(toDoData.Id));
            }

            // Action

            var toDoDataList =
                await _toDoDataStore.ListAsync();

            // Assert

            toDoDataList.Should().NotBeNull();
            toDoDataList.Count().Should().BeGreaterThan(3);
        }

        [Fact]
        public async Task When_UpdateByIdAsync()
        {
            // Arrange

            var toDoData =
                new ToDoData
                {
                    Description = _faker.Lorem.Paragraph(1)
                };

            toDoData.ToDoId =
                toDoData.Id;

            await _toDoDataContainer.CreateItemAsync(
                toDoData,
                new PartitionKey(toDoData.Id));

            toDoData.Description = _faker.Lorem.Paragraph(1);

            // Action

            await _toDoDataStore.UpdateByIdAsync(
                toDoData.Id,
                toDoData);

            // Assert

            toDoData =
                await _toDoDataContainer.ReadItemAsync<ToDoData>(
                    toDoData.Id,
                    new PartitionKey(toDoData.Id));

            toDoData.Should().NotBeNull();
        }
    }
}
