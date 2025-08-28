using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

public class ProductsService
{
    private readonly IProductsRepository _productsRepository;

    public ProductsService(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }

    public int EnsureProductExists(int productId)
    {
        var product = _productsRepository.Get(productId);

        if (product != null)
        {
            return product.ProductID;
        }

        var newProduct = new Products
        {
            ProductID = productId,
            ProductName = $"Imported from Microsoft - {productId}"
        };

        _productsRepository.Save(newProduct);
        return newProduct.ProductID;
    }
}
