using Microsoft.AspNetCore.Mvc;

namespace ToDoList.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
