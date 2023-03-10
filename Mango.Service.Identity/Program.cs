using Mango.Service.Identity.DbContexts;
using Mango.Service.Identity.Helpers;
using Mango.Service.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options=>
    options.UseSqlServer(config.GetConnectionString("Mac")));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

var identityBuilder = builder.Services.AddIdentityServer(option =>{
    option.Events.RaiseErrorEvents = true;
    option.Events.RaiseFailureEvents = true;
    option.Events.RaiseInformationEvents = true;
    option.Events.RaiseSuccessEvents = true;
    option.EmitStaticAudienceClaim = true;
}).AddInMemoryIdentityResources(SD.IdentityResources)
.AddInMemoryApiScopes(SD.ApiScopes)
.AddInMemoryClients(SD.Clients)
.AddAspNetIdentity<AppUser>();

identityBuilder.AddDeveloperSigningCredential();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

