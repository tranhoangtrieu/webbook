using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBook.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
       
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public List<ProductImage>? Images { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? ImageUrl { get; set; } // Đường dẫn đến hình ảnh đại diện
        public List<string>? ImageUrls { get; set; } // Danh sách các hình ảnh khác
        public bool IsDetactive { get; set; }

        [ForeignKey("TacGia")]
        public int TacGiaId { get; set; }
        public TacGia? TacGia { get; set; }
    }

    public class ProductImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
