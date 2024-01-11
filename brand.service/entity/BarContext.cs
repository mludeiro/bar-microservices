using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BrandService.Entity
{

    public class BarContext : DbContext
    {
        public BarContext(DbContextOptions<BarContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>().ToTable("Brand");
            modelBuilder.Entity<Beer>().ToTable("Beer");
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Beer> Beers { get; set;}
        public DbSet<Brand> Brands { get; set;}

    }
}