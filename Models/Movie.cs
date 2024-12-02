namespace mojefilmy_softwarestudio_be.Models
{
  public class Movie
  {
    public Int32 Id { get; set; }
    public Int32? ExternalId { get; set; }
    public string? Title { get; set; }
    public string? Director { get; set; }
    public float Rate { get; set; }
    public Int32 Year { get; set; }
  }

  public class ExternalMovie
  {
    public Int32 Id { get; set; }
    public string? Title { get; set; }
    public string? Director { get; set; }
    public float Rate { get; set; }
    public Int32 Year { get; set; }
  }
}
