using Microsoft.EntityFrameworkCore;
using TestTask.Domain;

namespace TestTask.Application.Interfaces
{
    public interface ITestDbContext
    {
        DbSet<Product> Products { get; set; }
        DbSet<ProductVersion> ProductVersions { get; set; }
        DbSet<EventLog> EventLogs { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
