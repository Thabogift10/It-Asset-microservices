using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AssetService.Data;
using AssetService.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AssetDbContext>(Options => Options.UseSqlite("Data Source = asset.db"));
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
    var db = scope.ServiceProvider.GetRequiredService<AssetDbContext>();
    db.Database.EnsureCreated();
}
//
app.MapAssetEndpoint();
app.Run();