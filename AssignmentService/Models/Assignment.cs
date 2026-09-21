using System.ComponentModel.DataAnnotations;
namespace AssignmentService.Models;

public class Assignments
{
    public int Id{get; set;}
    public int AssetId{get; set;}
    public int UserId{get; set;}
    public DateTime AssignedDate {get; set;} = DateTime.UtcNow;
    public DateTime? ReturnDate{get; set;}
    public string Status{get;set;} = "Active";

}