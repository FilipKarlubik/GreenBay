using Microsoft.AspNetCore.Mvc;

namespace GreenBay.Controllers
{
    public class HomeController : Controller
    {
        [Route("/hello")]
        public IActionResult Hello()
        {
            return View();
        }
    }
}
