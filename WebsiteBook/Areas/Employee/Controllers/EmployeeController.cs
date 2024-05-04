using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBook.Models;


namespace WebsiteBook.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = SD.Role_Employee)]
    public class EmplotyeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
