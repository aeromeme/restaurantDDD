using Application.DTOs;
using Application.Mappers;
using Domain.Entities;
using Domain.Ports;
using Domain.ValueObjects;
using Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.ProductCase
{
    public class ProductService 
    {
        public ICategoryRepository _categoryRepository;
        public IProductRepository _productRepository;
        public IUnitOfWork _unitOfWork;
        public ProductService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
        }
        public async Task<IResult<IReadOnlyList<ProductDto>>> GetAll()
        {
            var data = await _productRepository.GetAllAsync();
            var products = data.Select(ProductMapper.ToDto).ToList();
            return Result<IReadOnlyList<ProductDto>>.Ok(products);
        }

        public async Task<IResult<ProductDto?>> GetById(ProductId id)
        {
            var data= await _productRepository.GetByIdAsync(id);
            if (data == null) return Result<ProductDto?>.Fail("Product not found");
            var product = ProductMapper.ToDto( data);
            return Result<ProductDto?>.Ok(product);
        }
        public async Task<IResult<PagedProductListDto>> GetPaged (string? searchTerm, string? sortBy, int pageNumber, int pageSize)
        {
            var (totalItems, products) = await _productRepository.GetPaged(searchTerm, sortBy, pageNumber, pageSize);

            var productDtos = products.Select(ProductMapper.ToDto).ToList();

            var pagedResult = new PagedProductListDto
            {
                Items = productDtos,
                TotalCount = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedProductListDto>.Ok(pagedResult);
        }

        public async Task<IResult<ProductDto>> AddEntity(ProductCreateDto dto)
        {
            var categoryId = new CategoryId(dto.CategoryId);
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
                return Result<ProductDto>.Fail("Category not found");

            var resultProduct = Product.Create(dto.Name, new Money(dto.Price, "USD"), dto.Stock, category);
            if (!resultProduct.Success || resultProduct.Data == null)
                return Result<ProductDto>.Fail(resultProduct.Message ?? "Product creation failed");

            var product = resultProduct.Data;
            _productRepository.Add(product);
            await _unitOfWork.SavesChangesAsync();
            return Result<ProductDto>.Ok(ProductMapper.ToDto(product), "Product created successfully");
        }

        public Task DeleteEntity(ProductId id)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult<ProductDto>> UpdateEntity(ProductUpdateDto dto)
        {
            var productId = new ProductId(dto.ProductId);
            var existingProduct = await _productRepository.GetByIdAsync(productId);
            if (existingProduct == null)
                return Result<ProductDto>.Fail("Product not found");

            // Update category if changed
            if (existingProduct.Category.Id.Value != dto.CategoryId)
            {
                var categoryId = new CategoryId(dto.CategoryId);
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                    return Result<ProductDto>.Fail("Category not found");
                var categoryResult = existingProduct.ChangeCategory(category);
                if (!categoryResult.Success)
                    return Result<ProductDto>.Fail(categoryResult.Message ?? "Failed to change category");
            }

            // Update name if changed
            if (!string.Equals(existingProduct.Name, dto.Name, StringComparison.Ordinal))
            {
                var nameResult = existingProduct.ChangeName(dto.Name);
                if (!nameResult.Success)
                    return Result<ProductDto>.Fail(nameResult.Message ?? "Failed to change name");
            }

            // Update price if changed
            if (existingProduct.Price.Amount != dto.Price)
            {
                var priceResult = existingProduct.ChangePrice(new Money(dto.Price, "USD"));
                if (!priceResult.Success)
                    return Result<ProductDto>.Fail(priceResult.Message ?? "Failed to change price");
            }

            // Update stock if changed
            if (existingProduct.Stock != dto.Stock)
            {
                var diff = dto.Stock - existingProduct.Stock;
                Result stockResult;
                if (diff > 0)
                    stockResult = existingProduct.IncreaseStock(diff);
                else
                    stockResult = existingProduct.ReduceStock(-diff);

                if (!stockResult.Success)
                    return Result<ProductDto>.Fail(stockResult.Message ?? "Failed to update stock");
            }

            _productRepository.Update(existingProduct);
            await _unitOfWork.SavesChangesAsync();
            return Result<ProductDto>.Ok(ProductMapper.ToDto(existingProduct), "Product updated successfully");
        }

       
        public Task DeleteEntity<TId>(TId id)
        {
            throw new NotImplementedException();
        }
    }
}
