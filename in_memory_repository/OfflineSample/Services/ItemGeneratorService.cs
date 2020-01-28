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
        private IOfflineSampleRepositoryContext OfflineSampleRepositoryContext => DependencyService.Get<IOfflineSampleRepositoryContext>();

        public Task GenerateUserAsync(int itemCount = 0)
        {
            var newUser = CreateUser();

            return OfflineSampleRepositoryContext.InsertUserAsync(newUser).ContinueWith(task =>
            {
                if (itemCount > 0)
                {
                    var newOrders = Enumerable.Repeat(newUser.Id, itemCount).Select(userId => GenerateOrder(userId));
                    return OfflineSampleRepositoryContext.InsertOrdersAsync(newOrders);
                }
                return Task.CompletedTask;
            }).Unwrap();
        }

        public Task GenerateUsersAsync(int userCount = 1, int orderCount = 0)
        {
            var newUsers = Enumerable.Repeat(this, userCount).Select(_ => CreateUser()).ToList(); //ToList to avoid deferred execution on the new users.
            return OfflineSampleRepositoryContext.InsertUsersAsync(newUsers).ContinueWith(task =>
            {
                if (orderCount > 0)
                {
                    var newOrders = newUsers.SelectMany(a => Enumerable.Repeat(a.Id, orderCount).Select(userId => GenerateOrder(userId)));
                    return OfflineSampleRepositoryContext.InsertOrdersAsync(newOrders);
                }
                return Task.CompletedTask;
            }).Unwrap();
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
                Description = $"Order - {orderId}"
            };

            return newOrder;
        }
    }
}
