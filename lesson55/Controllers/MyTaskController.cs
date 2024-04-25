using lesson55.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        return View(_context.Tasks.FirstOrDefault(t => t.Id == id));
    }

    public IActionResult Open(int? id)
    {
        if (id != null)
        {
            MyTask task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (task.Status == "Новая")
            {
                task.Status = "Открыта";
                _context.SaveChanges();
            }
        }
        return RedirectToAction("Index");
    }

    public IActionResult Close(int? id)
    {
        if (id != null)
        {
            MyTask task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (task.Status=="Открыта")
            {
                task.Status = "Закрыта";
                _context.SaveChanges();
            }
        }
        return RedirectToAction("Index");
    }
    [HttpGet]
    [ActionName("Delete")]
    public async Task<IActionResult> ConfirmDelete(int? id)
    {
        if (id != null)
        {
            MyTask task = await _context.Tasks.FirstOrDefaultAsync(p => p.Id == id);
            if (task != null)
                if (task.Status=="Закрыта"|| task.Status=="Новая")
                    return View(task);
        }
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id != null)
        {
            MyTask task = new MyTask() { Id = id.Value };
            _context.Entry(task).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return NotFound();
    }
}