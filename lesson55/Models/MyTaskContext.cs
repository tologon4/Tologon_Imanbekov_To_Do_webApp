using Microsoft.EntityFrameworkCore;

namespace lesson55.Models;

public class MyTaskContext : DbContext
{
    public DbSet<MyTask> Tasks { get; set; }
    public MyTaskContext(DbContextOptions<MyTaskContext> options) : base(options){}
}