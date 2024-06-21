using Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Api.Data{
    public class ApplicationDbContext: IdentityDbContext<AppUser>{
        public ApplicationDbContext(DbContextOptions options) : base(options) { 

            
        }

        protected override void OnModelCreating(ModelBuilder builder){
            base.OnModelCreating(builder);

            List<IdentityRole> roles = new List<IdentityRole>{
                new IdentityRole{Name="Admin",NormalizedName="ADMIN"},
                new IdentityRole{Name="User",NormalizedName="USER"},
            };

             builder.Entity<IdentityRole>().HasData(roles);

        }

        public DbSet<AppUser> AppUser {get; set;}  = null!;  
        public DbSet<Workouts> Workouts {get; set;}  = null!;  
    }
}