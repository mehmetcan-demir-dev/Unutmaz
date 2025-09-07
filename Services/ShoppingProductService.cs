using SQLite;
using Unutmaz.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unutmaz.Services
{
    public class ShoppingProductService
    {
        private readonly SQLiteAsyncConnection _database;

        public ShoppingProductService(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        public async Task<List<ShoppingProductModel>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _database.Table<ShoppingProductModel>()
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<int> AddProductAsync(ShoppingProductModel product)
        {
            return await _database.InsertAsync(product);
        }

        public async Task<int> DeleteProductAsync(ShoppingProductModel product)
        {
            return await _database.DeleteAsync(product);
        }

        // İstersen ürün güncelleme metodu da ekleyebiliriz, sonra yaparız.
    }
}
