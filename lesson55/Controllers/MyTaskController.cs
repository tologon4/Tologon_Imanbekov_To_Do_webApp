using lesson55.Models;
using lesson55.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lesson55.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace lesson55.Controllers;

public class MyTaskController : Controller
{
    public MyTaskContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public MyTaskController(MyTaskContext context, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    public async Task<IActionResult> Index(string? priority, string? status, string? titleSearch, DateTime? dateFrom, DateTime? dateTo, string? wordFilter,TaskSortState sortState = TaskSortState.NameAsc, int page = 1)
    {
        IQueryable<MyTask> filteredTasks = _context.Tasks;
        ViewBag.Priorities = new List<string>() {"Высокий", "Средний", "Низкий" };
        ViewBag.Statuses = new List<string>() { "Новая", "Открыта", "Закрыта" };
        if (!string.IsNullOrEmpty(ViewBag.Priority))
            priority = ViewBag.Priority;
        if (!string.IsNullOrEmpty(ViewBag.Status))
            status = ViewBag.Status;
        if (!string.IsNullOrEmpty(ViewBag.TitleSearch))
            titleSearch = ViewBag.TitleSearch;
        if (!string.IsNullOrEmpty(ViewBag.WordFilter))
            wordFilter = ViewBag.WordFilter;
        if (!string.IsNullOrEmpty(ViewBag.DateTo))
            dateTo = ViewBag.DateTo;
        if (!string.IsNullOrEmpty(ViewBag.DateFrom))
            dateFrom = ViewBag.DateFrom;
        List<MyTask> tasks = await FilterSort(filteredTasks,  priority,  status,  titleSearch,  dateFrom,  dateTo,  wordFilter,  sortState);
        int pageSize = 1;
        int count = tasks.Count();
        var items = tasks.Skip((page - 1) * pageSize).Take(pageSize);
        PageViewModel pvm = new PageViewModel(count, page, pageSize);
        TaskIndexViewModel tivm = new TaskIndexViewModel()
        {
            PageViewModel = pvm,
            Tasks = items
        };
        ViewBag.CurrentSort = sortState;
        ViewBag.CurrentFilter = tasks;
        ViewBag.Priority = priority;
        ViewBag.Status = status;
        ViewBag.TitleSearch = titleSearch;
        ViewBag.WordFilter = wordFilter;
        if (dateTo.HasValue)
            ViewBag.DateTo = dateTo.Value.ToUniversalTime();
        if (dateFrom.HasValue)
            ViewBag.DateFrom = dateFrom.Value.ToUniversalTime();
        return View(tivm);
    }

    public async Task<List<MyTask>> FilterSort(IQueryable<MyTask> filteredTasks, string? priority, string? status, string? titleSearch, DateTime? dateFrom, DateTime? dateTo, string? wordFilter, TaskSortState sortState)
    {
        
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
        return tasks;
    }

    [Authorize]
    public async Task<IActionResult> Create()
    {
        ViewBag.Priorities = new List<string>() {"Высокий", "Средний", "Низкий" };
        string userId = _userManager.GetUserId(User);
        User user = await _userManager.FindByIdAsync(userId);
        ViewBag.User = user;
        return View();
    }

    [HttpPost]
    public IActionResult Create(MyTask task)
    {
        ViewBag.Priorities = new List<string>(){"Высокий", "Средний", "Низкий" };
        int? userId = int.Parse(_userManager.GetUserId(User));
        User user = _context.Users.FirstOrDefault(u => u.Id == userId);
        ViewBag.User = user;
        if (_context.Tasks.Any(t => t.Name == task.Name) && _context.Tasks.Any(t => t.ExecutorId == userId))
        {
            ModelState.AddModelError("", "Задача с таким название уже существует!");
            return View(task);
        }
        if (ModelState.IsValid)
        {
            task.DateOfCreation = DateTime.Now.ToUniversalTime();
            _context.Tasks.Add(task);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(task);
    }

    [Authorize]
    public IActionResult MyTask(int id)
    {
        var tasks = _context.Tasks.Include(u => u.Creator).Include(u => u.Executor).ToList();
        var task = tasks.FirstOrDefault(t => t.Id == id);
        int? userId = Convert.ToInt32(_userManager.GetUserId(User));
        if (userId != null)
        {
            ViewBag.UserId = userId;
        }
        return View(task);
    }
    
    [Authorize]
    public IActionResult Open(int? id)
    {
        if (id != null)
        {
            MyTask task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            int userId = Convert.ToInt32(_userManager.GetUserId(User));
            if (task.Status == "Новая" && task.ExecutorId != null && (task.ExecutorId == userId || User.IsInRole("admin")))
            {
                task.DateOfOpening = DateTime.UtcNow;
                task.Status = "Открыта";
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        return NotFound();
    }
    [Authorize]
    public IActionResult Close(int? id)
    {
        if (id != null)
        {
            MyTask task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            int userId = Convert.ToInt32(_userManager.GetUserId(User));
            if (task.Status=="Открыта" && task.ExecutorId != null && (task.ExecutorId == userId || User.IsInRole("admin")))
            {
                task.DateOfClosing = DateTime.UtcNow;
                task.Status = "Закрыта";
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        return NotFound();
    }
    [HttpGet]
    [ActionName("Delete")]
    public async Task<IActionResult> ConfirmDelete(int? id)
    {
        if (id != null)
        {
            MyTask task = await _context.Tasks.FirstOrDefaultAsync(p => p.Id == id);
            string userId = _userManager.GetUserId(User);
            User user = await _userManager.FindByIdAsync(userId);
            if (task != null && (task.CreatorId == user.Id || User.IsInRole("admin")))
            {
                if (!task.Status.Equals("Открыта"))
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
    [Authorize]
    public IActionResult TakeTask(int id)
    {
        int userId = int.Parse(_userManager.GetUserId(User));
        if (id != null)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (task.ExecutorId == null)
            {
                task.ExecutorId = userId;
                _context.Update(task);
                _context.SaveChanges();
                return RedirectToAction("MyTask", task);
            }
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