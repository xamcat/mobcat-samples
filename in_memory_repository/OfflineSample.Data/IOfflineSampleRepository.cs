using Microsoft.MobCAT.Repository;
using Microsoft.MobCAT.Repository.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OfflineSample.Data
{
    public interface IOfflineSampleRepository<T> : IBaseRepository<T> where T : BaseModel
    { }
}
