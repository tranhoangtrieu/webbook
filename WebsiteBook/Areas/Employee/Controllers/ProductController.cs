using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteBook.Repositories;
using WebsiteBook.Models;

namespace WebsiteBook.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Employee")]

    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public IActionResult Index()
        {
            return View();
        }
    }
}
