using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services;

public class OrderService
    (HttpClient httpClient,
    IOrder orderInterface,
    ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
{

    public async Task<ProductDTO> GetProduct(int productId) 
    {
        // Call Product Api using HttpClient
        // Redirect this Call to the Api Gateway
        var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
        if (!getProduct.IsSuccessStatusCode)
            return null!;
        var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
        return product!;
    }

    public async Task<AppUserDTO> GetUser(int userId)
    {
        // Call Product Api using HttpClient
        // Redirect this Call to the Api Gateway
        var getUser = await httpClient.GetAsync($"/api/users/{userId}");
        if (!getUser.IsSuccessStatusCode)
            return null!;
        var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
        return user!;
    }


    public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
    {
        //Prepare Order
        var order = await orderInterface.FindByIdAsync(orderId);
        if (order is null || order!.Id <= 0)
            return null!;

        //Get Retry pipeline
        var retrypipeLine = resiliencePipeline.GetPipeline("my-retry-pipeline");

        //Prepare Product
        var productDTO = await retrypipeLine.ExecuteAsync(async token => await GetProduct(order.ProductId));

        //Prepare Client
        var appUserDTO = await retrypipeLine.ExecuteAsync(async token => await GetUser(order.ClientId));

        // Populate order details 
        return new OrderDetailsDTO
            (
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.TelephoneNumber,
                productDTO.Name,
                order.PurchaseQuantity,
                productDTO.Price,
                productDTO.Quantity * order.PurchaseQuantity,
                order.OrderedDate
            );
    }

    public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
    {
        // Get all Client's order
        var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
        if(!orders.Any()) return null!;

        // entity  to dto
        var (_, _orders) = OrderConversion.FromEntity(null, orders);
        return _orders!;
    
    }
}
