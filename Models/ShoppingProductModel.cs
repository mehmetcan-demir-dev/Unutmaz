using SQLite;

namespace Unutmaz.Models
{
    public class ShoppingProductModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int CategoryId { get; set; } // Bu alan ürünün kategorisini tutacak

        public string ProductName { get; set; }
    }
}
