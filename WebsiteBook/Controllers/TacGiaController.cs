using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteBook.Models;
using WebsiteBook.Repositories;

namespace WebsiteBook.Controllers
{
    public class TacGiaController : Controller
    {
        private readonly ITacGia _tacGia;

        public TacGiaController(ITacGia tacGia)
        {
            _tacGia = tacGia;
        }


        public async Task<IActionResult> IndexTacGia()
        {
            var Tacgias = await _tacGia.GetAllAsync();
            return View(Tacgias);
        }

        public async Task<IActionResult> AddTacGia(TacGia tacGia)
        {

            if (ModelState.IsValid)
            {
                await _tacGia.AddAsync(tacGia);
                return RedirectToAction(nameof(IndexTacGia)); // Chuyển hướng về trang danh sách danh mục sau khi thêm thành công
            }

            return View(tacGia);
        }

        public async Task<IActionResult> DeleteTacGia(int id)
        {
            var category = await _tacGia.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            else
            {
                await _tacGia.DeleteAsync(id);
                return RedirectToAction(nameof(IndexTacGia));
            }


        }

        public async Task<IActionResult> Update(int id)
        {
            var tacGia = await _tacGia.GetByIdAsync(id);
            if (tacGia == null)
            {
                return NotFound();
            }
            var TacGia = await _tacGia.GetAllAsync();
            ViewBag.TacGia = new SelectList(TacGia, "Id", "Name", tacGia.Id);

            return View(tacGia);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, TacGia tacGia)
        {
            if (id != tacGia.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _tacGia.UpdateAsync(tacGia);  
                return RedirectToAction(nameof(IndexTacGia));
            }
            return View(tacGia);
        }




    }
}
