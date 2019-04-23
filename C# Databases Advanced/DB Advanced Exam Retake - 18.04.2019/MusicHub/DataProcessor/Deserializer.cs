using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MusicHub.Data.Models;
using MusicHub.Data.Models.Enums;
using MusicHub.DataProcessor.ImportDtos;
using Newtonsoft.Json;

namespace MusicHub.DataProcessor
{
    using System;

    using Data;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data";

        private const string SuccessfullyImportedWriter
            = "Imported {0}";
        private const string SuccessfullyImportedProducerWithPhone
            = "Imported {0} with phone: {1} produces {2} albums";
        private const string SuccessfullyImportedProducerWithNoPhone
            = "Imported {0} with no phone number produces {1} albums";
        private const string SuccessfullyImportedSong
            = "Imported {0} ({1} genre) with duration {2}";
        private const string SuccessfullyImportedPerformer
            = "Imported {0} ({1} songs)";

        public static string ImportWriters(MusicHubDbContext context, string jsonString)
        {
            var writers = JsonConvert.DeserializeObject<Writer[]>(jsonString);

            var validWriters = new List<Writer>();
            var sb = new StringBuilder();

            foreach (var writer in writers)
            {
                if (IsValid(writer) && writer.Songs.All(IsValid))
                {
                    validWriters.Add(writer);
                    sb.AppendLine(string.Format(SuccessfullyImportedWriter, writer.Name));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.Writers.AddRange(validWriters);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProducersAlbums(MusicHubDbContext context, string jsonString)
        {
            var producerAlbums = JsonConvert.DeserializeObject<ImportProducerAndAlbumDto[]>(jsonString);

            var validProducerAlbums = new List<Producer>();
            var sb = new StringBuilder();

            foreach (var producerAlbum in producerAlbums)
            {
                var albums = new List<Album>();

                if (IsValid(producerAlbum) &&
                    producerAlbum.Albums.All(IsValid) &&
                    producerAlbum.PhoneNumber == null)
                {
                    foreach (var album in producerAlbum.Albums)
                    {
                        albums.Add(new Album()
                        {
                            Name = album.Name,
                            ReleaseDate = DateTime.ParseExact(album.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                        });
                    }

                    sb.AppendLine(string.Format(SuccessfullyImportedProducerWithNoPhone,
                        producerAlbum.Name,
                        producerAlbum.Albums.Count()));

                    validProducerAlbums.Add(new Producer()
                    {
                        Name = producerAlbum.Name,
                        Pseudonym = producerAlbum.Pseudonym,
                        PhoneNumber = producerAlbum.PhoneNumber,
                        Albums = albums
                    });
                }
                else if (IsValid(producerAlbum) &&
                         producerAlbum.Albums.All(IsValid) &&
                         producerAlbum.PhoneNumber != null)
                {
                    foreach (var album in producerAlbum.Albums)
                    {
                        albums.Add(new Album()
                        {
                            Name = album.Name,
                            ReleaseDate = DateTime.ParseExact(album.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                        });
                    }

                    sb.AppendLine(string.Format(SuccessfullyImportedProducerWithPhone,
                        producerAlbum.Name,
                        producerAlbum.PhoneNumber,
                        producerAlbum.Albums.Count()));

                    validProducerAlbums.Add(new Producer()
                    {
                        Name = producerAlbum.Name,
                        Pseudonym = producerAlbum.Pseudonym,
                        PhoneNumber = producerAlbum.PhoneNumber,
                        Albums = albums
                    });
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.Producers.AddRange(validProducerAlbums);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSongs(MusicHubDbContext context, string xmlString)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportSongDto[]), new XmlRootAttribute("Songs"));

            var songDto = (ImportSongDto[])xml.Deserialize(new StringReader(xmlString));

            var songs = new List<Song>();

            var sb = new StringBuilder();

            foreach (var song in songDto)
            {
                var genre = Enum.TryParse(song.Genre, out Genre Genree);
                var isValidAlbum = context.Albums.FirstOrDefault(a => a.Id == song.AlbumId) != null;
                var writersCount = context.Writers.FirstOrDefault(a => a.Id == song.WriterId) != null;

                if (IsValid(song) && genre && isValidAlbum && writersCount)
                {
                    var splittedDuration = song.Duration.Split(':');
                    songs.Add(new Song()
                    {
                        Name = song.Name,
                        Duration = new TimeSpan(int.Parse(splittedDuration[0]),
                            int.Parse(splittedDuration[1]),
                            int.Parse(splittedDuration[2])),
                        CreatedOn = DateTime.ParseExact(song.CreatedOn, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Genre = Enum.Parse<Genre>(song.Genre),
                        AlbumId = song.AlbumId,
                        WriterId = song.WriterId,
                        Price = song.Price
                    });

                    sb.AppendLine(string.Format(SuccessfullyImportedSong, song.Name, song.Genre, song.Duration));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.Songs.AddRange(songs);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSongPerformers(MusicHubDbContext context, string xmlString)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ImportSongPerformerDto[]), new XmlRootAttribute("Performers"));

            var performerDtos = (ImportSongPerformerDto[])xml.Deserialize(new StringReader(xmlString));

            var performersSongs = new List<Performer>();

            var sb = new StringBuilder();

            foreach (var performer in performerDtos)
            {
                bool isValidSong = false;
                int falseCounter = 0;

                foreach (var item in performer.PerformersSongs)
                {
                    isValidSong = context.Songs.FirstOrDefault(s => s.Id == item.Id) != null;
                    if (!isValidSong)
                    {
                        falseCounter++;
                    }
                }

                if (IsValid(performer) && falseCounter == 0)
                {
                    falseCounter = 0;
                    performersSongs.Add(new Performer()
                    {
                        FirstName = performer.FirstName,
                        LastName = performer.LastName,
                        Age = performer.Age,
                        NetWorth = performer.NetWorth,
                        PerformerSongs = performer.PerformersSongs.Select(ps => new SongPerformer()
                        {
                            SongId = ps.Id
                        })
                        .ToArray()
                    });

                    sb.AppendLine(string.Format(SuccessfullyImportedPerformer, performer.FirstName, performer.PerformersSongs.Count()));
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                }
            }

            context.Performers.AddRange(performersSongs);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);

            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext,
                validationResults, true);
        }
    }
}