using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Taskify.Data;
using Taskify.Models;
using static System.Net.Mime.MediaTypeNames;

//Program.cs este fisierul care se executa prima data in cadrul aplicatiei

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() 
    .AddEntityFrameworkStores<ApplicationDbContext>();
/*rolul se adauga inaintea bd*/


builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    ///CreateScope ofera acces la instanta curenta la aplicatiei
    ///var scope are un Service Provider - fol pentru a injecta dependente in aplciatie
    ///dependente = DB, Cookie, Sesiune, Autentificare, etc.
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);

}




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//speram sa mearga sa facem erori
app.Use(async (context, next) =>
{

    await next();

    if(context.Response.StatusCode == 404)
    {

        context.Request.Path= "/Home/Error/404";
        await next();
    }
}
);



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Projects}/{action=UserDelete}/{id}/{rmvuser}/{rmvproject}");
app.MapRazorPages();



app.Run();
