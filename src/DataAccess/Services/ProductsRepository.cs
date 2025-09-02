using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess.Context;


namespace Marketplace.SaaS.Accelerator.DataAccess.Repositories;

public class ProductsRepository : IProductsRepository
{
    private readonly SaasKitContext _context;

    public ProductsRepository(SaasKitContext context)
    {
        _context = context;
    }

    public Products Get(int productId)
    {
        return _context.Products.FirstOrDefault(p => p.ProductID == productId);
    }

    public IEnumerable<Products> Get()
    {
        return _context.Products.ToList();
    }

    public int Save(Products product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
        return product.ProductID;
    }

    public Products GetByName(string name)
    {
        return _context.Products.FirstOrDefault(p => p.ProductName == name);
    }

    public IEnumerable<Products> GetActiveProducts()
    {
        return _context.Products.ToList();
    }

    public void Remove(Products entity)
    {
        _context.Products.Remove(entity);
        _context.SaveChanges();
    }

}
