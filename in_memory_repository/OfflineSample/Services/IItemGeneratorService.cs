using System.Threading.Tasks;

namespace OfflineSample.Services
{
    public interface IItemGeneratorService
    {
        Task GenerateUserAsync(int itemCount = 0);
        Task GenerateUsersAsync(int userCount = 1, int orderCount = 0);
    }
}