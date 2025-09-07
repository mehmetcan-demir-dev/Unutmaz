using SQLite;
using Unutmaz.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unutmaz.Services
{
    public class ShoppingCategoryService
    {
        private readonly SQLiteAsyncConnection _database;

        public ShoppingCategoryService(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task<List<ShoppingCategoryModel>> GetCategoriesAsync()
        {
            return await _database.Table<ShoppingCategoryModel>().ToListAsync();
        }

        public async Task SeedDefaultCategoriesAsync()
        {
            var defaultCategories = new List<ShoppingCategoryModel>
            {
                new ShoppingCategoryModel { CategoryName = "Gıda" },
                new ShoppingCategoryModel { CategoryName = "Temizlik" },
                new ShoppingCategoryModel { CategoryName = "Elektronik" },
                new ShoppingCategoryModel { CategoryName = "Kıyafet" },
                new ShoppingCategoryModel { CategoryName = "İçecek" },
                new ShoppingCategoryModel { CategoryName = "Kişisel Bakım" },
                new ShoppingCategoryModel { CategoryName = "Ev Aletleri" },
                new ShoppingCategoryModel { CategoryName = "Kırtasiye" },
                new ShoppingCategoryModel { CategoryName = "Evcil Hayvan" },
                new ShoppingCategoryModel { CategoryName = "Bahçe" }
            };

            foreach (var category in defaultCategories)
            {
                var exists = await _database.Table<ShoppingCategoryModel>()
                                            .Where(c => c.CategoryName == category.CategoryName)
                                            .FirstOrDefaultAsync();

                if (exists == null)
                    await _database.InsertAsync(category);
            }
        }
    }
}
