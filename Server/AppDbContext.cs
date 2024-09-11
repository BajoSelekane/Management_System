using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace Server
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<GenerateDepartment> GenerateDepartments { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Branch> Branchs { get; set; }
        public DbSet<SystemRoles> SystemRoles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<RefreshTokenInfo> RefreshTokenInfos { get; set; }
        
    }
}

