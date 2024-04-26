namespace lesson55.Models;

public class TaskIndexViewModel
{
    public IEnumerable<MyTask> Tasks { get; set; }
    public PageViewModel PageViewModel { get; set; }
}