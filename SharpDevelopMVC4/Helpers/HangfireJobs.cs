
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPNETWebApp45.Models;
using CsvHelper;
using RestSharp;
public static class HangfireJobsApi45
{
	
    public static List<BingImage> BingPhotos()
    {
    	List<BingImage> newPhotos = new List<BingImage> ();
    	try
    	{
  		  	const string baseUrl = "https://www.bing.com";
			IRestClient client = new RestClient(baseUrl);  
			var request = new RestRequest("/HPImageArchive.aspx?format=js&idx=0&n=8&mkt=en-US", Method.GET);
			var response = client.Execute<BingPhotos>(request);
			newPhotos = response.Data.images;      		
    	}
    	catch {}
		
    	var csvFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "BingPhotos.csv");
    	List<BingImage> bingImages;
        if (System.IO.File.Exists(csvFile))
        {
            using (var reader = new System.IO.StreamReader(csvFile))
            using (var csv = new CsvReader(reader))
            {
                bingImages = csv.GetRecords<BingImage>().ToList();
            }
        }
        else
			bingImages = new List<BingImage>();
        
        bingImages.AddRange(newPhotos);
        bingImages = bingImages.OrderByDescending(o => o.startdate).GroupBy(x => x.startdate).Select(y => y.First()).ToList();
		
        using (var writer = new System.IO.StreamWriter(csvFile))
        using (var csv = new CsvWriter(writer))
        {
            csv.WriteRecords(bingImages);
        }
        
        return bingImages;
    }
    
    public static void RemoveNewsCache(string cacheName = "")
    {
    	HttpContext.Current.Cache.Remove(cacheName);
    }
}