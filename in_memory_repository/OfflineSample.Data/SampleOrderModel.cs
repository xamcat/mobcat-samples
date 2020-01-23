using Microsoft.MobCAT.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace OfflineSample.Data
{
    /// <summary>
    /// Sample Order Item a User has made.
    /// </summary>
    public class SampleOrderModel : BaseModel
    {
        public string UserId { get; set; }

        public string Description { get; set; }
    }
}
