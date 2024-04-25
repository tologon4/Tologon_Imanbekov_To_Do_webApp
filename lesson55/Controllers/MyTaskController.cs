using lesson55.Models;
using lesson55.Services;
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
    public IActionResult Index(TaskSortState sortState = TaskSortState.NameAsc)
    {
        var tasks = _context.Tasks.ToList();
        ViewBag.NameSort = sortState == TaskSortState.NameAsc ? TaskSortState.NameDesc : TaskSortState.NameAsc;
        ViewBag.PrioritySort = sortState == TaskSortState.PriorityAsc ? TaskSortState.PriorityDesc : TaskSortState.PriorityAsc;
        ViewBag.StatusSort = sortState == TaskSortState.StatusAsc ? TaskSortState.StatusDesc : TaskSortState.StatusAsc;
        ViewBag.DateOfCreationSort = sortState == TaskSortState.DateOfCreationAsc ? TaskSortState.DateOfCreationDesc : TaskSortState.DateOfCreationAsc;
        switch (sortState)
        {
            case TaskSortState.NameAsc:
                tasks = tasks.OrderBy(t => t.Name).ToList();
                break;
            case TaskSortState.NameDesc:
                tasks = tasks.OrderByDescending(t => t.Name).ToList();
                break;
            case TaskSortState.PriorityAsc:
                tasks = tasks.OrderBy(t => SortFields(t.Priority)).ToList();
                break;
            case TaskSortState.PriorityDesc:
                tasks = tasks.OrderByDescending(t => SortFields(t.Priority)).ToList();
                break;
            case TaskSortState.StatusAsc:
                tasks = tasks.OrderBy(t => SortFields(t.Status)).ToList();
                break;
            case TaskSortState.StatusDesc:
                tasks = tasks.OrderByDescending(t => SortFields(t.Status)).ToList();
                break;
            case TaskSortState.DateOfCreationAsc:
                tasks = tasks.OrderBy(t => t.DateOfCreation).ToList();
                break;
            case TaskSortState.DateOfCreationDesc:
                tasks = tasks.OrderByDescending(t => t.DateOfCreation).ToList();
                break;
        }
        return View(tasks);
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
            task.DateOfCreation = DateTime.Now.ToUniversalTime();
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

    private int SortFields(string field)
    {
        switch (field.ToLower())
        {
            case "высокий":
                return 3;
            case "средний":
                return 2;
            case "низкий":
                return 1;
            case "новая":
                return 3;
            case "открыта":
                return 2;
            case "закрыта":
                return 1;
            default:
                return 0;
        }
    }
}