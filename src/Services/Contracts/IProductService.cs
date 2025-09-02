using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

public interface IProductService
{
    ProductResult GetProductById(int productId);
    IEnumerable<ProductResult> GetAllProducts();
    int SaveProduct(ProductResult product);
}
