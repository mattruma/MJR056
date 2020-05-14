using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class ToDoDataStore
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _cosmosContainer;

        public ToDoDataStore(
            CosmosClient cosmosClient,
            string databaseId)
        {
            _cosmosClient = cosmosClient;
            _cosmosContainer = _cosmosClient.GetDatabase(databaseId).GetContainer("toDos");
        }

        public async Task AddAsync(
              ToDoData toDoData)
        {
            await _cosmosContainer.CreateItemAsync(
                toDoData,
                new PartitionKey(toDoData.Id));
        }

        public async Task DeleteByIdAsync(
            string id)
        {
            await _cosmosContainer.DeleteItemAsync<ToDoData>(
                id,
                new PartitionKey(id));
        }

        public async Task<ToDoData> GetByIdAsync(
            string id)
        {
            var itemResponse =
                await _cosmosContainer.ReadItemAsync<ToDoData>(
                    id,
                    new PartitionKey(id));

            return itemResponse.Resource;
        }

        public async Task<IEnumerable<ToDoData>> ListAsync()
        {
            var query = _cosmosContainer
                .GetItemLinqQueryable<ToDoData>()
                .OrderBy(x => x.DueOn);

            var feedIterator =
                query.ToFeedIterator();

            var toDoDataList =
                new List<ToDoData>();

            while (feedIterator.HasMoreResults)
            {
                var feedResponse =
                    await feedIterator.ReadNextAsync();

                toDoDataList.AddRange(
                    feedResponse.Resource);
            }

            return toDoDataList;
        }

        public async Task UpdateByIdAsync(
            string id,
            ToDoData toDoData)
        {
            await _cosmosContainer.ReplaceItemAsync(
                toDoData,
                id,
                new PartitionKey(id));
        }
    }
}
