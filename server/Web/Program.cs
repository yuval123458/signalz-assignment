using Application.Users;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// DI registration
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IImageStorage>(new DiskImageStorage(
    Path.Combine(builder.Environment.ContentRootPath, "uploads")));
builder.Services.AddScoped<UserService>();

var app = builder.Build();

app.MapControllers();

// static frontend
var clientPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "..", "client"));
var clientFiles = new PhysicalFileProvider(clientPath);
app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = clientFiles });
app.UseStaticFiles(new StaticFileOptions { FileProvider = clientFiles });

// serve saved images at /uploads
var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.Run();
