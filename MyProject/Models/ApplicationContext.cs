using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace MyProject.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public override DbSet<User> Users { get; set; } = null!;
        public  DbSet<Post> Posts { set; get; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            Database.EnsureCreated(); 
        }
    }
}
