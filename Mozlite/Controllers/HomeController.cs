using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Data;
using Mozlite.Extensions.Tasks;
using Mozlite.Models;

namespace Mozlite.Controllers
{
    public class HomeController : Mozlite.Mvc.ControllerBase
    {
        private readonly IRepository<TaskDescriptor> _repository;
        public HomeController(IRepository<TaskDescriptor> repository)
        {
            _repository = repository;
        }


        public IActionResult Index()
        {
            var tasks = _repository.Fetch();
            return View(tasks);
        }

        [HttpPost]
        public IActionResult ModalTest(IFormCollection form)
        {
            return Error("你已经成功提交了信息：" + form["role"]);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ModalTest()
        {
            return View();
        }
    }
}
