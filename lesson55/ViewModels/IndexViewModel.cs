using lesson55.Models;

namespace lesson55.ViewModels;

public class IndexViewModel
{
    public IEnumerable<MyTask> MyTasks { get; set; }

    public PageViewModel PageViewModel { get; set; }
}