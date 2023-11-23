using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TCCProjeto.Models;

namespace TCCProjeto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Equipe()
        {
            return View();
        }

        // CONVENCIONAIS

        public IActionResult Papel()
        {
            return View();
        }

        public IActionResult Vidro()
        {
            return View();
        }
        public IActionResult Plastico()
        {
            return View();
        }
        public IActionResult Metal()
        {
            return View();
        }


        // ESPECÍFICOS
        public IActionResult Pilha()
        {
            return View();
        }

        public IActionResult Organico()
        {
            return View();
        }

        public IActionResult Madeira()
        {
            return View();
        }
        public IActionResult Hospitalar()
        {
            return View();
        }

        public IActionResult Radioativo()
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