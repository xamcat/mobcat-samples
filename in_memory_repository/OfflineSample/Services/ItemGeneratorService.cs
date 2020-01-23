using OfflineSample.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OfflineSample.Services
{
    public class ItemGeneratorService : IItemGeneratorService
    {
        private IOfflineSampleRepository<SampleUserModel> SampleUserStore => DependencyService.Get<IOfflineSampleRepository<SampleUserModel>>();

        private IOfflineSampleRepository<SampleOrderModel> SampleOrderStore => DependencyService.Get<IOfflineSampleRepository<SampleOrderModel>>();

        public async Task GenerateUsersAsync(int userCount = 1, int orderCount = 0)
        {
            var newUsers = Enumerable.Repeat(this, userCount).Select(_ => CreateUser());
            await SampleUserStore.InsertAsync(newUsers);

            if (orderCount > 0)
            {
                var newOrders = newUsers.SelectMany(a => Enumerable.Repeat(a.Id, orderCount).Select(userId => GenerateOrder(userId)));
                await SampleOrderStore.InsertAsync(newOrders);
            }
        }

        public async Task GenerateUserAsync(int itemCount = 0)
        {
            var newUser = CreateUser();

            await SampleUserStore.InsertItemAsync(newUser);

            if (itemCount > 0)
            {
                var newOrders = Enumerable.Repeat(newUser.Id, itemCount).Select(userId => GenerateOrder(userId));
                await SampleOrderStore.InsertAsync(newOrders);
            }
        }

        private SampleUserModel CreateUser()
        {
            var userId = Guid.NewGuid().ToString();
            return new SampleUserModel
            {
                Id = userId,
                Name = $"UserName: {userId}"
            };
        }

        private SampleOrderModel GenerateOrder(string userId)
        {
            var orderId = Guid.NewGuid().ToString();
            var newOrder = new SampleOrderModel
            {
                Id = orderId,
                UserId = userId,
                Description = $"Order - {orderId} | by {userId}"
            };

            return newOrder;
        }
    }
}
