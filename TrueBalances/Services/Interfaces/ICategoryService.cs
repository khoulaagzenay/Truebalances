﻿using TrueBalances.Models;

namespace TrueBalances.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<int> AddCategoryAsync(Category category);
        Task<int> UpdateCategoryAsync(Category category);
        Task<int> DeleteCategoryAsync(int id);
        Task<Category?> GetCategoryWithExpensesByIdAsync(int id, int groupId);
    }
}
