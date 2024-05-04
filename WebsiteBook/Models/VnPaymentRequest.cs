using System.ComponentModel.DataAnnotations;

namespace WebsiteBook.Models
{
    public class VnPaymentRequest
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public string FullName { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
