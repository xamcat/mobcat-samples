using Microsoft.MobCAT.Repository;
using System;

namespace OfflineSample.Data
{
    public class SampleModel : BaseModel
    {
        public string Text { get; set; }
        public string Description { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}