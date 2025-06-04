using Microsoft.AspNetCore.Mvc;

namespace PhoneBook.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
