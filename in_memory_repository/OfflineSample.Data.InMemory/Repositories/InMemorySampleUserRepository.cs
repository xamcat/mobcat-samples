using Microsoft.MobCAT.Repository.InMemory;
using System;
using System.Collections.Generic;
using System.Text;

namespace OfflineSample.Data.InMemory
{
    public class InMemorySampleUserRepository : BaseInMemoryRepository<SampleUserModel, InMemorySampleUserModel>, IOfflineSampleRepository<SampleUserModel>
    {
        protected override SampleUserModel ToModelType(InMemorySampleUserModel repositoryType) => repositoryType == null ? null : new SampleUserModel
        {
            Id = repositoryType.Id,
            Name = repositoryType.Name
        };

        protected override InMemorySampleUserModel ToRepositoryType(SampleUserModel modelType) => modelType == null ? null : new InMemorySampleUserModel
        {
            Id = modelType.Id,
            Name = modelType.Name
        };
    }
}
