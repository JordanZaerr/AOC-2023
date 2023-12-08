using System.Diagnostics;

namespace Day5
{
    internal class Day5
    {
        public static string seedsSampleInput = "79 14 55 13";

        public static string seedsInput =
            "202517468 131640971 1553776977 241828580 1435322022 100369067 2019100043 153706556 460203450 84630899 3766866638 114261107 1809826083 153144153 2797169753 177517156 2494032210 235157184 856311572 542740109";

        public static List<long> seeds;
        public static Map SeedToSoil;
        public static Map SoilToFertilizer;
        public static Map FertilizerToWater;
        public static Map WaterToLight;
        public static Map LightToTemp;
        public static Map TempToHumidity;
        public static Map HumidityToLocation;

        static void Main()
        {
            // LoadSampleData();
            LoadData();

            var timer = Stopwatch.StartNew();
            Part1();//318728750
            timer.Stop();
            Console.WriteLine($"Part 1: {timer.ElapsedMilliseconds}ms");
            timer.Restart();
            Part2T2();//37384986
            Console.WriteLine($"Part 2: {timer.ElapsedMilliseconds}ms");

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        public static void Part1()
        {
            var answer = seeds.Min(ProcessSeed);
            Console.WriteLine(answer);
        }


        //This is brute forced and not fast (~60 minutes)
        public static void Part2()
        {
            var answer = 0L;
            var seedRanges = seeds.Chunk(2).Select(x => x.ToArray());

            foreach (var range in seedRanges)
            {
                for (long seed = range[0]; seed < range[0] + range[1]; seed++)
                {
                    var result = ProcessSeed(seed);
                    if(answer == 0 || result < answer)
                        answer = result;
                }
            }

            Console.WriteLine(answer);
        }

        //Same brute force just threaded (~20 minutes)
        //I'm working on something better, hopefully that pans out.
        public static void Part2T2()
        {
            var seedRanges = seeds.Chunk(2).Select(x => x.ToArray());

            var tasks = new List<Task<long>>();

            foreach (var range in seedRanges)
            {
                tasks.Add(Task.Run(() =>
                {
                    var answer = 0L;
                    for (long seed = range[0]; seed < range[0] + range[1]; seed++)
                    {
                        var result = ProcessSeed(seed);
                        if (answer == 0 || result < answer)
                            answer = result;
                    }
                    return answer;
                }));
            }

            Task.WaitAll(tasks.ToArray());

            var finalAnswer = tasks.Select(x => x.Result).Min();

            Console.WriteLine(finalAnswer);
        }


        public static long ProcessSeed(long seed)
        {
            var soil = SeedToSoil.GetDestination(seed);
            var fertilizer = SoilToFertilizer.GetDestination(soil);
            var water = FertilizerToWater.GetDestination(fertilizer);
            var light = WaterToLight.GetDestination(water);
            var temp = LightToTemp.GetDestination(light);
            var humidity = TempToHumidity.GetDestination(temp);
            var location = HumidityToLocation.GetDestination(humidity);
            return location;
        }

        public static void LoadSampleData()
        {
            seeds = seedsSampleInput
                .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(long.Parse).ToList();

            SeedToSoil = LoadMap("50 98 2|52 50 48".Split("|", StringSplitOptions.TrimEntries));
            SoilToFertilizer = LoadMap("0 15 37|37 52 2|39 0 15".Split("|", StringSplitOptions.TrimEntries));
            FertilizerToWater = LoadMap("49 53 8|0 11 42|42 0 7|57 7 4".Split("|", StringSplitOptions.TrimEntries));
            WaterToLight = LoadMap("88 18 7|18 25 70".Split("|", StringSplitOptions.TrimEntries));
            LightToTemp = LoadMap("45 77 23|81 45 19|68 64 13".Split("|", StringSplitOptions.TrimEntries));
            TempToHumidity = LoadMap("0 69 1|1 0 69".Split("|", StringSplitOptions.TrimEntries));
            HumidityToLocation = LoadMap("60 56 37|56 93 4".Split("|", StringSplitOptions.TrimEntries));
        }

        public static void LoadData()
        {
            seeds = seedsInput
                .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(long.Parse).ToList();

            SeedToSoil = LoadMap(File.ReadAllLines("Input\\seed-to-soil.txt"));
            SoilToFertilizer = LoadMap(File.ReadAllLines("Input\\soil-to-fertilizer.txt"));
            FertilizerToWater = LoadMap(File.ReadAllLines("Input\\fertilizer-to-water.txt"));
            WaterToLight = LoadMap(File.ReadAllLines("Input\\water-to-light.txt"));
            LightToTemp = LoadMap(File.ReadAllLines("Input\\light-to-temperature.txt"));
            TempToHumidity = LoadMap(File.ReadAllLines("Input\\temperature-to-humidity.txt"));
            HumidityToLocation = LoadMap(File.ReadAllLines("Input\\humidity-to-location.txt"));
        }


        public static Map LoadMap(string[] input)
        {
            var map = new Map();
            foreach (var line in input)
            {
                var values = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(long.Parse).ToArray();
                map.Ranges.Add(new Range
                {
                    DestinationStart = values[0],
                    SourceStart = values[1],
                    RangeLength = values[2]
                });
            }

            return map;
        }
    }

    [DebuggerDisplay("Ranges: {Ranges.Count}")]
    public class Map
    {
        public List<Range> Ranges { get; set; } = new();

        public long GetDestination(long input)
        {
            var matchingRange = Ranges.FirstOrDefault(x => x.ContainsValue(input));
            return matchingRange?.GetDestinationValue(input) ?? input;
        }
    }

    [DebuggerDisplay("{DestinationStart} {SourceStart} {RangeLength}")]
    public class Range
    {
        public long SourceStart { get; set; }
        public long DestinationStart { get; set; }
        public long RangeLength { get; set; }

        public bool ContainsValue(long input)
        {
            return SourceStart <= input && input < SourceStart + RangeLength;
        }

        public long GetDestinationValue(long input)
        {
            var offset = input - SourceStart;
            return DestinationStart + offset;
        }
    }
}
