using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reconciliation.Models;
using Reconciliation.Models.Insta;

namespace Reconciliation.Output
{
    public class Markdown
    {
        private ILogger Logger { get; }

        public Markdown(ILogger<Markdown> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task GeneratePostsAsync(string commandName,
            string path,
            IList<MediaFile> media)
        {
            int filesProcessedCount = 0;
            int totalEntriesProcessed = 0;
            foreach (var mediaSource in media.OrderBy(_ => _.EarliestPhoto))
            {
                int fileEntriesProcessedCount = 0;
                Logger.LogInformation("Generating Markdown for: {MediaFile}", mediaSource.SourceFile);
                var mediaBasePath = Path.GetDirectoryName(mediaSource.SourceFile);
                foreach (var photo in mediaSource.Photos.OrderBy(_ => _.TakenAt))
                {
                    string caption = string.IsNullOrEmpty(photo.Caption)
                        ? "untitled"
                        : photo.Caption;

                    string masterSlug = LowerAscii(TruncateSlug(caption));
                    string slug = null;
                    string fileName = null;
                    int suffix = 0;

                    bool fileExists = true;

                    while (fileExists)
                    {
                        slug = suffix == 0
                            ? masterSlug
                            : masterSlug + '-' + suffix.ToString();
                        fileName = $"{photo.TakenAt:yyyyMMdd}-{slug}.md";
                        suffix++;
                        fileExists = File.Exists(Path.Combine(path, "content", "posts", fileName));
                    }

                    string extension = Path.GetExtension(photo.Path).ToLower();
                    string photoDestination = Path.Combine(path,
                        "static",
                        "photos",
                        $"{photo.TakenAt:yyyyMM}");

                    if (!Directory.Exists(photoDestination))
                    {
                        Directory.CreateDirectory(photoDestination);
                    }

                    photoDestination = Path.Combine(photoDestination, $"{slug}{extension}");

                    string photoWebPath = $"/photos/{photo.TakenAt:yyyyMM}/{slug}{extension}";

                    var fm = new FrontMatter
                    {
                        Date = photo.TakenAt,
                        Location = photo.Location,
                        Title = caption,
                        Images = new List<string>
                        {
                            photoWebPath
                        }
                    };
                    string fullPath = Path.Combine(path, "content", "posts", fileName);
                    //Logger.LogInformation("Writing out {FileName}...", fileName);
                    File.WriteAllText(fullPath, JsonSerializer.Serialize(fm,
                        new JsonSerializerOptions
                        {
                            IgnoreNullValues = true
                        })
                        + Environment.NewLine
                        + $"![{caption}]({photoWebPath})");
                    string photoFilePath = photo.Path.Replace('/', Path.DirectorySeparatorChar);
                    //Logger.LogInformation("Copying image {PhotoPath} to {Destination}...",
                    //    photoFilePath,
                    //    photoDestination);
                    string sourcePhotoFullPath = Path.Combine(mediaBasePath, photoFilePath);
                    if (!File.Exists(sourcePhotoFullPath))
                    {
                        Logger.LogError("Could not find referenced file: {FilePath}",
                            sourcePhotoFullPath);
                    }
                    else
                    {
                        File.Copy(sourcePhotoFullPath,
                            Path.Combine(path, "static", photoDestination),
                            true);
                    }
                    fileEntriesProcessedCount++;
                }
                Logger.LogInformation("Processed {EntriesProcessedCount} entries of {TotalMedia}",
                    fileEntriesProcessedCount,
                    mediaSource.Photos.Length);
                totalEntriesProcessed += fileEntriesProcessedCount;
                filesProcessedCount++;
            }
            Logger.LogInformation("Processed {EntryProcessedCount} images from {FilesProcessedCount} backup files",
                totalEntriesProcessed,
                filesProcessedCount);
        }

        private string TruncateSlug(string title)
        {
            return TruncateSlug(title, 50);
        }

        private string TruncateSlug(string title, int truncateAt)
        {
            var fixedTitle = title.Replace("  ", " ").Trim();

            string slug = fixedTitle.Length > truncateAt
                ? fixedTitle.Substring(0, truncateAt)
                : fixedTitle;

            if (slug.Length < truncateAt)
            {
                return slug.Trim();
            }

            var lastSpace = slug.LastIndexOf(' ');
            if (lastSpace < 0)
            {
                return slug.Substring(0, Math.Min(slug.Length, truncateAt)).Trim();
            }
            return slug.Substring(0, lastSpace).Trim();
        }

        private string LowerAscii(string fullString)
        {
            var builder = new StringBuilder(fullString.Length);
            foreach (char c in fullString)
            {
                if ((int)c == 32)
                {
                    builder.Append('-');
                    continue;
                }
                if (
                    ((int)c > 47 && (int)c < 58)
                    || ((int)c > 64 && (int)c < 91)
                    || ((int)c > 96 && (int)c < 123))
                {
                    builder.Append(c);
                }
            }
            return builder.ToString().ToLowerInvariant().TrimEnd('-');
        }
    }
}