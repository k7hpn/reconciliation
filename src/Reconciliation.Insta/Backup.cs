using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reconciliation.Models.Insta;

[assembly: CLSCompliant(true)]
namespace Reconciliation.Insta
{
    public class Backup
    {
        private ILogger Logger { get; }

        public Backup(ILogger<Backup> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> Parse(string commandName, string path)
        {
            Logger.LogInformation("Performing {CommandName} on backup path: {InstagramPath}",
                commandName,
                path);

            var profileFiles = new List<string>(1);
            var mediaFiles = new List<string>();

            foreach (var directory in Directory.GetDirectories(path))
            {
                Logger.LogInformation("Examining {Directory}", directory);
                foreach (var file in Directory.GetFiles(directory))
                {
                    switch (Path.GetFileName(file))
                    {
                        case "profile.json":
                            profileFiles.Add(file);
                            break;
                        case "media.json":
                            mediaFiles.Add(file);
                            break;
                    }
                }
            }

            if (profileFiles.Count == 0)
            {
                Logger.LogError("Unable to find a profile.json file in the Instagram backup.");
            }
            else
            {
                if (profileFiles.Count > 1)
                {
                    Logger.LogWarning("Found multiple profile.json files in the Instagram backup, using {ProfileFilePath}",
                        profileFiles[0]);
                }

                var profile = await DeserializeInstagram<Profile>(profileFiles[0])
                     .ConfigureAwait(false);

                if (profile != null)
                {
                    Logger.LogInformation("Found profile for user {Username}", profile.Username);
                    Logger.LogInformation("Joined at {JoinedDate}", profile.DateJoined);
                    if (profile.ProfilePictureChanges != null)
                    {
                        foreach (var profileChange in profile.ProfilePictureChanges)
                        {
                            Logger.LogInformation("Changed profile picture at {ProfilePicChanged}",
                                profileChange.UploadTimestamp);
                        }
                    }
                }
            }

            if (mediaFiles.Count == 0)
            {
                Logger.LogError("Unable to find any media.json files in the Instagram backup.");
            }
            else
            {
                Logger.LogInformation("Found {MediaFileCount} media.json files...",
                    mediaFiles.Count);
                int fileCount = 1;
                long firstPhoto = DateTime.Now.Ticks;
                long lastPhoto = 0;
                int photoCount = 0;
                int videoCount = 0;
                foreach (var mediaFile in mediaFiles)
                {
                    var medias = await DeserializeInstagram<MediaFile>(mediaFile)
                        .ConfigureAwait(false);
                    medias.SourceFile = mediaFile;
                    Logger.LogInformation("File {FileCount}: {PhotoCount} photos and {VideoCount} videos",
                        fileCount++,
                        medias.Photos.Length,
                        medias.Videos.Length);
                    photoCount += medias.Photos.Length;
                    videoCount += medias.Videos.Length;
                    firstPhoto = Math.Min(medias.Photos.Min(_ => _.TakenAt).Ticks, firstPhoto);
                    lastPhoto = Math.Max(medias.Photos.Max(_ => _.TakenAt).Ticks, lastPhoto);
                }
                Logger.LogInformation("Total {TotalPhotos} photos, {TotalVideos} videos",
                    photoCount,
                    videoCount);
                Logger.LogInformation("First photo taken at: {FirstPhotoTakenAt}",
                    new DateTime(firstPhoto));
                Logger.LogInformation("Last photo taken at: {LastPhotoTakenAt}",
                    new DateTime(lastPhoto));
            }

            return await Task.FromResult(0).ConfigureAwait(false);
        }

        private async Task<T> DeserializeInstagram<T>(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<T>(stream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }).ConfigureAwait(false);
        }
    }
}
