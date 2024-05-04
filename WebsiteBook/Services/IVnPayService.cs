using WebsiteBook.Models;

namespace WebsiteBook.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequest model);
        VnPaymentResponse PaymentExecute(IQueryCollection collections);
    }
}
