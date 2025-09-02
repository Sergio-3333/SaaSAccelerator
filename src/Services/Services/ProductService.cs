using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Services;

public class ProductService : IProductService
{
    private readonly IProductsRepository productsRepository;

    public ProductService(IProductsRepository productsRepository)
    {
        this.productsRepository = productsRepository;
    }

    public ProductResult GetProductById(int productId)
    {
        var product = productsRepository.Get(productId);
        return product == null ? null : MapToResult(product);
    }

    public IEnumerable<ProductResult> GetAllProducts()
    {
        return productsRepository.Get().Select(MapToResult);
    }

    public int SaveProduct(ProductResult productResult)
    {
        var product = new Products
        {
            ProductID = productResult.ProductID,
            ProductName = productResult.ProductName
        };
        return productsRepository.Save(product);
    }

    private static ProductResult MapToResult(Products product)
    {
        return new ProductResult
        {
            ProductID = product.ProductID,
            ProductName = product.ProductName
        };
    }
}
