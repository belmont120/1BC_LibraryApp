using LibraryApp.Server.Application.Interfaces;
using LibraryApp.Server.Application.Interfaces.Repositories;
using LibraryApp.Server.Application.Options;
using LibraryApp.Server.Application.Services;
using LibraryApp.Server.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ConnectionStringsOptions>(
    builder.Configuration.GetSection("ConnectionStrings")
);
builder.Services.AddSingleton<IBookRepository, BookRepository>();
builder.Services.AddSingleton<IBookEventRepository, BookEventRepository>();
builder.Services.AddSingleton<IBookManagementService, BookManagementService>();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
