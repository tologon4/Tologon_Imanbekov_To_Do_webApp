using lesson55.Models;
using lesson55.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lesson55.Models;
namespace lesson55.Controllers;

public class MyTaskController : Controller
{
    public MyTaskContext _context;

    public MyTaskController(MyTaskContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index(string? priority, string? status, string? titleSearch, DateTime? dateFrom, DateTime? dateTo, string wordFilter,TaskSortState sortState = TaskSortState.NameAsc, int page = 1)
    {
        IQueryable<MyTask> filteredTasks = _context.Tasks;
        ViewBag.Priorities = new List<string>() {"Высокий", "Средний", "Низкий" };
        ViewBag.Statuses = new List<string>() { "Новая", "Открыта", "Закрыта" };
        
        /*DateTime? utcDateFrom = dateFrom.HasValue ? dateFrom.Value.ToUniversalTime() : null;
        DateTime? utcDateTo = dateTo.HasValue ? dateTo.Value.ToUniversalTime() : null;*/
        if (!string.IsNullOrEmpty(priority))
            filteredTasks = filteredTasks.Where(t => t.Priority == priority);
        if (!string.IsNullOrEmpty(status))
            filteredTasks = filteredTasks.Where(t => t.Status == status);
        if (!string.IsNullOrEmpty(titleSearch))
            filteredTasks = filteredTasks.Where(t => t.Name.Equals(titleSearch));
        if (!string.IsNullOrEmpty(wordFilter))
            filteredTasks = filteredTasks.Where(t => t.Description.Contains(wordFilter.ToLower()));
        if (dateFrom.HasValue)
        {
            dateFrom = dateFrom.Value.ToUniversalTime();
            filteredTasks = filteredTasks.Where(t => t.DateOfCreation >= dateFrom);
        }
        if (dateTo.HasValue)
        {
            dateTo = dateTo.Value.ToUniversalTime();
            filteredTasks = filteredTasks.Where(t => t.DateOfCreation <= dateTo);
        }

        var tasks = filteredTasks.ToList();
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
            default:
                tasks = tasks.OrderBy(t => t.Name).ToList();
                break;
        }
        int pageSize = 5;
        int count = tasks.Count();
        var items = tasks.Skip((page - 1) * pageSize).Take(pageSize);
        PageViewModel pvm = new PageViewModel(count, page, pageSize);
        TaskIndexViewModel tivm = new TaskIndexViewModel()
        {
            PageViewModel = pvm,
            Tasks = items
        };
        return View(tivm);
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
                task.DateOfOpening = DateTime.UtcNow;
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
                task.DateOfClosing = DateTime.UtcNow;
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
            {
                if (task.Status.Equals("Закрыта") || task.Status.Equals("Навая"))
                    return View(task);
                else
                    return RedirectToAction("Index");
            }
                
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