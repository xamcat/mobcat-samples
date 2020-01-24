using Microsoft.MobCAT.Repository.Abstractions;
using Microsoft.MobCAT.Repository.InMemory;
using OfflineSample.Data.InMemory.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OfflineSample.Data.InMemory.Helpers;

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

        public async Task<IEnumerable<SampleOrderModel>> ExecuteTableQueryAsync(Expression<Func<SampleOrderModel, bool>> expression = null)
        {
            var convertedExpression = expression.Convert<InMemorySampleOrderModel, SampleOrderModel, bool>();
            var repositoryTypeItems = await ExecuteTableQueryAsync(convertedExpression);
            return repositoryTypeItems?.Select(a => ToModelType(a));
        }
    }
}
