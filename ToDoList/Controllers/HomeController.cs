using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Render the landing page
        public IActionResult Index()
        {
            return View("LandingPage");
        }

       
        // Render the privacy policy page
        public IActionResult Privacy()
        {
            return View();
        }

        // Handle application errors
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            _logger.LogError("Error occurred with RequestId: {RequestId}", errorModel.RequestId);
            return View(errorModel);
        }
    }
}
