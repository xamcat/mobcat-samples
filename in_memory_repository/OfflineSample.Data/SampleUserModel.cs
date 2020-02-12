using Microsoft.MobCAT.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace OfflineSample.Data
{
    /// <summary>
    /// Sample Users can create Sample Orders
    /// </summary>
    public class SampleUserModel : BaseModel
    {
        public string Name { get; set; }
    }
}
