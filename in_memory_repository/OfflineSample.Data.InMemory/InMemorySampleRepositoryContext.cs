using Microsoft.MobCAT.Repository.InMemory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace OfflineSample.Data.InMemory
{
    public class InMemorySampleRepositoryContext : BaseInMemoryRepositoryContext, IOfflineSampleRepositoryContext
    {
        private Lazy<InMemorySampleUserRepository> _inMemorySampleUserRepository = new Lazy<InMemorySampleUserRepository>();

        private Lazy<InMemorySampleOrderRepository> _inMemorySampleOrderRepository = new Lazy<InMemorySampleOrderRepository>();

        public Task<IEnumerable<SampleUserModel>> GetUsersAsync()
            => _inMemorySampleUserRepository.Value.GetAsync();

        public Task<IEnumerable<SampleOrderModel>> GetOrdersForUserIdAsync(string userId)
            => _inMemorySampleOrderRepository.Value.GetOrdersAsync(userId);

        public Task<IEnumerable<SampleUserModel>> GetUsersWithMinimumOrderCountAsync(int minimumOrderCount) =>
            _inMemorySampleOrderRepository.Value
                       .GetOrdersWithMinimumCountAsync(minimumOrderCount)
                       .ContinueWith(task =>
                           _inMemorySampleUserRepository.Value.GetUsersFromIds(task.Result
                               .Select(b => b.UserId)
                               .Distinct()))
                                    .Unwrap();
        //Use ContinueWith and Unwrap to avoid multiple awaits: https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskextensions.unwrap?view=netframework-4.8


        public Task InsertUserAsync(SampleUserModel sampleUser) => _inMemorySampleUserRepository.Value.InsertItemAsync(sampleUser);

        public Task InsertUsersAsync(IEnumerable<SampleUserModel> sampleUsers) => _inMemorySampleUserRepository.Value.InsertAsync(sampleUsers);

        public Task InsertOrdersAsync(IEnumerable<SampleOrderModel> sampleOrders) => _inMemorySampleOrderRepository.Value.InsertAsync(sampleOrders);
    }
}
