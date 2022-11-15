using Microsoft.EntityFrameworkCore;

namespace TestPages3.Models
{
    public class AccoutContext : DbContext
    {
        public AccoutContext(DbContextOptions<AccoutContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        
    }
}