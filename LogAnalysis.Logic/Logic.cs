using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace LogAnalysis.Logic
{
    public record LogItem(string Url, DateTime DownloadTimestamp)
    {
        public LogItem(string url, string downloadDate, string downloadTime)
            : this(url, DateTime.Parse(downloadDate) + TimeSpan.Parse(downloadTime))
        {
        }
    }

    public record MonthlyStatisticItem(string Month, int Downloads);

    public record HourlyStatisticItem(string Hour, float Percentage);

    public record Statistic<T>(string Url, IEnumerable<T> Data);

    public class Photographer
    {
        [JsonPropertyName("pic")]
        public string Picture { get; set; } = string.Empty;

        [JsonPropertyName("takenBy")]
        public string TakenBy { get; set; } = string.Empty;
    }

    public static class LogAnalyzer
    {
        public static IEnumerable<LogItem> ToLogItem(this IEnumerable<string> lines, string? linesFilter = null) =>
            lines.Skip(1)
                .Where(line => !string.IsNullOrEmpty(line))
                .Select(line => line.Split('\t'))
                .Where(f => linesFilter is null || f[0] == linesFilter)
                .Select(line => new LogItem(line[0], line[1], line[2]));


        public static IEnumerable<Statistic<MonthlyStatisticItem>> CalculateMonthlyStatistic(this IEnumerable<LogItem> logItems) =>
            logItems.GroupBy(log => log.Url)
                .OrderBy(g => g.Key)
                .Select(g => new Statistic<MonthlyStatisticItem>
                (g.Key, g.GroupBy(g => g.DownloadTimestamp.ToString("yyyy-MM"))
                        .OrderBy(o => o.Key)
                        .Select(m => new MonthlyStatisticItem(m.Key, m.Count()))
                ));
        public static IEnumerable<Statistic<HourlyStatisticItem>> CalculateHourlyStatistic(this IEnumerable<LogItem> logItems) =>
            logItems.GroupBy(log => log.Url)
                .OrderBy(g => g.Key)
                .Select(g => new Statistic<HourlyStatisticItem>
                (g.Key, g.GroupBy(g => g.DownloadTimestamp.ToString("HH:00"))
                        .OrderBy(o => o.Key)
                        .Select(m => new HourlyStatisticItem(m.Key, (float)m.Count() / g.Count() * 100))
                ));

        public static IEnumerable<(string Photographer, int Downloads)> CalculatePhotographStatistic(
            this IEnumerable<LogItem> logItems, IEnumerable<Photographer> photographers) => 
            logItems.GroupBy(log => log.Url)
                .Select(group => (Photographer: photographers.First(p => p.Picture == group.Key).TakenBy,
                    Downloads: group.Count()))
                .OrderByDescending(x => x.Downloads);

    }


}
