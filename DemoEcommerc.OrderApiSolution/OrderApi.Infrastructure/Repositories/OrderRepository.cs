using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace OrderApi.Infrastructure.Repositories;

public class OrderRepository(OrderDbContext context) : IOrder
{
    public async Task<Response> CreateAsync(Order entity)
    {
        try 
        {
            var order = context.Orders.Add(entity).Entity;
            await context.SaveChangesAsync();
            return order.Id > 0 ? new Response(true, "order placed sSuccessfully") :
                new Response(false, "Error Occured While Placing Order");

        }
        catch (Exception ex) 
        {
            //  Log Original Exception
            LogException.LogExceptions(ex);
            return new Response(false, "Error Occured While Placing Order");
        }
    }

    public async Task<Response> DeleteAsync(Order entity)
    {
        try
        {
            var order = await FindByIdAsync(entity.Id);

            if(order is null)
                new Response(false, "Order Not found");

            context.Orders.Remove(entity);
            await context.SaveChangesAsync();
            return new Response(true, "Order Successfully Deleted");
        }
        catch (Exception ex)
        {
            //  Log Original Exception
            LogException.LogExceptions(ex);
            return new Response(false, "Error Occured While Deleting Order");
        }
    }

    public async Task<Order> FindByIdAsync(int id)
    {
        try
        {
            var order = await context.Orders!.FindAsync(id);
            return order is not null ? order : null!; 
        }
        catch (Exception ex)
        {
            //  Log Original Exception
            LogException.LogExceptions(ex);
            throw new Exception("Error Occured While Retrieving Order");
        }
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        try
        {
            var orders = await context.Orders.AsNoTracking().ToListAsync();
            return orders is not null ? orders : null!;
        }
        catch (Exception ex)
        {
            //  Log Original Exception
            LogException.LogExceptions(ex);
            throw new Exception("Error Occured While Retrieving Orders");
        }
    }

    public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {
            var order = await context.Orders.Where(predicate).FirstOrDefaultAsync()!;
            return order is not null ? order : null!;
        }
        catch (Exception ex)
        {
            //  Log Original Exception
            LogException.LogExceptions(ex);
            throw new Exception("Error Occured While Retrieving Order");
        }
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {
            var orders = await context.Orders.Where(predicate).ToListAsync();
            return orders is not null ? orders : null!;
        }
        catch (Exception ex)
        {
            //  Log Original Exception
            LogException.LogExceptions(ex);
            throw new Exception("Error Occured While Retrieving Orders");
        }
    }

    public async Task<Response> UpdateAsync(Order entity)
    {
        try
        {
            var order = await FindByIdAsync(entity.Id);
            if(order is null)
                new Response(false, "Order Not found");

            context.Entry(order).State = EntityState.Detached;
             context.Orders.Update(entity);
            await context.SaveChangesAsync();
            return new Response(true, "Order Updated Successfully");

        }
        catch (Exception ex)
        {
            //  Log Original Exception
            LogException.LogExceptions(ex);
            return new Response(false, "Error Occured While Updating Order");
        }
    }
}
