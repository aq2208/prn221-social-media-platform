using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PRN221_SocialMedia.Hub;
using PRN221_SocialMedia.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSignalR();

builder.Services.AddDbContext<PRN221_SocialMediaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DemoConnectStr")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();
app.UseAuthentication();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    app.MapHub<MessageHub>("/messageHub");
});

app.MapRazorPages();

app.Run();
