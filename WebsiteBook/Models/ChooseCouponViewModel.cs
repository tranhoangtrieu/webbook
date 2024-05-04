namespace WebsiteBook.Models
{
    public class ChooseCouponViewModel
    {
        public IEnumerable<Coupon> Coupons { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public Dictionary<int, Coupon> SelectedCoupons { get; set; }
    }
}
