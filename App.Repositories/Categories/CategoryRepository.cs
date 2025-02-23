﻿using Microsoft.EntityFrameworkCore;
namespace App.Repositories.Categories
{
    public class CategoryRepository(AppDbContext context) : GenericRepository<Category>(context), ICategoryRepository
    {
        public IQueryable<Category?> GetCategoryByProductsAsync()
        {
            return context.Categories.Include(x => x.Products).AsQueryable();
        }

        public Task<Category?> GetCategoryWithProductsAsync(int id)
        {
           return context.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);

        }

    }
}
