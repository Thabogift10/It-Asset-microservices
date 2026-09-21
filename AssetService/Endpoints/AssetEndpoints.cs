using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AssetService.Data;
using AssetService.Models;
using System.Runtime.Serialization;

namespace AssetService.Endpoints;

public static class AssetEndpoints
{
    public static void MapAssetEndpoint(this WebApplication app)
    {
        app.MapPost("/assets", async(Assets newAssets, AssetDbContext db) =>
        {
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(newAssets, new ValidationContext(newAssets),validationResults,true);

            if(!isValid)
            return Results.BadRequest(validationResults.Select(v => v.ErrorMessage));

            db.Assets.Add(newAssets);
            await db.SaveChangesAsync();
            return Results.Created($"/assets/{newAssets.Id}",newAssets);
        });

        app.MapGet("/assets/{id}", async(int id, AssetDbContext db) =>
        {
            var asset = await db.Assets.FirstOrDefaultAsync(a => a.Id == id);
            if(asset is null)
            return Results.NotFound("Asset not found");

            return Results.Ok(asset);
        });

        app.MapGet("/assets", async(string? search, AssetDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return Results.Ok(await db.Assets.ToListAsync());
            }
            var filteredAssets = await db.Assets.Where(a => a.AssetName.Contains(search)).ToListAsync();
            return Results.Ok(filteredAssets);
        });

        app.MapPut("/assets/{id}", async(int id, Assets updatedAsset, AssetDbContext db) =>
        {
            var newAsset = await db.Assets.FirstOrDefaultAsync(a => a.Id == id);
            if(newAsset is null)
            return Results.NotFound("Asset not found");

            newAsset.Status = updatedAsset.Status;
            await db.SaveChangesAsync();
            return Results.Ok(newAsset);
        });

        app.MapDelete("/assets/{id}", async(int id, AssetDbContext db) =>
        {
            var asset = await db.Assets.FirstOrDefaultAsync(a => a.Id == id);
            if(asset is null)
            return Results.NotFound("Asset not found");

            db.Assets.Remove(asset);
            await db.SaveChangesAsync();
            return Results.Ok($"Asset {id} removed");
        });
    }
}