using lesson55.Models;
using Microsoft.AspNetCore.Mvc;

namespace lesson55.Controllers;

public class MyTaskController : Controller
{
    public MyTaskContext _context;

    public MyTaskController(MyTaskContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View(_context.Tasks.ToList());
    }

    public IActionResult Create()
    {
        ViewBag.Priorities = new List<string>() {"Высокий", "Средний", "Низкий" };
        return View();
    }

    [HttpPost]
    public IActionResult Create(MyTask task)
    {
        ViewBag.Priorities = new List<string>(){"Высокий", "Средний", "Низкий" };
        if (ModelState.IsValid)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(task);
    }

    public IActionResult MyTask(int id)
    {
        if (id !=null)
            return View(_context.Tasks.FirstOrDefault(t => t.Id == id));
        return RedirectToAction("Index");
    }
}