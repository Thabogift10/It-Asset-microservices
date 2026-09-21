using System.ComponentModel.DataAnnotations;
namespace AssetService.Models;

    public class Assets
    {
      public int Id{get; set;}
      [Required(ErrorMessage = "Asset name is required")]
      [MinLength(2)]
      public string AssetName{get; set;} = string.Empty;
      public string SerialNumber{get; set;} = string.Empty;
      public string Status{get; set;} = "Available";
      public DateTime CreatedDate{get; set;} = DateTime.UtcNow;

    }
