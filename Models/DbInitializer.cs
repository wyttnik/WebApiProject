using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace RestProject.Models
{
    public class DbInitializer
    {
        public static void Initialize(DbContext DbContext, IDataProtectionProvider dataProtectionProvider)
        {
            if (DbContext.GetType() == typeof(RestProjectContext))
            {
                var context = (RestProjectContext)DbContext;

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
            else if (DbContext.GetType() == typeof(AuthContext))
            {
                var context = (AuthContext)DbContext;
                if (!context.Users.Any())
                {
                    var dataProtector = dataProtectionProvider.CreateProtector("UsersControllerPurpose");
                    var users = new User[]
                    {
                        new User{ User_id = 1, Login = dataProtector.Protect("zeroOzer")
                        , Password = dataProtector.Protect("123321"), Role = "admin"},
                        new User{ User_id = 2, Login = dataProtector.Protect("testUser"), 
                            Password = dataProtector.Protect("123"), Role = "user"},
                    };
                    context.Users.AddRange(users);
                    context.SaveChanges();

                }
            }
        }
    }
}
