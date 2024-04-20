using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace flooddetection
{
    class Program
    {
        static void Main(string[] args)
        {

            string dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            //Console.WriteLine(dataDirectory);
            string deviceDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data\\Device");
            var devices = Directory.GetFiles(deviceDirectory, "*.csv");
            foreach (var device in devices)
            {
                ProcessRainfallData(dataDirectory, device);
            }
        }

        static void ProcessRainfallData(string folderPath, string device)
        {
          
            var devicesrs = File.ReadAllLines(device).Skip(1).Select(line =>
            {
                var parts = line.Split(',');
                return new { ID = uint.Parse(parts[0].Trim()) };
            }).ToList();

            var files = Directory.GetFiles(folderPath, "*.csv");

            foreach (var file in files)
            {
              //  Console.WriteLine(file);
                var readings = File.ReadAllLines(file).Skip(1).Select(line =>
                {
                    var parts = line.Split(',');
                    return new { ID = uint.Parse(parts[0].Trim()), Time = DateTime.Parse(parts[1].Trim()), Rainfall = double.Parse(parts[2].Trim()) };
                }).ToList();
               // var recentReadings2;
                foreach (var devicesr in devicesrs)
                {
                //    Console.WriteLine(devicesr);
                //    Console.WriteLine(devicesrs);
                    var recentReadings1 = readings.Where(r => r.ID == devicesr.ID).ToList();
                    var length = recentReadings1.Count;

               //     Console.WriteLine(length);
                    if (length > 0)
                    {
                  //      Console.WriteLine(recentReadings1);
                        var line = recentReadings1[length - 1];
                   //     Console.WriteLine(line);
                        //var parts = line.Split(',');
                        var recentReadings = recentReadings1.Where(r => r.Time > line.Time.AddHours(-4)).ToList();

                        if (recentReadings.Any())
                        {
                            double averageRainfall = recentReadings.Average(r => r.Rainfall);
                            uint ID = devicesr.ID;
                            string status = GetStatus(averageRainfall);

                            Console.WriteLine($"{Path.GetFileNameWithoutExtension(file)}, {ID} - Average Rainfall: {averageRainfall:F2} mm, Status: {status}");
                        }
                    }
                }

                

            }
        }

            static string GetStatus(double averageRainfall)
            {
                if (averageRainfall < 10.0)
                    return "Green";
                else if (averageRainfall < 15.0 && averageRainfall >= 10)
                    return "Amber";
                else if (averageRainfall >= 15.0)
                    return "Red";
                else return "Green";
            }
     }
 }


