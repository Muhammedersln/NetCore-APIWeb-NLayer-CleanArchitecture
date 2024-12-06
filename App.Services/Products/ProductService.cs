﻿using App.Repositories;
using App.Repositories.Products;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace App.Services.Products
{
    public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork): IProductService
    {
        public async Task<ServiceResult< List<ProductDto>>> GetTopPriceProductsAsync (int count)
        {
            var products = await productRepository.GetTopPriceProductsAsync(count);

            var productsAsDtos = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

            return new ServiceResult<List<ProductDto>> { Data = productsAsDtos };
        }

        public async Task<ServiceResult<List<ProductDto>>> GetAllListAsync()
        {

            var products = await productRepository.GetAll().ToListAsync();

            var productsAsDtos = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

            return new ServiceResult<List<ProductDto>> { Data = productsAsDtos };
         }


        public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber, int pageSize)
        {
            var products = await productRepository.GetAll().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var productsAsDtos = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

            return new ServiceResult<List<ProductDto>> { Data = productsAsDtos };
        }

        public async Task<ServiceResult<ProductDto> > GetByIdAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product is null)
            {
                ServiceResult<ProductDto>.Fail("Product not found", HttpStatusCode.NotFound);
            }

            var productsAsDtos = new ProductDto(product!.Id, product.Name, product.Price, product.Stock);

            return ServiceResult<ProductDto>.Success(productsAsDtos!);
        }

        public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
        {
            var product = new Product { Name = request.Name, Price = request.Price, Stock = request.Stock };

            await productRepository.AddAsync(product);
            await unitOfWork.SaveChangesAsync();
            return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id), $"api/products/{product.Id}");


        }

        public async Task<ServiceResult> UpdateAsync(int Id, UpdateProductRequest request)
        {
            var product = await productRepository.GetByIdAsync(Id);
            if (product is null)
            {
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
            }

            product.Name = request.Name;
            product.Price = request.Price;
            product.Stock = request.Stock;

            productRepository.Update(product);
            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }  
     
        public async Task<ServiceResult> UpdateStockAsync(UpdateProductStcokRequest request)
        {
            var product = await productRepository.GetByIdAsync(request.ProductId);
            if (product is null)
            {
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
            }

            product.Stock = request.Stock;

            productRepository.Update(product);
            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }  // Add this method to the IProductService interface (App.Services.Products/IProductService.cs


        
        public async Task<ServiceResult> DeleteAsync(int Id)
        {
            var product = await productRepository.GetByIdAsync(Id);
            if (product is null)
            {
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
            }

            productRepository.Delete(product);
            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }
    }
}
