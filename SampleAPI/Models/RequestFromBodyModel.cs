using System.ComponentModel.DataAnnotations;

namespace SampleAPI.Models
{
  public class RequestFromBodyModel
  {
    [Required] public int Id { get; set; }
    [Required] public required string StaticData { get; set; }
  }
}