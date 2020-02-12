using Microsoft.MobCAT.Repository.InMemory;

namespace OfflineSample.Data.InMemory
{
    public class InMemorySampleOrderModel : BaseInMemoryModel
    {
        public string UserId { get; set; }

        public string Description { get; set; }
    }
}