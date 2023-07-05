using Microsoft.AspNetCore.Mvc;

namespace Tweetero.API.Controllers
{
    public class TweetsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
