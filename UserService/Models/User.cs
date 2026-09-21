using System.ComponentModel.DataAnnotations;
namespace UserService.Models;
public class Users
{
    public int Id{get; set;}
    //validation
    [Required(ErrorMessage = "User is required")]
    [MinLength(5)]
    public string UserName{get; set;} = "";
    [Required]
    [EmailAddress]
    public string Email{get; set;} = "";
    [Required]
    public string Department{get; set;} = "";
    [Required]
    public string Role{get; set;} = "";
}

