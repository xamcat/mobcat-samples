using Microsoft.MobCAT.Repository.Abstractions;
using Microsoft.MobCAT.Repository.InMemory;
using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
