using Microsoft.MobCAT.Repository.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfflineSample.Data
{
    public interface IOfflineSampleRepositoryContext : IRepositoryContext
    {
        public Task<IEnumerable<SampleUserModel>> GetUsersAsync();

        public Task<IEnumerable<SampleOrderModel>> GetOrdersForUserIdAsync(string userId);

        public Task<IEnumerable<SampleUserModel>> GetUsersWithMinimumOrderCountAsync(int minimumOrderCount);

        public Task InsertUserAsync(SampleUserModel sampleUser);

        public Task InsertUsersAsync(IEnumerable<SampleUserModel> sampleUsers);

        public Task InsertOrdersAsync(IEnumerable<SampleOrderModel> sampleOrders);
    }
}