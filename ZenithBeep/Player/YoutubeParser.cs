﻿using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace ZenithBeep.Player
{
    internal class YoutubeParser
    {
        private static Regex RgxTimestampMatch = new Regex(@"(?<ts>(?:[\d]{2}:[\d]{2}:[\d]{2}|[\d]{2}:[\d]{2}))", RegexOptions.Compiled);

        private static HttpClient HttpClient = new HttpClient();

        private static IConfigurationRoot? _configuration;
        private readonly IServiceProvider _service;

        public YoutubeParser(IServiceProvider service)
        {
            _configuration = service.GetRequiredService<IConfigurationRoot>();
        }

        private static async Task<(bool success, SortedList<TimeSpan, IVideoChapter>? chapters)> ParseChapters(string videoId, CancellationToken cancellationToken = default)
        {
            var Api_opration = _configuration["YOUTUBE_OPERATIONAL_API"];
            string apiEnd_point = string.Format("{0}/videos?part=snippet,chapters&id={1}", Api_opration, videoId);
            var response = await HttpClient.GetAsync(apiEnd_point, cancellationToken);
            var json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(json))
            {
                return (false, null);
            }

            IYoutubeRequest? yt = JsonConvert.DeserializeObject<IYoutubeRequest?>(json);
            if (yt == null) return (false, null);
            if (yt.Error != null) return (false, null);
            if (yt.Items.Count == 0) return (false, null);

            var slTracks = new SortedList<TimeSpan, IVideoChapter>();
            var videoItem = yt.Items[0];

            foreach (var chapter in videoItem.Chapters.Chapters)
            {
                var timespan = TimeSpan.FromSeconds(chapter.Time);
                slTracks.Add(timespan, chapter);
            }

            if (slTracks.Count == 0)
            {
                var splitDescription = videoItem.Snippet.Description
                    .Split('\n')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();

                foreach (var line in splitDescription)
                {
                    var matchTs = RgxTimestampMatch.Match(line);
                    if (matchTs.Success)
                    {
                        var ts = matchTs.Value.TryParseTimeStamp();
                        if (ts == null) continue;

                        slTracks.Add(ts.Value, new IVideoChapter() { Title = line, Time = (int)ts.Value.TotalSeconds, Thumbnails = new List<IThumbnail>() });
                    }
                }
            }

            return (slTracks.Count != 0, slTracks);
        }

        public class IVideoChapter
        {
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("time")]
            public int Time { get; set; }
            [JsonProperty("thumbnails")]
            public List<IThumbnail> Thumbnails { get; set; }
        }

        public class IThumbnail
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("width")]
            public int Width { get; set; }

            [JsonProperty("height")]
            public int Height { get; set; }
        }

        public class IYoutubeRequest
        {
            [JsonProperty("kind")]
            public string Kind { get; set; }
            [JsonProperty("etag")]
            public string Etag { get; set; }

            [JsonProperty("items")]
            public List<IItem> Items { get; set; }
            [JsonProperty("error")]
            public IError Error { get; set; }
        }

        public class IError
        {
            [JsonProperty("code")]
            public int Code { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }

        public class IItem
        {
            [JsonProperty("kind")]
            public string Kind { get; set; }

            [JsonProperty("etag")]
            public string Etag { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("chapters")]
            public IChapterDefinition Chapters { get; set; }

            [JsonProperty("snippet")]
            public ISnippet Snippet { get; set; }
        }

        public class IChapterDefinition
        {

            [JsonProperty("areAutoGenerated")]
            public bool AreAutoGenerated { get; set; }

            [JsonProperty("chapters")]
            public List<IVideoChapter> Chapters { get; set; }
        }
        public class ISnippet
        {
            [JsonProperty("publishedAt")]
            public object PublishedAt { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }
        }

    }
}
