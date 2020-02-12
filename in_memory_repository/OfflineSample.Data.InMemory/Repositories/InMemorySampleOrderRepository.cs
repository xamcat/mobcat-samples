using Microsoft.MobCAT.Repository.InMemory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfflineSample.Data.InMemory
{
    public class InMemorySampleOrderRepository : BaseInMemoryRepository<SampleOrderModel, InMemorySampleOrderModel>, IOfflineSampleRepository<SampleOrderModel>
    {
        protected override SampleOrderModel ToModelType(InMemorySampleOrderModel repositoryType) => repositoryType == null ? null : new SampleOrderModel
        {
            Id = repositoryType.Id,
            UserId = repositoryType.UserId,
            Description = repositoryType.Description
        };

        protected override InMemorySampleOrderModel ToRepositoryType(SampleOrderModel modelType) => modelType == null ? null : new InMemorySampleOrderModel
        {
            Id = modelType.Id,
            UserId = modelType.UserId,
            Description = modelType.Description
        };

        public async Task<IEnumerable<SampleOrderModel>> GetOrdersAsync(string userId)
        {
            var ordersForUser = await ExecuteTableQueryAsync(a => a.UserId == userId);
            return ordersForUser?.Select(a => ToModelType(a));
        }

        public Task<IEnumerable<SampleOrderModel>> GetOrdersWithMinimumCountAsync(int minimumCount) => ExecuteTableQueryAsync<string>(expression: a => true,
                groupingExpression: a => a.UserId).ContinueWith(task => task.Result.Where(a => a.Count() >= minimumCount)
                        .SelectMany(a => a.Select(b => ToModelType(b)))
                );
    }
}