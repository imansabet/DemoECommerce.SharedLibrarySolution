using ProductApi.Domain.Entities;
using System.Linq;

namespace ProductApi.Application.DTOs.Conversions;

public static class ProductConversion
{
    public static Product ToEntity(ProductDTO product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        Quantity = product.Quantity
    };
    
    public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEntity(Product product,IEnumerable<Product>? products) 
    {
        //return single
        if(product is not null || products is  null) 
        {
            var singleProduct = new ProductDTO
                (
                    product!.Id,
                    product!.Name,
                    product!.Quantity,
                    (int)product!.Price
                );
            return (singleProduct, null);
        }
        //return list
        if (products is not null || product is not null)
        {
            var _products = products.Select(p =>
                new ProductDTO(p.Id,p.Name!,p.Quantity, (int)p.Price)).ToList();
            return (null, _products);
        }
        return (null, null);

    }


}
