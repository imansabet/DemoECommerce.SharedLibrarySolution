using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController(IOrderService orderService,IOrder orderInterface) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders() 
    {
        var orders = await orderInterface.GetAllAsync();
        if (!orders.Any())
            return NotFound("No Order Detected in Db.");

        var (_, list) = OrderConversion.FromEntity(null, orders);
        return !list!.Any() ? NotFound() : Ok(list);

    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDTO>> GetOrder(int id)
    {
        var order = await orderInterface.FindByIdAsync(id);
        if (order is null)
            return NotFound(null);
        var (_order, _) = OrderConversion.FromEntity(order, null);
        return _order is not null ? Ok(_order) : NotFound("No Order Found");
    }

    [HttpGet("client/{clientId:int}")]
    public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientId)
    {
        if (clientId <= 0) return BadRequest("invalid Data Provided");
        var orders = await orderService.GetOrdersByClientId(clientId);
        return !orders.Any() ? NotFound(null) : Ok(orders);

    }

    [HttpGet("details/{orderId:int}")]
    public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
    {
        if (orderId <= 0) return BadRequest("Invalid Data Provided");
        var orderDetail = await orderService.GetOrderDetails(orderId);
        return orderDetail.OrderId > 0 ? Ok(orderDetail) : NotFound("No Order Found");

    }


    [HttpPost]
    public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest("Incomplete data submitted");

        // convert to entity
        var getEntity = OrderConversion.ToEntity(orderDTO);
        var response = await orderInterface.CreateAsync(getEntity);
        return response.Flag ? Ok(response) : BadRequest(response);
    }
    [HttpPut]
    public async Task<ActionResult<Response>> UpdateOrder(OrderDTO orderDTO)
    { 
        // convert to entity
        var order = OrderConversion.ToEntity(orderDTO);
        var response = await orderInterface.UpdateAsync(order);
        return response.Flag ? Ok(response) : BadRequest(response);
    }
    [HttpDelete]
    public async Task<ActionResult<OrderDTO>> DeleteOrder(OrderDTO orderDTO)
    {
        var order = OrderConversion.ToEntity(orderDTO);
        var response = await orderInterface.DeleteAsync(order);
        return response.Flag ? Ok(response) : BadRequest(response);

    }
}
