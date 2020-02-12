using Microsoft.MobCAT.Repository.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<SampleUserModel>> GetUsersFromIds(IEnumerable<string> userIds)
        {
            var inMemoryUsers = await ExecuteTableQueryAsync(a => userIds.Contains(a.Id));
            return inMemoryUsers?.Select(a => ToModelType(a));
        }
    }
}
