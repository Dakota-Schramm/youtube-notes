using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using youtube_notes.Data;
var builder = WebApplication.CreateBuilder(args);

// TODO: Add environment-specific configuration for the database connection string
// https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-model?view=aspnetcore-10.0&tabs=visual-studio-code#use-sqlite-for-development-sql-server-for-production
// Development environment configuration
builder.Services.AddDbContext<youtube_notesContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("youtube_notesContext") ?? throw new InvalidOperationException("Connection string 'youtube_notesContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
