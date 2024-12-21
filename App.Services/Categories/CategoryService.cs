using App.Repositories;
using App.Repositories.Categories;
using App.Services.Categories.Create;
using App.Services.Categories.Update;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace App.Services.Categories
{
    public class CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
    {
        
        public async Task<ServiceResult<int>> Create(CreateCategoryRequest request)
        {
            var anyCategory = await categoryRepository.Where(x => x.Name == request.Name).AnyAsync();

            if (anyCategory) {
                return ServiceResult<int>.Fail("Category ismi veritabanında bulunmaktadır", HttpStatusCode.NotFound);
            }


            var newCategory = new Category { Name = request.Name };

            await categoryRepository.AddAsync(newCategory);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult<int>.Success(newCategory.Id);
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request)
        {
            var category = await categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return ServiceResult.Fail("Category not found", HttpStatusCode.NotFound);
            }

            var isCategoryNameExist = await categoryRepository.Where(x => x.Name == request.Name && x.Id == category.Id).AnyAsync();

            if (isCategoryNameExist)
            {
                return ServiceResult.Fail("kategori ismi veri tabanında bulunmaktadır", HttpStatusCode.BadRequest);
            }
                
            category = mapper.Map(request, category);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.OK);
        }

        public async Task<ServiceResult> DeleteAsync (int id)
    }
}
