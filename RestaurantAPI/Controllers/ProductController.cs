using Application.DTOs;
using Application.UseCase.CategoryCase;
using Application.UseCase.ProductCase;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ProductService _productService;
        public ProductController(ILogger<ProductController> logger, ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            var products = await _productService.GetAll();
            return Ok(products.Data);
        }

        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCategories(Guid Id)
        {
            var productId = new ProductId(Id);   
            var products = await _productService.GetById(productId);
            return Ok(products.Data);

        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromBody] ProductCreateDto dto)
        {
            var result = await _productService.AddEntity(dto);
            return Ok(result.Data);
        }

        // --- Added Pagination Endpoint ---
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedProductListDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string ? searchTerm = null,
            [FromQuery] string ? sortBy = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _productService.GetPaged(searchTerm,sortBy,pageNumber, pageSize);
            return Ok(result.Data);
        }
    }
}
