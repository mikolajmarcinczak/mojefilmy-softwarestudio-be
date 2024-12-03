using System.ComponentModel.DataAnnotations;

namespace mojefilmy_softwarestudio_be.Models
{
  public class Movie
  {
    [Required]
    public Int32 Id { get; set; }
    [Required]
    public Int32? ExternalId { get; set; }
    [Required]
    [MaxLength(255)]
    public string? Title { get; set; }
    [Required]
    [MaxLength(255)]
    public string? Director { get; set; }
    public float Rate { get; set; }
    public Int32 Year { get; set; }
  }

  public class ExternalMovie
  {
    [Required]
    public Int32 Id { get; set; }
    [Required]
    [MaxLength(255)]
    public string? Title { get; set; }
    [Required]
    [MaxLength(255)]
    public string? Director { get; set; }
    public float Rate { get; set; }
    public Int32 Year { get; set; }
  }
}
