using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts() 
        {
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
                return NotFound("No Products Detected in Db");

            var (_, list) = ProductConversion.FromEntity(null!,products);
            return list.Any() ? Ok(list) : NotFound("No Product Found");
        }  
    }
}
