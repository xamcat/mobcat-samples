using Microsoft.MobCAT.Repository.InMemory;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OfflineSample.Data.InMemory
{
    public class InMemorySampleUserRepository : BaseInMemoryRepository<SampleUserModel, InMemorySampleUserModel>, IOfflineSampleRepository<SampleUserModel>
    {
        public Task<IEnumerable<SampleUserModel>> ExecuteTableQueryAsync(Expression<Func<SampleUserModel, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

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
