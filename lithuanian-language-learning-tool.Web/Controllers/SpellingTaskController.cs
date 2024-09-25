using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    public class SpellingTaskController : Controller
    {
        // GET: /SpellingTask/
        public IActionResult SpellingTask()
        {
            return View();
        }

        // POST: /SubmitSpellingTask/
        [HttpPost]
        public IActionResult SubmitSpellingTask()
        {
            // Process the form data here
            return RedirectToAction("SpellingTask");
        }
    }
}
