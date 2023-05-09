using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestProject.Models;

namespace RestProject.Models
{
    public class RestProjectContext : DbContext
    {
        public RestProjectContext (DbContextOptions<RestProjectContext> options)
            : base(options)
        {
        }

        public DbSet<RestProject.Models.Book> Books { get; set; }

        public DbSet<RestProject.Models.Author> Authors { get; set; }

        public DbSet<RestProject.Models.Publisher> Publishers { get; set; }

        public DbSet<RestProject.Models.AuthorBook> AuthorBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().ToTable("Book")
                .HasMany(e => e.Authors)
                .WithMany(e => e.Books)
                .UsingEntity<AuthorBook>();

            modelBuilder.Entity<Author>().ToTable("Author");

            modelBuilder.Entity<Publisher>().ToTable("Publisher")
                .HasMany(e=>e.Books)
                .WithOne(e=>e.Publisher)
                .HasForeignKey(e=>e.PublisherId)
                .IsRequired();
        }
    }
}
