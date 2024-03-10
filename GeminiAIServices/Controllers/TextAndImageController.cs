using Microsoft.AspNetCore.Mvc;

namespace GeminiAIServices.Controllers
{
    public class TextAndImageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
