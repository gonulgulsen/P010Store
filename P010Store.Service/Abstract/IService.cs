using P010Store.Data.Abstract;
using P010Store.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P010Store.Service.Abstract
{
    public interface IService<T> : IRepository<T> where T : class, IEntity,new()
    {

    }
}
