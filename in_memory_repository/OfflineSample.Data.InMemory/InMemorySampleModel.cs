using Microsoft.MobCAT.Repository.InMemory;
using System;

namespace OfflineSample.Data.InMemory
{
    public class InMemorySampleModel : BaseInMemoryModel
    {
        public string Text { get; set; }
        public string Description { get; set; }
        public long TimestampTicks { get; set; }
    }
}
