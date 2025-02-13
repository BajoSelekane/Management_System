using BaseLibrary.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Server
{
    //public class AppDbContext : IdentityDbContext<ApplicationUser>, IDisposable
    //{
    //    public AppDbContext()
    //        : base("MvcArchBSEFDB", throwIfV1Schema: false)
    //    {
    //    }

    //    public static AppDbContext Create()
    //    {
    //        return new AppDbContext();
    //    }

    //    protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
    //    {
    //        base.OnModelCreating(modelBuilder);

    //    }

    //    public DbSet<IdentityUser> AppUsers { get; set; } // Creates Two Tables AspNetUsers & IdentityUser


    //}

    //public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    public class AppDbContext : IdentityDbContext<IdentityUser>, IDisposable
    {
        public AppDbContext()
        {
            
        }

       
        public DbSet<Employee> Employees { get; set; }
        //public DbSet<ApplicationUsers> ApplicationUsers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<GenerateDepartment> GenerateDepartments { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<SystemRoles> SystemRoles { get; set; }
        //public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<RefreshTokenInfo> RefreshTokenInfos { get; set; }

    }
}

