using Microsoft.MobCAT.Repository.InMemory;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OfflineSample.Data.InMemory
{
    public class InMemorySampleRepository : BaseInMemoryRepository<SampleModel, InMemorySampleModel>, IOfflineSampleRepository<SampleModel>
    {
        public Task<IEnumerable<SampleModel>> ExecuteTableQueryAsync(Expression<Func<SampleModel, bool>> expression = null)
        {
            throw new NotImplementedException();
        }

        protected override SampleModel ToModelType(InMemorySampleModel repositoryType) => repositoryType == null ? null : new SampleModel
{
            Id = repositoryType.Id,
            Text = repositoryType.Text,
            Timestamp = new DateTimeOffset(repositoryType.TimestampTicks, TimeSpan.Zero)
         };

        protected override InMemorySampleModel ToRepositoryType(SampleModel modelType) => modelType == null ? null : new InMemorySampleModel
        {
            Id = modelType.Id,
            Text = modelType.Text,
            TimestampTicks = modelType.Timestamp.UtcTicks
        };
    }
}
