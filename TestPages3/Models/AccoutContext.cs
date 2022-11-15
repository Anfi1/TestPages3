using Microsoft.EntityFrameworkCore;

namespace TestPages3.Models
{
    public class AccoutContext : DbContext
    {
        //
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public AccoutContext(DbContextOptions<AccoutContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";
 
            string adminEmail = "admin@mail.ru";
            string adminPassword = "123456";
 
            // добавляем роли
            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role userRole = new Role { Id = 2, Name = userRoleName };
            Account adminUser = new Account { Id = 1, Email = adminEmail, Password = adminPassword, RoleId = adminRole.Id };
 
            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<Account>().HasData( new Account[] { adminUser });
            base.OnModelCreating(modelBuilder);
        }
        
    }
}