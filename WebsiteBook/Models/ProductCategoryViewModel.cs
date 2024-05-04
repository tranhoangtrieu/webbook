using WebsiteBook.Repositories;

namespace WebsiteBook.Models
{
    public class ProductCategoryViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Coupon> Coupons { get; set; }

        public IEnumerable<TacGia> TacGia { get; set; }

     
    }
}
