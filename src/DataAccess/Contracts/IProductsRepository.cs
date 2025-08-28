using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

public interface IProductsRepository : IBaseRepository<Products>
{
    Products GetByName(string name);
    IEnumerable<Products> GetActiveProducts();
}
