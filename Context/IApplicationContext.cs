using Microsoft.EntityFrameworkCore;

namespace CQRS.WebApi;

public interface IApplicationContext
{
    DbSet<Product> Products { get; set; }
    Task<int> SaveChanges();
}
