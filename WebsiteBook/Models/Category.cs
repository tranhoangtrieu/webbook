using System.ComponentModel.DataAnnotations;

namespace WebsiteBook.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public required string Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}
