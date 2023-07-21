using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using ASPNETWebApp45.Models;
using CsvHelper;
using gabsCsv = CsvHelper.Configuration;
using Dapper;
using Hangfire;
using NeatEastMusicModel;


namespace ASPNETWebApp45.Controllers.Api
{
	/// <summary>
	/// Description of Poordooytify.
	/// </summary>
	public class PoordooytifyController : ApiController
	{
		[Route("api/poordooytify/add/{apiKey}")]
        [Route("api/sample/poordooytify/add/{apiKey}")]
        [HttpPost]
        public IHttpActionResult GetPoordooytify(List<PoordooytifySong> newSongs, [FromUri]string apiKey)
        {
        	if(apiKey != "GuZULApUN9FjqHDrW8ey4B4uegUW8WB5") return BadRequest();
        	
        	var songs = new List<PoordooytifySong>();
        	var csvFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "poordooytify.csv");
	        if (System.IO.File.Exists(csvFile))
	        {
	            using (var reader = new System.IO.StreamReader(csvFile))
	            using (var csv = new CsvReader(reader))
	            {
	            	songs = csv.GetRecords<PoordooytifySong>().ToList(); // .Where(x => !x.Genre.StartsWith("BSIT", StringComparison.Ordinal))
	            }
	            
				newSongs.Where(x => string.IsNullOrWhiteSpace(x.AudioType) || x.AudioType == "string").ToList().ForEach(x => {
					var url = x.Link.Split('?')[0];
					url = url.Split('.').Last();
					x.AudioType = url.StartsWith("opus", StringComparison.OrdinalIgnoreCase) ? "opus" : "m4a";
				});
	            
    	        songs.AddRange(newSongs);
			                    	
				using (var writer = new System.IO.StreamWriter(csvFile))
				using (var csv = new CsvWriter(writer))
				{
				    csv.WriteRecords(songs);
				}
				
				HttpRuntime.Cache["PoordooytifySongs"] = songs;
				return Ok("success");
	        }
		    else
		    	return BadRequest();        		        	      
        }
        
        [Route("api/poordooytify/fix")]
     	[Route("api/sample/poordooytify/fix")]
        [HttpPost]
        public IHttpActionResult GetPoordooytify(string genre = "", string fixgenre = "", bool fixAudioType = false)
        {  	
        	var songs = new List<PoordooytifySong>();
        	var csvFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "poordooytify.csv");
	        if (System.IO.File.Exists(csvFile))
	        {
	            using (var reader = new System.IO.StreamReader(csvFile))
	            using (var csv = new CsvReader(reader))
	            {
	            	songs = csv.GetRecords<PoordooytifySong>().Where(x => !x.Genre.StartsWith("BSIT", StringComparison.OrdinalIgnoreCase)).ToList();
	            }
	            
	            if (fixAudioType) {
					songs.ForEach(x => {
						var url = x.Link.Split('?')[0];
						url = url.Split('.').Last();
						x.AudioType = url.StartsWith("m4a", StringComparison.OrdinalIgnoreCase) ? "m4a" : "opus";
					});     
				} else if (string.IsNullOrWhiteSpace(genre) || string.IsNullOrWhiteSpace(fixgenre)) {
//					songs.Where(x => !string.IsNullOrWhiteSpace(x.Link) && (string.IsNullOrWhiteSpace(x.Key) || x.Key.Trim().Equals("#NAME?", StringComparison.OrdinalIgnoreCase))).ToList().ForEach(x => {
//						x.Key = x.Link.Split('/')[4]; // System.Convert.ToBase64String(Guid.NewGuid().ToByteArray());
//					}); 
//					songs.Where(x => x.Artist == "#NAME?").ToList().ForEach(x => {
//						x.Artist = "Mykz";
//					});
	            	                                                        
					songs = songs.Where(x => !string.IsNullOrWhiteSpace(x.Key) && !string.IsNullOrWhiteSpace(x.Link) && !string.IsNullOrWhiteSpace(x.Title)).ToList();
				} else if (!string.IsNullOrWhiteSpace(genre)) {
					songs.Where(x => x.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase)).ToList().ForEach(x => {
						x.Genre = fixgenre;
					});       	
				}
	                                                                                                         			                  
				using (var writer = new System.IO.StreamWriter(csvFile))
				using (var csv = new CsvWriter(writer))
				{
				    csv.WriteRecords(songs);
				}
				
				HttpRuntime.Cache["PoordooytifySongs"] = songs;		
				HttpRuntime.Cache.Remove("PoordooytifyGenres");
				return Ok(new { msg = songs.Count() + " songs fixed and updated.", genres = songs.Select(s => s.Genre).Distinct().ToList()});
	        }
		    else
		    	return BadRequest();        		        	      
        }        
        
		//[Authorize]
        [HttpGet]
        [Route("api/poordooytify")]
        [Route("api/sample/poordooytify")]
        public IHttpActionResult GetPoordooytify(string search = "", string id ="", string genre = "", int page = 1, int pageSize = 20, string picUrl = "/img/mykzpic.jpg", int opusonly = 0)
        {
        	var songs = new List<PoordooytifySong>();
        	var genres = new List<string>();
        	if(HttpRuntime.Cache["PoordooytifySongs"] == null)
        	{
	        	var csvFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "poordooytify.csv");
		        if (System.IO.File.Exists(csvFile))
		        {
		            using (var reader = new System.IO.StreamReader(csvFile))
		            using (var csv = new CsvReader(reader))
		            {
		            	songs = csv.GetRecords<PoordooytifySong>().Where(x => !x.Genre.StartsWith("BSIT", StringComparison.OrdinalIgnoreCase)).ToList();;
		            	HttpRuntime.Cache["PoordooytifySongs"] = songs;
		            	
		            	genres = songs.Select(s => s.Genre).Distinct().ToList();
		            	genres = string.Join(",", genres).ToUpper().Split(',').Distinct().Where(x => !string.IsNullOrWhiteSpace(x) || x != "NULL").ToList();
		            	HttpRuntime.Cache["PoordooytifyGenres"] = genres;
		            }
		        }
        	}
        	else
        	{
        		songs = (List<PoordooytifySong>)HttpRuntime.Cache["PoordooytifySongs"];
        		genres = (List<string>)HttpRuntime.Cache["PoordooytifyGenres"];
        	}

        	if(opusonly == 1)
        		songs = songs.Where(x => x.AudioType != "m4a").ToList();
        	
        	var rnd = new Random();
        	picUrl = string.IsNullOrEmpty(picUrl) ? "/img/mykzpic.jpg" : picUrl;
        	var resultType = "search";        	
			if (!string.IsNullOrWhiteSpace(search)) {
				var songresult = songs.Where(x => x.Title.ToLower().Contains(search.ToLower()) || x.Artist.ToLower().Contains(search.ToLower())).Skip(pageSize * (page - 1)).Take(pageSize).OrderBy(o => o.Title).ThenBy(d => d.Artist).ToList();        		
//				if (songresult.Count() < pageSize) {
//					var keywords = search.Split(' ');
//					var keys = songresult.Select(s => s.Key).ToList();
//					keywords.ToList().ForEach(k => {
//						if (!string.IsNullOrWhiteSpace(k)) {
//							k = k.Trim().ToLower();
//							var key = songs.Where(x => keys.All(a => a != x.Key) && x.Title.ToLower().Contains(k) || x.Artist.ToLower().Contains(k)).Select(s => s.Key).ToList();
//							keys.AddRange(key);
//						}        			                          	
//					});
//					songresult.AddRange(songs.Where(x => keys.Contains(x.Key)).ToList());
//				}
				songs = songresult;
			}          	
        	else if(!string.IsNullOrWhiteSpace(id))
        	{
        		songs = songs.Where(x => x.Key == id).ToList();
        	}  
        	else if(!string.IsNullOrWhiteSpace(genre))
        	{
        		songs = songs.Where(x => x.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase)).OrderBy(x => rnd.Next()).Take(pageSize).ToList();
        		resultType = "genre";
        	}        		
        	else
        	{        		
        		songs = songs.OrderBy(x => rnd.Next()).ToList();
        		if(page > 0)
					songs = songs.Skip(pageSize * (page - 1) ).Take(pageSize).ToList();
        		resultType = "general";
        	}        		         
        	
        	var neatEastMusic = new NeatEaseMusic();
        	if(resultType == "search")
        	{
        		neatEastMusic.songs = songs.AsEnumerable().Select(s => new SearchSongItem{
	        	                            	id = s.Key,
	        	                            	name = s.Title,
	        	                            	artists = new List<Artist>( new [] { new Artist{name = s.Artist, img1v1Url = picUrl} }),
	        	                            	album = new AlbumMeta { id = s.Key, name = s.Title},
	        	                            	al = new Al { id = s.Key, name = s.Title, picUrl = picUrl},
	        	                            	ar = new List<Ar>( new [] { new Ar{name = s.Artist} }),
	        	                            	audioType = s.AudioType
	        	                            }).ToList();     
        	}
        	else
        	{	        	
	        	var tracks = songs.AsEnumerable().Select(s => new Track{
	        	                            	id = s.Key,
	        	                            	name = s.Title,
	        	                            	ar = new List<Ar>( new [] { new Ar{name = s.Artist} }),
	        	                            	al = new Al { id = s.Key, name = s.Title, picUrl = picUrl},
	        	                            	audioType = s.AudioType,
												originSongSimpleData = new OriginSongSimpleData
												{
													songId = s.Key,
													name = s.Title,
													artists = new List<Artist>( new [] { new Artist{name = s.Artist} })   
												}
	        	                            }).ToList();        		
	        	neatEastMusic.playlist = new Playlist
	        	{
	        		tracks = tracks
	        	};     
        	}   	
        		
            return Ok(neatEastMusic);
	        
        }  
        
        [AllowAnonymous]
        [HttpGet]
        [Route("api/poordooytify/DropboxSongData")]
        [Route("api/sample/DropboxSongData")]
        public async Task<HttpResponseMessage> GetSongData(string id)
        {
        	var songs = new List<PoordooytifySong>();
        	if(HttpRuntime.Cache["PoordooytifySongs"] == null)
        	{
	        	var csvFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "poordooytify.csv");
		        if ( System.IO.File.Exists(csvFile))
		        {
		            using (var reader = new System.IO.StreamReader(csvFile))
		            using (var csv = new CsvReader(reader))
		            {
		            	songs = csv.GetRecords<PoordooytifySong>().ToList();
		            }
		        }
        	}
        	else
        	{
        		songs = (List<PoordooytifySong>)HttpRuntime.Cache["PoordooytifySongs"];
        	}
        	
        	var Link = songs.FirstOrDefault(x => x.Key == id).Link;
        	var url = Link.Remove(Link.Length -1, 1) + "1";
        	var uri = new Uri(url);
			HttpClient client = new HttpClient();
			return await client.GetAsync(uri);		
        }         

        [Route("api/poordooytify/genre")]
        [Route("api/sample/poordooytify/genre")]
        [HttpGet]
        public IHttpActionResult GetPoordooytify()
        {
        	var genres = new List<string>();
        	if(HttpRuntime.Cache["PoordooytifyGenres"] == null)
        	{
	        	var csvFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "poordooytify.csv");
		        if (System.IO.File.Exists(csvFile))
		        {
		            using (var reader = new System.IO.StreamReader(csvFile))
		            using (var csv = new CsvReader(reader))
		            {
		            	var songs = csv.GetRecords<PoordooytifySong>().Where(x => !x.Genre.StartsWith("BSIT", StringComparison.Ordinal)).ToList();;
		            	genres = songs.Select(s => s.Genre).Distinct().ToList();
		            	genres = string.Join(",", genres).ToUpper().Split(',').Distinct().Where(x => !string.IsNullOrWhiteSpace(x) || x != "NULL").ToList();
		            	HttpRuntime.Cache["PoordooytifyGenres"] = genres;
		            }
		        }
        	}
        	else
        	{
        		genres = (List<string>)HttpRuntime.Cache["PoordooytifyGenres"];
        	}
        	
        	return Ok(genres);
     		        	      
        }        
   
              
	}
}