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
        //This is where the implementations live

        //Rationale for pattern of exposing high level methods instead of expressions
        // - no dependencies/chains between abstract and concrete model
        // - No way to convert expression types between models
        // - Specifics about the underlying details (type and mechanism) are encapsulated by Context. - code perspective
        // - High level abstraction reduces complexity for consumer. They just need to understand the intent. - consumer perspective
        // - Ultimately the semantics of query is up to the underlying storage (better separation of concerns.) - storage developer perspective

        // - https://github.com/xamcat/mobcat-library/blob/master/docs/repository.md
        // Update image for sample implementation.


        public Task<IEnumerable<SampleUserModel>> GetUsersAsync()
            => _inMemorySampleUserRepository.Value.GetAsync();

        public Task<IEnumerable<SampleOrderModel>> GetOrdersForUserIdAsync(string userId)
            => _inMemorySampleOrderRepository.Value.GetOrdersAsync(userId);

        public Task<IEnumerable<SampleUserModel>> GetUsersWithMinimumOrderCountAsync(int minimumOrderCount) =>
            _inMemorySampleOrderRepository.Value
                       .GetOrdersWithMinimumCountAsync(minimumOrderCount)
                       .ContinueWith(task =>
                           _inMemorySampleUserRepository.Value.GetUsersFromIds(task.Result
                               .GroupBy(a => a.UserId)
                               .Select(b => b.Key)))
                                    .Unwrap();
        //Use ContinueWith and Unwrap to avoid multiple awaits: https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskextensions.unwrap?view=netframework-4.8


        public Task InsertUserAsync(SampleUserModel sampleUser) => _inMemorySampleUserRepository.Value.InsertItemAsync(sampleUser);

        public Task InsertUsersAsync(IEnumerable<SampleUserModel> sampleUsers) => _inMemorySampleUserRepository.Value.InsertAsync(sampleUsers);

        public Task InsertOrdersAsync(IEnumerable<SampleOrderModel> sampleOrders) => _inMemorySampleOrderRepository.Value.InsertAsync(sampleOrders);
        

        //var validOrders = await _inMemorySampleOrderRepository.Value.GetOrdersWithMinimumCountAsync(minimumOrderCount);
        //var userIds = validOrders.GroupBy(a => a.UserId).Select(b => b.Key);
        //return await _inMemorySampleUserRepository.Value.GetUsersFromIds(userIds);
    }
}
