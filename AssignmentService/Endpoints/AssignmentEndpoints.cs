using System.ComponentModel.DataAnnotations;
using AssignmentService.Data;
using AssignmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace AssignmentService.Endpoints;

public static class AssignmentEndpoints
{
    public static void MapAssignmentEndpoints(this WebApplication app)
    {
        app.MapPost("/assignments", async(Assignments newAssignment, AssignmentDbContext db) =>
        {
            var validationAssignment = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(newAssignment, new ValidationContext(newAssignment),validationAssignment,true);

            if(!isValid)
            return Results.BadRequest(validationAssignment.Select(v => v.ErrorMessage));

            db.Assignments.Add(newAssignment);
            await db.SaveChangesAsync();
            return Results.Created($"/assignments/{newAssignment.Id}", newAssignment);
        });

        app.MapGet("/assignments", async(AssignmentDbContext db) =>
        {
            return Results.Ok(await db.Assignments.ToListAsync());
        });

        app.MapGet("/assignments/{id}", async(int id, AssignmentDbContext db) =>
        {
            var assignment = await db.Assignments.FirstOrDefaultAsync(a => a.Id == id);
            if(assignment is null)
            return Results.NotFound("Assignment not found");

            return Results.Ok(assignment);
        });

        app.MapPut("/assignments/{id}/return", async(int id,Assignments updatedAssignment, AssignmentDbContext db) =>
        {
            var newUpdatedAssignment = await db.Assignments.FirstOrDefaultAsync(n => n.Id == id);
            if(newUpdatedAssignment is null)
            return Results.NotFound("Assignment not found");

            newUpdatedAssignment.ReturnDate = DateTime.UtcNow;
            newUpdatedAssignment.Status = "Returned";

            await db.SaveChangesAsync();
            return Results.Ok(newUpdatedAssignment);
        });

        app.MapDelete("/assignments/{id}", async(int id, AssignmentDbContext db) =>
        {
            var assignment = await db.Assignments.FirstOrDefaultAsync(a => a.Id == id);
            if(assignment is null)
            return Results.NotFound("Assignment not found");

            db.Assignments.Remove(assignment);
            await db.SaveChangesAsync();
            return Results.Ok($"Assigment {id} removed");
        });
    }
}

