using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mojefilmy_softwarestudio_be.Models;

namespace mojefilmy_softwarestudio_be.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class MoviesController : ControllerBase
  {
    private readonly MovieContext _context;
    private readonly HttpClient _client;

    public MoviesController(MovieContext context, IHttpClientFactory clientFactory)
    {
      _context = context;
      _client = clientFactory.CreateClient();
    }

    // GET: api/Movies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
    {
      return await _context.Movies.ToListAsync();
    }

    // GET: api/Movies/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Movie>> GetMovie(int id)
    {
      var movie = await _context.Movies.FindAsync(id);

      if (movie == null)
      {
        return NotFound($"An error occurred while getting movie with id={id}: Not found.");
      }

      return movie;
    }

    // PUT: api/Movies/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMovie(int id, CreateMovieDTO updatedMovie)
    {
      var existingMovie = await _context.Movies.FindAsync(id);

      if (existingMovie == null)
      {
        return NotFound($"Movie with id={id} not found.");
      }

      _context.Entry(existingMovie).State = EntityState.Modified;

      existingMovie.Title = updatedMovie.Title;
      existingMovie.Director = updatedMovie.Director;
      existingMovie.Rate = updatedMovie.Rate;
      existingMovie.Year = updatedMovie.Year;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!MovieExists(id))
        {
          return NotFound($"An error occurred while getting movie with id={id}: Not found.");
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // POST: api/Movies
    [HttpPost]
    public async Task<ActionResult<Movie>> PostMovie(CreateMovieDTO createMovie)
    {
      if (string.IsNullOrEmpty(createMovie.Title) || string.IsNullOrEmpty(createMovie.Director))
      {
        return BadRequest("Title and Director are required fields.");
      }

      var movie = new Movie
      {
        Title = createMovie.Title,
        Director = createMovie.Director,
        Rate = createMovie.Rate,
        Year = createMovie.Year,
        ExternalId = null
      };

      _context.Movies.Add(movie);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportMovies()
    {
      var externalMovies = await FetchMoviesFromApi();

      foreach (var movie in externalMovies)
      {
        if (string.IsNullOrEmpty(movie.Title) || string.IsNullOrEmpty(movie.Director))
        {
          continue;
        }

        bool exists = await _context.Movies.AnyAsync(m => m.ExternalId == movie.ExternalId);

        if (!exists)
        {
          _context.Movies.Add(movie);
        }
      }

      await _context.SaveChangesAsync();

      return Ok();
    }

    // DELETE: api/Movies/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
      var movie = await _context.Movies.FindAsync(id);
      if (movie == null)
      {
        return NotFound($"An error occurred while getting movie with id={id}: Not found.");
      }

      _context.Movies.Remove(movie);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool MovieExists(int id)
    {
      return _context.Movies.Any(e => e.Id == id);
    }

    private async Task<List<Movie>> FetchMoviesFromApi()
    {
      // Fetch movies from external API
      string externalUrl = "https://filmy.programdemo.pl/MyMovies";

      try
      {
        var response = await _client.GetAsync(externalUrl);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var externalMovies = JsonSerializer.Deserialize<List<ExternalMovie>>(content, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });

        if (externalMovies == null)
        {
          Console.WriteLine("No movies found in the external API response.");
          return new List<Movie>();
        }

        var movies = externalMovies
          .Where(m => !string.IsNullOrEmpty(m.Title) && !string.IsNullOrEmpty(m.Director))
          .Select(m => new Movie
          {
            ExternalId = m.Id,
            Title = m.Title ?? "Untitled",
            Director = m.Director ?? "Unknown",
            Rate = m.Rate,
            Year = m.Year
          }).ToList();

        return movies;
      }
      catch (JsonException jsonEx)
      {
        Console.WriteLine($"JSON Deserialization Error: {jsonEx.Message}");
        Environment.Exit(-1);
        return new List<Movie>();
      }
      catch (HttpRequestException httpEx)
      {
        Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
        Environment.Exit(-1);

        return new List<Movie>();
      }
      catch (Exception ex)
      {
        var errorString = nameof(_client) + ex.Message;

        Console.WriteLine(errorString);
        Environment.Exit(-1);

        return new List<Movie>();
      }
    }
  }
}