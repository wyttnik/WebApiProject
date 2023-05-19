using Microsoft.EntityFrameworkCore;

namespace RestProject.Models
{
    public class DbInitializer
    {
        public static void Initialize(RestProjectContext context)
        {
            string script;
            if (!context.Authors.Any())
            {
                script = File.ReadAllText(@"sql_scripts\02_sqlserver_populate_author.sql");
                context.Database.ExecuteSqlRaw(script);
            };
            if (!context.Publishers.Any())
            {
                script = File.ReadAllText(@"sql_scripts\03_sqlserver_populate_publisher.sql");
                context.Database.ExecuteSqlRaw(script);
            };
            if (!context.BookLanguages.Any())
            {
                script = File.ReadAllText(@"sql_scripts\04_sqlserver_populate_language.sql");
                context.Database.ExecuteSqlRaw(script);
            };
            if (!context.Books.Any())
            {
                script = File.ReadAllText(@"sql_scripts\05_sqlserver_populate_book.sql");
                context.Database.ExecuteSqlRaw(script);
            };
            if (!context.AuthorBooks.Any())
            {
                script = File.ReadAllText(@"sql_scripts\06_sqlserver_populate_bookauthor.sql");
                context.Database.ExecuteSqlRaw(script);
            };
        }
    }
}
