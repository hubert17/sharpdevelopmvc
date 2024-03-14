using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using ASPNETWebApp45.Models;
using JWTAuth;
using X.PagedList;

namespace ASPNETWebApp45.Controllers.Api
{
	/// <summary>
	/// Description of SongController.
	/// </summary>
	public class SongController : ApiController
	{
		private readonly MyApp45DbContext _db = new MyApp45DbContext();

		[HttpGet]
		public IHttpActionResult GetAll(int page = 1, int pageSize = 100, string search = "", string artist = "", int? year = null, int? peak = null)
		{
			IQueryable<Song> songs = _db.Songs.AsQueryable();

			if (!string.IsNullOrWhiteSpace(search))
			{
				songs = songs.Where(x =>
							   x.Title.ToLower().Contains(search.ToLower())
							   || x.Artist.ToLower().Contains(search.ToLower())
						  ).OrderBy(o => o.Title);
			}

			if (!string.IsNullOrWhiteSpace(artist))
			{
				songs = songs.Where(x => x.Artist.ToLower() == artist.ToLower());
			}

			if (year != null)
			{
				songs = songs.Where(x => x.ReleaseYear == year.Value);
			}

			if (peak != null)
			{
				songs = songs.Where(x => x.PeakChartPosition <= peak);
			}

			var songPagedList = songs.OrderByDescending(x => x.ReleaseYear).ToPagedList(page, pageSize);

			return Ok(new
			{
				PageNumber = songPagedList.PageNumber,
				PageSize = songPagedList.PageSize,
				TotalItemCount = songPagedList.TotalItemCount,
				TotalPageCount = songPagedList.PageCount,
				Items = songPagedList.ToList()
			});

		}

		[HttpGet]
		public IHttpActionResult Get(int Id)
		{
			var song = _db.Songs.Find(Id);
			if (song != null)
				return Ok(song);
			else
				return BadRequest("Song not found");

		}

		[HttpPost]
		public IHttpActionResult Create([FromBody]Song song)
		{
			_db.Songs.Add(song);
			_db.SaveChanges();
			return Ok(song.Id);
		}

		[ApiAuthorize]
		[HttpPut]
		public IHttpActionResult Update([FromBody]Song updatedSong)
		{
			var exists = _db.Songs.Any(x => x.Id == updatedSong.Id);
			if (exists)
			{
				_db.Entry(updatedSong).State = EntityState.Modified;
				_db.SaveChanges();

				return Ok(updatedSong);
			}
			else
			{
				return BadRequest("Song not found");
			}

		}

		[ApiAuthorize]
		[HttpDelete]
		public IHttpActionResult Delete(int Id)
		{
			var songToDelete = _db.Songs.Find(Id);
			if (songToDelete != null)
			{
				_db.Songs.Remove(songToDelete);
				_db.SaveChanges();
				return Ok("Successfully deleted");
			}
			else
			{
				return BadRequest("Song not found");
			}
		}

		[ApiAuthorize]
		[HttpGet]
		[Route("api/Song/seed")]
		public IHttpActionResult Seed(bool clearSongTable = false)
		{
			try
			{
				Song.Seed(clearSongTable);
				return Ok("Successful seeding of database with Songs.");
			}
			catch (Exception ex)
			{
				return BadRequest("Seeding failed. " + ex.Message);
			}
		}
	}
}