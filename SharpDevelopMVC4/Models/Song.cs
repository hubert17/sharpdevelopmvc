﻿﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using EntityFramework.Utilities;

namespace ASPNETWebApp45.Models
{
    public class Song
    {
        [Ignore]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Duration { get; set; }
        public int PeakChartPosition { get; set; }
        public int ReleaseYear { get; set; }
        public string RecordLabel { get; set; }
        [Ignore]
        public string Genre { get; set; }

        public static void Seed(bool clearSongTable = false)
        {
            var csvFile = GetBillboardCsvFile();
            if (File.Exists(csvFile))
            {
                using (var _db = new MyApp45DbContext())
                {
                    if(clearSongTable)
                        _db.Database.ExecuteSqlCommand("DELETE FROM [" + "Song" + "s]");

                    if (!_db.Songs.Any())
                    {
                        CsvHelper.Configuration.Configuration config = new CsvHelper.Configuration.Configuration
                        {
                            HasHeaderRecord = true
                        };

                        using (var reader = new StreamReader(csvFile))
                        using (var csv = new CsvReader(reader, config))
                        {
                            var songs = csv.GetRecords<Song>().ToList();                            
                            try
                            {
                                EFBatchOperation.For(_db, _db.Songs).InsertAll(songs);
                            }
                            catch (InvalidOperationException)
                            {
                                Hangfire.BackgroundJob.Enqueue(() => InsertAllNormal(songs));
                                //throw new Exception(iopEx.Message + " Seeding song the normal way and this will take sometime but will be processed in the background.");
                            }
                            
                        }
                    }
                    else
                        throw new Exception("Song table is not empty.");
                }
            }
        }

        private static string GetBillboardCsvFile()
        {
            const string BILLBOARD_CSV_FILE = @"BillboardTo2013.csv";
            return Path.IsPathRooted(BILLBOARD_CSV_FILE) ? BILLBOARD_CSV_FILE : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", BILLBOARD_CSV_FILE);
        }

        public static void InsertAllNormal(List<Song> songs)
        {
            using (var _db = new MyApp45DbContext())
            {
                //_db.Songs.AddRange(songs);
                songs.ForEach(song =>
                {
                    _db.Songs.Add(song);
                    _db.SaveChanges();
                });
                
            }
        }

    }
}