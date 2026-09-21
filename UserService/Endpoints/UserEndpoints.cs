using System.ComponentModel.DataAnnotations;
using UserService.Data;
using UserService.Models;
using Microsoft.EntityFrameworkCore;
namespace UserService.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/users", async(Users newUser, UserDbContext db) =>
        {
            var validationUsers = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(newUser, new ValidationContext(newUser),validationUsers,true);

            if(!isValid)
            return Results.BadRequest(validationUsers.Select(u => u.ErrorMessage));

            db.Users.Add(newUser);
            await db.SaveChangesAsync();
            return Results.Created($"/users/{newUser.Id}", newUser);
        });

        app.MapGet("/users", async(UserDbContext db) =>
        {
            return Results.Ok(await db.Users.ToListAsync());
        });

        app.MapGet("/users/{id}", async(int id, UserDbContext db) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(user is null)
            return Results.NotFound("User not found");
            return Results.Ok(user);
        });

        app.MapPut("/users/{id}", async(int id, Users updateUser, UserDbContext db) =>
        {
            var newUser = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(newUser is null)
            return Results.NotFound("User not found");

            newUser.UserName = updateUser.UserName;
            newUser.Email = updateUser.Email;
            newUser.Department = updateUser.Department;
            newUser.Role = updateUser.Role;

            await db.SaveChangesAsync();
            return Results.Ok(newUser);
        });

        app.MapDelete("/users/{id}", async(int id, UserDbContext db) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(user is null)
            return Results.NotFound("User not found");

            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return Results.Ok($"User {id} reemoved");
        });
    }
}

