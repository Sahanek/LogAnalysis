using LogAnalysis.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

var accessLog = File.ReadAllLines("access-log.txt").ToLogItem();

var photographersText = await File.ReadAllTextAsync("photographers.json");
var photographers = JsonSerializer.Deserialize<IEnumerable<Photographer>>(photographersText);

var monthlyStat = accessLog.CalculateMonthlyStatistic();

foreach (var item in monthlyStat)
{
    Console.WriteLine(item.Url + ":");
    foreach (var stats in item.Data)
    {
        Console.WriteLine($"\t{stats.Month}: {stats.Downloads}");
    }

}

var hourlyStat = accessLog.CalculateHourlyStatistic();

foreach (var item in hourlyStat)
{
    Console.WriteLine(item.Url + ":");
    foreach (var stats in item.Data)
    {
        Console.WriteLine($"\t{stats.Hour}: {stats.Percentage:0.00} %");
    }

}

var photographStats = accessLog.CalculatePhotographStatistic(photographers);

foreach (var item in photographStats)
{
    Console.WriteLine($"{item.Photographer} : {item.Downloads}");
}
