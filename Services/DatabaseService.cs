using SQLite;
using Unutmaz.Models;

namespace Unutmaz.Services
{
    public class DatabaseService
    {
        public SQLiteAsyncConnection Database { get; }

        public DatabaseService()
        {
            Database = new SQLiteAsyncConnection(DatabaseConfig.DatabasePath);

            // Tablolar oluşturuluyor
            Database.CreateTableAsync<ShoppingCategoryModel>().Wait();
            Database.CreateTableAsync<ShoppingProductModel>().Wait();
            Database.CreateTableAsync<KisiselAlisverisListesiModel>().Wait();
            Database.CreateTableAsync<KisiselAlisverisArsivModel>().Wait();

        }
    }
}
