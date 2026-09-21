using UserService.Endpoints;
using UserService.Data;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UserDbContext>(Options => Options.UseSqlite("Data Source=user.db"));
builder.Services.AddCors(Options =>
{
    Options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");
//
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>(); 
    db.Database.EnsureCreated();
}
//
app.MapUserEndpoints();
app.Run();