using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    public class PunctuationTaskController : Controller
    {
        // GET: /PunctuationTask/
        public IActionResult PunctuationTask()
        {
            return View();
        }

        // POST: /SubmitPunctuationTask/
        [HttpPost]
        public IActionResult SubmitPunctuationTask()
        {
            
            return RedirectToAction("PunctuationTask");
        }
    }
}
