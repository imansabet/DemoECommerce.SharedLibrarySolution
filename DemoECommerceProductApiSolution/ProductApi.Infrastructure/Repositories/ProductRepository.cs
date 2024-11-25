using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories;

internal class ProductRepository(ProductDbContext context) : IProduct
{
    public async Task<Response> CreateAsync(Product entity)
    {
        try
        {
            // check if this product exist
            var getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
            if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
            {
                return new Response(false, $"{entity.Name} Already Added");
            }
            var currentEntity = context.Products.Add(entity).Entity;
            await context.SaveChangesAsync();

            if (currentEntity is not null && currentEntity.Id > 0)
            {
                return new Response(true, $"{entity.Name} Added Successfully");

            }
            else
            {
                return new Response(false, $"Error Occurred When Adding {entity.Name} .");

            }

        }
        catch (Exception ex)
        {
            //Log Original Exception
            LogException.LogExceptions(ex);
            //display message to client
            return await Task.FromResult(new Response(false, "Error Occurred When Adding New Product."));
        }
    }

    public async Task<Response> DeleteAsync(Product entity)
    {
        try
        {
            var product = await FindByIdAsync(entity.Id);
            if (product is null)
                return new Response(false, $"{entity.Name} was not found .");

            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return new Response(true, $"{entity.Name} Deleted .");

        }
        catch (Exception ex)
        {
            //Log Original Exception
            LogException.LogExceptions(ex);
            //display message to client
            return await Task.FromResult(new Response(false, "Error Occurred When Deleting New Product."));
        }
    }

    public async Task<Product> FindByIdAsync(int id)
    {
        try
        {
            var product = await context.Products.FindAsync(id);
            //                condition? value_if_true : value_if_false
            return product is not null ? product : null!;
        }
        catch (Exception ex)
        {
            //Log Original Exception
            LogException.LogExceptions(ex);
            //display message to client
            throw new Exception("Error Occured When Retrieving Product ");
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            var products = await context.Products.AsNoTracking().ToListAsync();
            //condition? value_if_true : value_if_false
            return products is not null ? products : null!;
        }
        catch (Exception ex)
        {
            //Log Original Exception
            LogException.LogExceptions(ex);
            //display message to client
            throw new InvalidOperationException("Error Occured When Retrieving Products");
        }
    }

    public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
    {

        try
        {
            var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
            return product is not null ? product : null!;
        }
        catch (Exception ex)
        {
            //Log Original Exception
            LogException.LogExceptions(ex);
            //display message to client
            throw new InvalidOperationException("Error Occured When Retrieving Product");
        }
    }

    public async Task<Response> UpdateAsync(Product entity)
    {
        try
        {
            var product = await FindByIdAsync(entity.Id);
            if(product is null)
            {
                return new Response(false, $"{entity.Name} was not found .");
            }
            context.Entry(product).State = EntityState.Detached;
            context.Products.Update(entity);
            context.SaveChangesAsync();
            return new Response(true, $"{entity.Name} Updated Successfully");

        }
        catch (Exception ex)
        {
            //Log Original Exception
            LogException.LogExceptions(ex);
            //Display message to client
            return await Task.FromResult(new Response(false, "Error Occurred When Updating Product."));
        }
    }
}