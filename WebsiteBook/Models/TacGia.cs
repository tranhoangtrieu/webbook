using System.ComponentModel.DataAnnotations;

namespace WebsiteBook.Models
{
    public class TacGia
    {

        public int Id { get; set; }
        public string? Name { get; set; }

        public string Phone { get; set; }
        public string DiaChi { get; set; }

        public List<Product>? Products { get; set; }

    }
}
