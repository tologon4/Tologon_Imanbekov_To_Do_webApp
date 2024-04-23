using Microsoft.AspNetCore.Mvc;

namespace lesson55.Controllers;

public class MyTaskController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}