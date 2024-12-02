using System.Linq;
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
        return NotFound();
      }

      return movie;
    }

    // PUT: api/Movies/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMovie(int id, Movie movie)
    {
      if (id != movie.Id)
      {
        return BadRequest();
      }

      _context.Entry(movie).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!MovieExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // POST: api/Movies
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Movie>> PostMovie(Movie movie)
    {
      _context.Movies.Add(movie);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(Movie), new { id = movie.Id }, movie);
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportMovies()
    {
      var externalMovies = await FetchMoviesFromApi();

      foreach (var movie in externalMovies)
      {
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
        return NotFound();
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

        var content = await response.Content.ReadAsStreamAsync();
        var externalMovies = await JsonSerializer.DeserializeAsync<List<ExternalMovie>>(content);

        if (externalMovies == null)
        {
          return new List<Movie>();
        }

        var movies = externalMovies.Select(m => new Movie
        {
          ExternalId = m.Id,
          Title = m.Title,
          Director = m.Director,
          Rate = m.Rate,
          Year = m.Year
        }).ToList();

        return movies;
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