using Microsoft.EntityFrameworkCore;
using AssetService.Models;
namespace AssetService.Data;

public class AssetDbContext : DbContext
{
    public AssetDbContext(DbContextOptions<AssetDbContext>options): base(options){}
    public DbSet<Assets> Assets{get; set;}
}
