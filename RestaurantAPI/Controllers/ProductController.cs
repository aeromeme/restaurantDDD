using Application.DTOs;
using Application.UseCase.CategoryCase;
using Application.UseCase.ProductCase;
using Domain.Entities;
using Domain.Results;
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
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<ProductDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<ProductDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var productId = new ProductId(Id);   
            var result = await _productService.GetById(productId);

            if (!result.Success)
            {
                if (result.Message != null && result.Message.ToLower().Contains("not found"))
                    return NotFound(result);

                return BadRequest(result);
            }

            if (result.Data == null)
                return NotFound(Result<ProductDto>.Fail("Product not found"));

            return Ok(result.Data);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<ProductDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<ProductDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Post([FromBody] ProductCreateDto dto)
        {
            var result = await _productService.AddEntity(dto);

            if (!result.Success)
            {
                if (result.Message != null && result.Message.ToLower().Contains("not found"))
                    return NotFound(result);

                return BadRequest(result);
            }

            if (result.Data == null)
                return BadRequest(result);

            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<ProductDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<ProductDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(Guid id, ProductUpdateDto dto)
        {
            if (id != dto.ProductId)
                return BadRequest(Result<ProductDto>.Fail("Product ID mismatch."));

            var result = await _productService.UpdateEntity(dto);
            if (!result.Success)
            {
                if (result.Message.ToLower().Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(result);
                return BadRequest(result);
            }
            var productDto = result.Data;
            return Ok(productDto);
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
