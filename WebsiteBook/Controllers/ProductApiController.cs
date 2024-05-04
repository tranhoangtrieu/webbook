using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebsiteBook.Models;
using WebsiteBook.Repositories;

namespace WebsiteBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductApiController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("getall", Name = "GetAllProducts")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                // Handle exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("getproductbyid/{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                    return NotFound();
                return Ok(product);
            }
            catch (Exception ex)
            {
                // Handle exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("add", Name = "AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            try
            {
                // Kiểm tra nếu trường product là null
                if (product == null)
                {
                    return BadRequest("Product data is missing");
                }

                // Kiểm tra xem productId có hợp lệ không
                if (product.Id != 0)
                {
                    return BadRequest("Product Id should be 0 for new product");
                }

                // Thêm sản phẩm vào repository
                await _productRepository.AddAsync(product);

                // Trả về thông tin sản phẩm vừa được thêm vào
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về lỗi 500 nếu có lỗi xảy ra
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [HttpPut("update/{id}", Name = "UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            try
            {
                if (id != product.Id)
                    return BadRequest();

                await _productRepository.UpdateAsync(product);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Handle exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("delete/{id}", Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Handle exception
                return StatusCode(500, "Internal server error");
            }
        }

    }
}