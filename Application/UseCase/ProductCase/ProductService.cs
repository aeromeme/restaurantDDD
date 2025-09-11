using Application.DTOs;
using Application.Mappers;
using Domain.Entities;
using Domain.Ports;
using Domain.ValueObjects;
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
            if (data == null) return Result<ProductDto?>.Fail("Product found");
            var product = ProductMapper.ToDto( data);
            return Result<ProductDto?>.Ok(product);
        }

        public async Task<IResult<ProductDto>> AddEntity(ProductCreateDto dto)
        {
            var categoryId = new CategoryId(dto.CategoryId);
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null) throw new ApplicationException("Category not found");
            var product = new Product(dto.Name, new Money(dto.Price, dto.Currency), dto.Stock, category);
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
            var productId = new ProductId(dto.Id);
            var existingProduct = await _productRepository.GetByIdAsync(productId);
            if (existingProduct == null)
                throw new ApplicationException("Product not found");

            // Update category if changed
            if (existingProduct.Category.Id.Value != dto.CategoryId)
            {
                var categoryId = new CategoryId(dto.CategoryId);
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                    throw new ApplicationException("Category not found");
                existingProduct.ChangeCategory(category);
            }

            // Update name if changed
            if (!string.Equals(existingProduct.Name, dto.Name, StringComparison.Ordinal))
                existingProduct.ChangeName(dto.Name);

            // Update price if changed
            if (existingProduct.Price.Amount != dto.Price || existingProduct.Price.Currency != dto.Currency)
                existingProduct.ChangePrice(new Money(dto.Price, dto.Currency));

            // Update stock if changed
            if (existingProduct.Stock != dto.Stock)
            {
                var diff = dto.Stock - existingProduct.Stock;
                if (diff > 0)
                    existingProduct.IncreaseStock(diff);
                else
                    existingProduct.ReduceStock(-diff);
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
