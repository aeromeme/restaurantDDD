using Application.UseCase.CategoryCase;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly GetAllCategories _getAllCategories;
        public CategoryController(ILogger<CategoryController> logger, GetAllCategories getAllCategories)
        {
            _logger = logger;
            _getAllCategories = getAllCategories;
        }
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Category>),StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _getAllCategories.ExecuteAsync();
            return Ok(categories);

        }
    }
}
