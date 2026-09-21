using AssignmentService.Data;
using AssignmentService.Endpoints;
using AssignmentService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AssignmentDbContext>(Options => Options.UseSqlite("Data Source = assignment.db"));
builder.Services.AddCors(Options =>
{
    Options.AddPolicy("AllowAll", Policy =>
    {
        Policy.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");
//
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AssignmentDbContext>();
    db.Database.EnsureCreated();
}
//
app.MapAssignmentEndpoints();
app.Run();