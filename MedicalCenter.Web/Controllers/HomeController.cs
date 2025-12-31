using System.Diagnostics;
using MedicalCenter.Web.Attributes;
using MedicalCenter.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalCenter.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly bool isAdministrator;
        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            isAdministrator = httpContextAccessor.HttpContext.Session.GetString("UserRole") == "Administrator";
        }

        [AuthenticateAuthorize]
        public IActionResult Index()
        {
            if (isAdministrator)
            {
                return RedirectToAction("Index", "MedicalExaminations");
            }

            return RedirectToAction("MyExaminations", "MedicalExaminations");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
