using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestProject.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RestProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RestProjectContext") ??
    throw new InvalidOperationException("Connection string 'RestProjectContext' not found.")));

// Add services to the container.

builder.Services.AddControllers()
/*.AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)*/ ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<RestProjectContext>();
    context.Database.EnsureCreated();
    context.Database.ExecuteSqlRaw("INSERT INTO author (author_name, author_id) VALUES\r\n('A. Bartlett Giamatti', 1),\r\n('A. Elizabeth Delany', 2),\r\n('A. Merritt', 3),\r\n('A. Roger Merrill', 4),\r\n('A. Walton Litz', 5),\r\n('A.B. Yehoshua', 6)");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
