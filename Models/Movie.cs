using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace mojefilmy_softwarestudio_be.Models
{
  public class Movie
  {
    [Required]
    [JsonPropertyName("id")]
    public Int32 Id { get; set; }
    [JsonPropertyName("externalId")]
    public Int32? ExternalId { get; set; }
    [Required]
    [MaxLength(255)]
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;
    [Required]
    [MaxLength(255)]
    [JsonPropertyName("director")]
    public string Director { get; set; } = null!;
    [JsonPropertyName("rate")]
    public float Rate { get; set; }
    [JsonPropertyName("year")]
    public Int32 Year { get; set; }
  }

  public class ExternalMovie
  {
    [Required]
    [JsonPropertyName("id")]
    public Int32 Id { get; set; }
    [Required]
    [MaxLength(255)]
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [Required]
    [MaxLength(255)]
    [JsonPropertyName("director")]
    public string? Director { get; set; }
    [JsonPropertyName("rate")]
    public float Rate { get; set; }
    [JsonPropertyName("year")]
    public Int32 Year { get; set; }
  }

  public class CreateMovieDTO
  {
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = null!;
    [Required]
    [MaxLength(255)]
    public string Director { get; set; } = null!;
    public float Rate { get; set; }
    public Int32 Year { get; set; }
  }
}
