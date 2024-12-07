using eCommerce.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;

namespace UnitTest.ProductApi.Controllers;

public class ProductControllerTest
{
    private readonly IProduct productInterface;
    private readonly ProductsController productsController;

    public ProductControllerTest()
    {
        productInterface = A.Fake<IProduct>();
        productsController = new ProductsController(productInterface);
    }

    //Get All products
    [Fact]
    public async Task GetProduct_WhenProductExists_ReturnOkResponseWithProducts() 
    {
        //Arange 
        var products = new List<Product>()
        {
            new(){Id=1,Name="Product 1",Quantity=10,Price=100.70m},
            new(){Id=2,Name="Product 2",Quantity=110,Price=1004.70m},
        };

        // Set up Fake Response for GetAll method
        A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

        //Act
        var result = await productsController.GetProducts();

        //Asert 
        var okResult = result.Result as OkObjectResult;

        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var returnedProducts = okResult.Value as IEnumerable<ProductDTO>;
        returnedProducts.Should().NotBeNull();
        returnedProducts.Should().HaveCount(2);
        returnedProducts!.First().Id.Should().Be(1);
        returnedProducts!.Last().Id.Should().Be(2);
    }


    [Fact]
    public async Task GetProduct_WhenNoProductExists_ReturnNotFoundSResponse()
    {
        //Arange 
        var products = new List<Product>();
        

        // Set up Fake Response for GetAll method
        A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

        //Act
        var result = await productsController.GetProducts();

        //Asert 
        var notFoundResult = result.Result as NotFoundObjectResult;

        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var message = notFoundResult.Value as string;
        message.Should().Be("No Products Detected in Db");
    }

    [Fact]
    public async Task CreateProduct_WhenModleStateIsInvalid_ReturnBadRequest()
    {
        //Arange 
        var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
        productsController.ModelState.AddModelError("Name", "Required");

        //Act
        var result = await productsController.CreateProduct(productDTO);

        //Asert 
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

    }


    [Fact]
    public async Task CreateProduct_WhenCreateIsSuccessfull_ReturnOkResponse()
    {
        //Arange 
        var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
        var response = new Response(true, "Created");

        //Act
        A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
        var result = await productsController.CreateProduct(productDTO);

        //Asert 
        var okResult = result.Result as OkObjectResult; ;
        okResult!.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseResult = okResult.Value as Response;
        responseResult!.Message.Should().Be("Created");
        responseResult!.Flag.Should().BeTrue();

    }
}
