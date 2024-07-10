using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly string _workerName;
    private static readonly Random Random = new Random();
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _workerName = Environment.MachineName;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var context = new ItemsContext())
            {
                // Retrieve items assigned to this worker
                var items = await context.Items
                    .Where(item => item.CurrentWorker == _workerName)
                    .ToListAsync(stoppingToken);
                foreach (var item in items)
                {
                    item.Value += 1;
                    context.Update(item);
                }
                await context.SaveChangesAsync(stoppingToken);
                _logger.LogInformation($"Worker {_workerName} processing items: {string.Join(", ", items.Select(i => i.ID))}");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
public class ItemsContext : DbContext
{
    public DbSet<Item> Items { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=tlspc-035;Database=Worker;Integrated Security=True;");
    }
}
public class Item
{
    public Guid ID { get; set; }
    public int Value { get; set; }
    public string CurrentWorker { get; set; }
}