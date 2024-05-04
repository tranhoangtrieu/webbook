﻿using Microsoft.AspNetCore.Identity;

namespace WebsiteBook.Repositories
{
    using System.Collections.Generic;
    using WebsiteBook.Models;
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }
}
