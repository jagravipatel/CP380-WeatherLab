using System;
using System.Linq;


namespace WeatherLab
{
    class Program
    {
        static string dbfile = @".\data\climate.db";

        static void Main(string[] args)
        {
            var measurements = new WeatherSqliteContext(dbfile).Weather;

            var total_2020_precipitation = measurements.Where(m => m.year == 2020)
                                        .Select(m => m.precipitation)
                                        .Sum();
            Console.WriteLine($"Total precipitation in 2020: {total_2020_precipitation} mm\n");

            //
            // Heating Degree days have a mean temp of < 18C
            //   see: https://en.wikipedia.org/wiki/Heating_degree_day
            //

            var days = measurements.GroupBy(y => y.year)
                   .Select(sel => new
                   {
                       year = sel.Key,
                       hdd = sel.Where(hdd => hdd.meantemp < 18).Count(),
                       cdd = sel.Where(cdd => cdd.meantemp >= 18).Count()
                   });
            //
            // Cooling degree days have a mean temp of >=18C
            //

            //
            // Most Variable days are the days with the biggest temperature
            // range. That is, the largest difference between the maximum and
            // minimum temperature
            //
            // Oh: and number formatting to zero pad.
            // 
            // For example, if you want:
            //      var x = 2;
            // To display as "0002" then:
            //      $"{x:d4}"
            //
            Console.WriteLine("Year\tHDD\tCDD");

            foreach (var d in days)
            {   Console.WriteLine($"{ d.year }\t{ d.hdd }\t{ d.cdd }");     }

            Console.WriteLine("\nTop 5 Most Variable Days");
            Console.WriteLine("YYYY-MM-DD\tDelta");

            // ?? TODO ??
            var display = measurements.Select(sel => new
                    {
                        date = $"{sel.year}-{sel.month:d2}-{sel.day:d2}",
                        delta = (sel.maxtemp - sel.mintemp)
                    })
                    .OrderByDescending(d => d.delta);

            int check = 0;
            foreach (var i in display)
            {   if (check < 5) {
                    Console.WriteLine($"{i.date}\t{i.delta}");
                    check++;
                }
                else
                { break; }
            }
        }
    }
}
