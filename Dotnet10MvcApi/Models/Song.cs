using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Dotnet10MvcApi.Data;
using CsvHelper.Configuration.Attributes;

namespace Dotnet10MvcApi.Models
{
    public class Song
    {
        [Ignore]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int PeakChartPosition { get; set; }
        public int ReleaseYear { get; set; }
        public string RecordLabel { get; set; } = string.Empty;

        [Ignore]
        [NotMapped]
        public string Genre { get; set; } = string.Empty;

        public static void Seed(ApplicationDbContext db, bool clearSongTable = false)
        {
            var csvFile = GetBillboardCsvFile();
            if (File.Exists(csvFile))
            {
                if (clearSongTable)
                {
                    db.Database.ExecuteSqlRaw("DELETE FROM [Songs]");
                }

                if (!db.Songs.Any())
                {
                    var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true
                    };

                    using (var reader = new StreamReader(csvFile))
                    using (var csv = new CsvReader(reader, config))
                    {
                        var songs = csv.GetRecords<Song>().ToList();
                        
                        // Optimized Raw ADO.NET Transaction Seeding
                        var conn = db.Database.GetDbConnection();
                        bool wasClosed = conn.State == System.Data.ConnectionState.Closed;
                        if (wasClosed)
                        {
                            conn.Open();
                        }

                        using (var transaction = conn.BeginTransaction())
                        {
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.Transaction = transaction;
                                // Use named parameters, as required by the EF Core Jet provider's command parser
                                cmd.CommandText = "INSERT INTO [Songs] ([Title], [Artist], [Duration], [PeakChartPosition], [ReleaseYear], [RecordLabel]) VALUES (@p0, @p1, @p2, @p3, @p4, @p5)";
                                
                                var titleParam = cmd.CreateParameter(); titleParam.ParameterName = "@p0"; titleParam.DbType = System.Data.DbType.String; cmd.Parameters.Add(titleParam);
                                var artistParam = cmd.CreateParameter(); artistParam.ParameterName = "@p1"; artistParam.DbType = System.Data.DbType.String; cmd.Parameters.Add(artistParam);
                                var durationParam = cmd.CreateParameter(); durationParam.ParameterName = "@p2"; durationParam.DbType = System.Data.DbType.String; cmd.Parameters.Add(durationParam);
                                var peakParam = cmd.CreateParameter(); peakParam.ParameterName = "@p3"; peakParam.DbType = System.Data.DbType.Int32; cmd.Parameters.Add(peakParam);
                                var yearParam = cmd.CreateParameter(); yearParam.ParameterName = "@p4"; yearParam.DbType = System.Data.DbType.Int32; cmd.Parameters.Add(yearParam);
                                var labelParam = cmd.CreateParameter(); labelParam.ParameterName = "@p5"; labelParam.DbType = System.Data.DbType.String; cmd.Parameters.Add(labelParam);

                                foreach (var song in songs)
                                {
                                    titleParam.Value = song.Title ?? "";
                                    artistParam.Value = song.Artist ?? "";
                                    durationParam.Value = song.Duration ?? "";
                                    peakParam.Value = song.PeakChartPosition;
                                    yearParam.Value = song.ReleaseYear;
                                    labelParam.Value = song.RecordLabel ?? "";
                                    
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                        }

                        if (wasClosed)
                        {
                            conn.Close();
                        }
                    }
                }
                else
                {
                    throw new Exception("Song table is not empty.");
                }
            }
            else
            {
                throw new FileNotFoundException($"Seed CSV file not found at: {csvFile}");
            }
        }

        private static string GetBillboardCsvFile()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "BillboardTo2013.csv");
            if (!File.Exists(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "BillboardTo2013.csv");
            }
            return path;
        }
    }
}
