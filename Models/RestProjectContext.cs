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

        public DbSet<Book> Books { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<BookLanguage> BookLanguages { get; set; }

        public DbSet<AuthorBook> AuthorBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().ToTable("Book");
            modelBuilder.Entity<Book>().HasKey(e => e.Book_id);
            modelBuilder.Entity<Book>()
                .HasMany(e => e.Authors)
                .WithMany(e => e.Books)
                .UsingEntity<AuthorBook>();

            modelBuilder.Entity<Author>().ToTable("Author").HasKey(e=>e.Author_id);

            modelBuilder.Entity<Publisher>().ToTable("Publisher");
            modelBuilder.Entity<Publisher>().HasKey(e => e.Publisher_id);
            modelBuilder.Entity<Publisher>()
                .HasMany(e=>e.Books)
                .WithOne(e=>e.Publisher)
                .HasForeignKey(e=>e.Publisher_id)
                .IsRequired();

            modelBuilder.Entity<BookLanguage>().ToTable("Book_language");
            modelBuilder.Entity<BookLanguage>().HasKey(e => e.Language_id);
            modelBuilder.Entity<BookLanguage>()
                .HasMany(e => e.Books)
                .WithOne(e => e.BookLanguage)
                .HasForeignKey(e => e.Language_id)
                .IsRequired();

            modelBuilder.Entity<AuthorBook>().ToTable("Book_author")
                .HasKey(e => new { e.Book_id, e.Author_id });
        }
    }
}
