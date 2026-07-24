using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dotnet10MvcApi.Data;
using Dotnet10MvcApi.Models;
using Dotnet10MvcApi.Models.Entities;

namespace Dotnet10MvcApi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public SongController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 100, string search = "", string artist = "", int? year = null, int? peak = null)
        {
            IQueryable<Song> songs = _db.Songs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                songs = songs.Where(x => 
                    x.Title.ToLower().Contains(lowerSearch) || 
                    x.Artist.ToLower().Contains(lowerSearch)
                );
            }

            if (!string.IsNullOrWhiteSpace(artist))
            {
                var lowerArtist = artist.ToLower();
                songs = songs.Where(x => x.Artist.ToLower() == lowerArtist);
            }

            if (year != null)
            {
                songs = songs.Where(x => x.ReleaseYear == year.Value);
            }

            if (peak != null)
            {
                songs = songs.Where(x => x.PeakChartPosition <= peak.Value);
            }

            var totalItems = await songs.CountAsync();
            
            var items = await songs
                .OrderByDescending(x => x.ReleaseYear)
                .ThenBy(x => x.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return Ok(new
            {
                PageNumber = page,
                PageSize = pageSize,
                TotalItemCount = totalItems,
                TotalPageCount = totalPages,
                Items = items
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var song = await _db.Songs.FindAsync(id);
            if (song != null)
                return Ok(song);
            
            return BadRequest("Song not found");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Song song)
        {
            _db.Songs.Add(song);
            await _db.SaveChangesAsync();
            return Ok(song.Id);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Song updatedSong)
        {
            var exists = await _db.Songs.AnyAsync(x => x.Id == updatedSong.Id);
            if (exists)
            {
                _db.Entry(updatedSong).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return Ok(updatedSong);
            }
            
            return BadRequest("Song not found");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var songToDelete = await _db.Songs.FindAsync(id);
            if (songToDelete != null)
            {
                _db.Songs.Remove(songToDelete);
                await _db.SaveChangesAsync();
                return Ok("Successfully deleted");
            }
            
            return BadRequest("Song not found");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("seed")]
        public IActionResult Seed(bool clearSongTable = false)
        {
            try
            {
                Song.Seed(_db, clearSongTable);
                return Ok("Successful seeding of database with Songs.");
            }
            catch (Exception ex)
            {
                return BadRequest("Seeding failed. " + ex.Message);
            }
        }
    }
}
