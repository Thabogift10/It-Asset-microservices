using AssignmentService.Models;
using Microsoft.EntityFrameworkCore;
namespace AssignmentService.Data;

public class AssignmentDbContext : DbContext
{
    public AssignmentDbContext(DbContextOptions<AssignmentDbContext> options) : base(options){}
    public DbSet<Assignments> Assignments{get;set;}
}

